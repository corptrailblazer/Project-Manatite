using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Procedural mini-solar-system layout: spawns <see cref="planetQty"/> random
/// planets stacked vertically with arrows between them.
///
/// Refactor notes: null guards on prefab list; for-index iteration; cached
/// reusable Vector3 for arrow offset.
/// </summary>
public class SolarSystemCreator : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new();
    [SerializeField] private GameObject arrowprefab;
    [SerializeField] private Transform parentObject;
    [SerializeField, Min(0)] private int planetQty;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private Vector3 arrowOffsetFromPlanet = new(0f, 2.3f, 0f);

    private void Start()
    {
        if (prefabs == null || prefabs.Count == 0 || parentObject == null) return;

        for (int i = 0; i < planetQty; i++)
        {
            var prefab = prefabs[Random.Range(0, prefabs.Count)];
            if (prefab == null) continue;

            var planet = Instantiate(prefab, parentObject);
            planet.transform.localPosition = new Vector3(0f, i * spacing, 0f);
            planet.transform.localRotation = Quaternion.identity;
            planet.transform.localScale = Vector3.one;

            if (i < planetQty - 1 && arrowprefab != null)
            {
                var arrow = Instantiate(arrowprefab, parentObject);
                arrow.transform.localPosition = planet.transform.localPosition + arrowOffsetFromPlanet;
                arrow.transform.localScale = Vector3.one;
            }
        }
    }
}
