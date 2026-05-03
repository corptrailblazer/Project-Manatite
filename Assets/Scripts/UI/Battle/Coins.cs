using ProjectManatite.Core;
using TMPro;
using UnityEngine;

/// <summary>
/// HUD coin counter. Tracks accumulated Manatite (the in-game currency) and
/// keeps a TextMeshPro label in sync.
///
/// <para>Refactor notes vs. the original Coins.cs:</para>
/// <list type="bullet">
///   <item>Subscribes to <see cref="GameEvents.CoinsChanged"/> so producers (Enemy drops, shop, save load) don't need a direct reference to this component.</item>
///   <item><c>updateCoin</c> kept for backwards compatibility with <see cref="SaveLoader"/>; new callers should use the event channel or <see cref="UpdateCoin"/>.</item>
/// </list>
/// </summary>
public class Coins : MonoBehaviour
{
    [Tooltip("Current Manatite total. Serialized field name retained so existing save JSON deserializes unchanged.")]
    public int qntyMantatite;

    [SerializeField] private TextMeshPro text;

    private void OnEnable()
    {
        GameEvents.CoinsChanged += OnCoinsChangedEvent;
    }

    private void OnDisable()
    {
        GameEvents.CoinsChanged -= OnCoinsChangedEvent;
    }

    private void Start()
    {
        Refresh();
    }

    /// <summary>Backwards-compat alias preserved for existing callers.</summary>
    public void updateCoin(int qnt) => AddCoins(qnt);

    /// <summary>Add (or subtract, if negative) to the running coin total.</summary>
    public void UpdateCoin(int delta) => AddCoins(delta);

    public void AddCoins(int delta)
    {
        qntyMantatite += delta;
        Refresh();
    }

    public void SetCoins(int amount)
    {
        qntyMantatite = Mathf.Max(0, amount);
        Refresh();
    }

    private void OnCoinsChangedEvent(int delta)
    {
        AddCoins(delta);
    }

    private void Refresh()
    {
        if (text != null) text.text = qntyMantatite.ToString();
    }
}
