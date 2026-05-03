using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Chips", menuName = "Chips/BaseChip")]
public class ChipSO : ScriptableObject
{

    [Header("Modifiers")]
    [SerializeField, Range(-5f, 5f)] public float damagePct;
    [SerializeField] public float damageFlat;
    [SerializeField, Range(-5f, 5f)] public float fireRatePct;
    [SerializeField] public float fireRateFlat;
    [SerializeField, Range(-5f, 5f)] public float projSpeedPct;
    [SerializeField] public float projSpeedFlat;
    [SerializeField] public ModifierInterface modifier;

    [SerializeField] public string chipName;
    [SerializeField] public string effect;

    [Header("Drop Rate")]
    [Range(0f, 1f)] public float dropRate;

    [SerializeField] public Sprite sprite;
}
