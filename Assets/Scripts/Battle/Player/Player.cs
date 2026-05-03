using System;
using System.Collections;
using System.IO;
using ProjectManatite.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// The player ship. Owns hit points, the destructible shield visual stack, and the
/// death sequence (animator swap, fade, time-scale freeze).
///
/// <para>Refactor notes vs. the original Player.cs:</para>
/// <list type="bullet">
///   <item>Implements <see cref="IDamageable"/> so other systems (projectiles, hazards) can damage the player without coupling to this concrete type.</item>
///   <item>The original <c>TakeDamage(float, bool isLoad)</c> dual-purpose API is split into <see cref="TakeDamage(float, GameObject)"/> (normal damage path) and <see cref="RestoreHealth(int)"/> (called only by the save system).</item>
///   <item>Empty <c>Update</c> removed.</item>
///   <item>Animator and transform are cached in <c>Awake</c> instead of <c>Start</c> so they're available before the first frame.</item>
///   <item>Save side-effects route through <see cref="GameEvents.SaveRequested"/> instead of a direct <c>SaveLoader</c> call (loose coupling).</item>
///   <item>Shield-decrement logic now driven by <c>currentHp</c> rather than a hardcoded base of 5.</item>
/// </list>
/// </summary>
[DisallowMultipleComponent]
public class Player : MonoBehaviour, IDamageable
{
    // ----- Serialized config -----

    [Header("Stats")]
    [Tooltip("Starting / max hit points. The number of children initially under shieldContainer is expected to equal this value.")]
    [SerializeField, FormerlySerializedAs("hp")] private int _hp = 5;

    [Header("Visuals")]
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject shieldContainer;
    [SerializeField] private RuntimeAnimatorController dieAnim;
    [SerializeField] private GameObject defeat;
    [SerializeField] private Image backgroundFade;

    [Header("Death sequence timings")]
    [SerializeField] private float shieldEffectLifetime = 0.5f;
    [SerializeField] private float pauseDelaySeconds = 1.5f;
    [SerializeField] private float fadeDurationSeconds = 1.4f;
    [SerializeField] private float fadeTargetAlpha = 0.95f;
    [SerializeField] private Vector3 defeatPanelLocalPos = new(0f, 4f, 0f);

    [Header("Save integration")]
    [Tooltip("Optional. If left null, save events go via GameEvents.SaveRequested instead of a direct call.")]
    [SerializeField] private SaveLoader saveLoader;

    // ----- Runtime cache -----

    private Animator _animator;
    private int _maxHp;

    // ----- IDamageable -----

    public float CurrentHealth => _hp;
    public float MaxHealth => _maxHp;
    public bool IsDead => _hp <= 0;

    /// <summary>Integer HP used by the save system.</summary>
    public int Health => _hp;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    // ----- Lifecycle -----

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _maxHp = Mathf.Max(_hp, 1);
    }

    private void Start()
    {
        // Broadcast initial HP so UI can latch on without polling.
        OnHealthChanged?.Invoke(_hp, _maxHp);
        GameEvents.RaisePlayerHealthChanged(_hp, _maxHp);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemies physically ramming the player deal contact damage.
        if (other.attachedRigidbody == null) return;
        if (!other.gameObject.CompareTag("Enemy")) return;

        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            TakeDamage(enemy.Damage, other.gameObject);
        }
    }

    // ----- Damage / heal -----

    /// <summary>
    /// Apply incoming damage. Spawns the shield-hit FX and triggers a save.
    /// Use <see cref="RestoreHealth"/> for save-load restoration instead.
    /// </summary>
    public virtual void TakeDamage(float dmg, GameObject source = null)
    {
        if (IsDead) return;
        ApplyDamageInternal((int)dmg);

        if (saveLoader != null) saveLoader.Save();
        else GameEvents.RaiseSaveRequested();

        if (shieldPrefab != null)
        {
            var shieldFx = Instantiate(shieldPrefab, transform);
            Destroy(shieldFx, shieldEffectLifetime);
        }

        if (IsDead) Die();
    }

    /// <summary>
    /// Sets HP directly (no FX, no save). Call this from the save system to
    /// restore a previously-persisted health value.
    /// </summary>
    public void RestoreHealth(int newHp)
    {
        int clamped = Mathf.Clamp(newHp, 0, _maxHp);
        int delta = _hp - clamped;
        if (delta <= 0)
        {
            _hp = clamped;
            OnHealthChanged?.Invoke(_hp, _maxHp);
            GameEvents.RaisePlayerHealthChanged(_hp, _maxHp);
            return;
        }
        ApplyDamageInternal(delta);
        if (IsDead) Die();
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || IsDead) return;
        _hp = Mathf.Min(_hp + (int)amount, _maxHp);
        OnHealthChanged?.Invoke(_hp, _maxHp);
        GameEvents.RaisePlayerHealthChanged(_hp, _maxHp);
    }

    // Common path for damage / restore: decrement HP, prune shield children,
    // raise the OnHealthChanged signal exactly once.
    private void ApplyDamageInternal(int dmg)
    {
        if (dmg <= 0) return;
        _hp = Mathf.Max(0, _hp - dmg);

        if (shieldContainer != null)
        {
            // Keep `_hp` shields. Children index 0.._maxHp-1 represent shields top-down.
            int desiredCount = _hp;
            for (int i = shieldContainer.transform.childCount - 1; i >= desiredCount; i--)
            {
                Destroy(shieldContainer.transform.GetChild(i).gameObject);
            }
        }

        OnHealthChanged?.Invoke(_hp, _maxHp);
        GameEvents.RaisePlayerHealthChanged(_hp, _maxHp);
    }

    // ----- Death sequence -----

    protected virtual void Die()
    {
        OnDied?.Invoke();
        GameEvents.RaisePlayerDied();

        // Wipe persisted chip data so the next run starts clean. The original
        // implementation hardcoded the path here; we keep it for behavioural
        // parity but isolate it in a helper.
        WipePersistedChips();

        if (_animator != null && dieAnim != null)
            _animator.runtimeAnimatorController = dieAnim;

        StartCoroutine(FadeRoutine());
        StartCoroutine(PauseGameRoutine());

        if (defeat != null) defeat.transform.localPosition = defeatPanelLocalPos;

        // Strip every child (shields, particle systems, etc.) so the death pose is clean.
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child == transform) continue;
            Destroy(child.gameObject);
        }
    }

    private static void WipePersistedChips()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "chips.json");
            File.WriteAllText(path, JsonUtility.ToJson(null, true));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Player] Could not wipe chips.json: {e.Message}");
        }
    }

    private IEnumerator PauseGameRoutine()
    {
        yield return new WaitForSeconds(pauseDelaySeconds);
        Time.timeScale = 0f;
    }

    private IEnumerator FadeRoutine()
    {
        if (backgroundFade == null) yield break;

        float t = 0f;
        Color c = backgroundFade.color;
        float startA = c.a;

        while (t < fadeDurationSeconds)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startA, fadeTargetAlpha, t / fadeDurationSeconds);
            backgroundFade.color = c;
            yield return null;
        }

        c.a = fadeTargetAlpha;
        backgroundFade.color = c;
    }
}
