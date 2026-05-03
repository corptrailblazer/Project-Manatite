/// <summary>
/// Thin proxy that forwards chip apply / unapply requests from a chip slot UI to
/// a specific turret. Originally lived in <c>Assets/Scenes/BattleMap/UI/</c> —
/// kept in <c>UI/Battle/</c> after the move so existing prefab GUID references
/// still resolve.
///
/// Empty <c>Start</c> / <c>Update</c> from the original have been deleted.
/// </summary>
using UnityEngine;

public class TurretInventory : MonoBehaviour
{
    [Tooltip("Local position of the popup when shown.")]
    public Vector2 placement;

    [SerializeField] private WeaponBehaviour selectedTurret;

    public void Apply(Chip chip)
    {
        if (selectedTurret == null || chip == null) return;
        selectedTurret.Apply(chip);
    }

    public void Unapply(Chip chip)
    {
        if (selectedTurret == null || chip == null) return;
        selectedTurret.Unapply(chip);
    }
}
