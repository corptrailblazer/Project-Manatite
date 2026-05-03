using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProjectManatite.Core;
using UnityEngine;

/// <summary>
/// One entry in the save file: which chip is on which turret.
/// Field names are kept identical to the original so existing save JSON
/// deserializes unchanged.
/// </summary>
[System.Serializable]
public struct EquippedChips
{
    public string name;
    public string eqturret;
}

/// <summary>
/// On-disk save payload. JSON layout is unchanged from the original.
/// </summary>
[System.Serializable]
public struct SaveStruct
{
    public List<EquippedChips> eqChips;
    public List<string> chips;
    public int qntyMantatite;
    public int health;
}

/// <summary>
/// Serializes / deserializes <see cref="SaveStruct"/> to disk and applies it
/// to the relevant runtime systems (player HP, coin total, inventory, equipped chips).
///
/// <para>Refactor notes vs. the original SaveLoader.cs:</para>
/// <list type="bullet">
///   <item>Subscribes to <see cref="GameEvents.SaveRequested"/> so callers don't need a direct reference to perform a save.</item>
///   <item><c>Player.TakeDamage(5 - health, true)</c> hack replaced with <see cref="Player.RestoreHealth(int)"/> — explicit semantics.</item>
///   <item>Player health key uses <see cref="Player.Health"/> instead of <c>playerScript.hp</c> (private now).</item>
///   <item>Load is wrapped in <c>try/catch</c>; a missing or malformed save file no longer crashes the scene start.</item>
///   <item>Inventory iteration uses <see cref="Inventory.Slots"/> rather than walking the tag tree.</item>
///   <item>JSON path no longer hard-coded to "Save.json"; configurable via <see cref="saveFileName"/>.</item>
/// </list>
/// </summary>
public class SaveLoader : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private List<GameObject> turrets;
    [SerializeField] private Player playerScript;
    [SerializeField] private Coins coinsScript;
    [SerializeField] private string saveFileName = "Save.json";
    [SerializeField] private float loadInitialDelaySeconds = 1f;
    [SerializeField] private float loadPerChipDelaySeconds = 0.01f;

    private SaveStruct data;
    private string path;

    private void OnEnable()
    {
        GameEvents.SaveRequested += Save;
    }

    private void OnDisable()
    {
        GameEvents.SaveRequested -= Save;
    }

    private void Start()
    {
        data = new SaveStruct();
        path = Path.Combine(Application.persistentDataPath, saveFileName);
        Load();
    }

    // ---------- Save ----------

    public void Save()
    {
        SaveChip();
        SaveConditions();

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveLoader] Save failed at '{path}': {e.Message}", this);
        }
    }

    private void SaveConditions()
    {
        if (coinsScript != null) data.qntyMantatite = coinsScript.qntyMantatite;
        if (playerScript != null) data.health = playerScript.Health;
    }

    private void SaveChip()
    {
        var chips = new List<string>();
        var equippedChips = new List<EquippedChips>();

        if (inventory != null)
        {
            foreach (Transform child in inventory.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag("Chip"))
                    chips.Add(child.name.Replace("(Clone)", string.Empty).Trim());
            }
        }

        if (turrets != null)
        {
            for (int i = 0; i < turrets.Count; i++)
            {
                var turret = turrets[i];
                if (turret == null) continue;

                foreach (Transform child in turret.GetComponentsInChildren<Transform>(true))
                {
                    if (!child.CompareTag("Chip")) continue;
                    equippedChips.Add(new EquippedChips
                    {
                        name = child.name.Replace("(Clone)", string.Empty).Trim(),
                        eqturret = turret.name,
                    });
                }
            }
        }

        data.eqChips = equippedChips;
        data.chips = chips;
    }

    // ---------- Load ----------

    private void Load()
    {
        if (!File.Exists(path)) return;

        try
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveStruct>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[SaveLoader] Failed to read '{path}': {e.Message}", this);
            return;
        }

        StartCoroutine(LoadChipsRoutine());
        LoadConditions();
    }

    private void LoadConditions()
    {
        if (coinsScript != null) coinsScript.SetCoins(data.qntyMantatite);
        if (playerScript != null) playerScript.RestoreHealth(data.health);
    }

    private IEnumerator LoadChipsRoutine()
    {
        // The original used a 1s warm-up before loading chips so all
        // dependencies (Inventory, Turrets) had time to spawn. Preserved.
        yield return new WaitForSeconds(loadInitialDelaySeconds);

        if (data.chips != null)
        {
            for (int i = 0; i < data.chips.Count; i++)
            {
                var prefab = Resources.Load<GameObject>(data.chips[i]);
                if (prefab == null) continue;

                var go = Instantiate(prefab);
                yield return new WaitForSeconds(loadPerChipDelaySeconds);
                if (go.TryGetComponent<Chip>(out var chip)) chip.Collect(requestSave: false);
            }
        }

        if (data.eqChips != null)
        {
            for (int i = 0; i < data.eqChips.Count; i++)
            {
                var eq = data.eqChips[i];
                var prefab = Resources.Load<GameObject>(eq.name);
                if (prefab == null) continue;

                var go = Instantiate(prefab);
                yield return new WaitForSeconds(loadPerChipDelaySeconds);
                if (!go.TryGetComponent<Chip>(out var chip)) continue;
                chip.Collect(requestSave: false);

                var turret = GameObject.Find(eq.eqturret);
                if (turret == null) continue;

                SlotBehaviour targetSlot = null;
                foreach (Transform slot in turret.GetComponentsInChildren<Transform>(true))
                {
                    if (!slot.TryGetComponent<SlotBehaviour>(out var s)) continue;
                    if (s.occupiedChip == null) { targetSlot = s; break; }
                }

                if (targetSlot != null) targetSlot.LoadEquip(chip);
            }
        }

        GameEvents.RaiseLoadCompleted();
    }
}
