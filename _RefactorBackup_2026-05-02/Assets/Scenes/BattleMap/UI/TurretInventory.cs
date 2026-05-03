using UnityEngine;

public class TurretInventory : MonoBehaviour
{
    public Vector2 placement;
    [SerializeField] WeaponBehaviour selectedTurret;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Apply(Chip chip)
    {
        selectedTurret.Apply(chip);
    }

    public void Unapply(Chip chip)
    {
        selectedTurret.Unapply(chip);
    }

}
