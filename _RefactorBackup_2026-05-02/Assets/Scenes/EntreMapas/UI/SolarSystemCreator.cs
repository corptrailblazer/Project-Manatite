using UnityEngine;
using System.Collections.Generic;

public class SolarSystemCreator : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] private GameObject arrowprefab;
    [SerializeField] private Transform parentObject;
    [SerializeField] private int planetQty;
    [SerializeField] private float spacing;


    void Start()
    {
        GameObject planet;
        GameObject arrow;

        for (int i = 0; i < planetQty; i++)
        {
            GameObject prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];
            planet = Instantiate(prefab, parentObject);

            planet.transform.localPosition = new Vector3(0f, i * spacing, 0f);

            planet.transform.localRotation = Quaternion.identity;
            planet.transform.localScale = Vector3.one;

            if(i < planetQty -1)
            {
                arrow = Instantiate(arrowprefab, parentObject);

                arrow.transform.localPosition = planet.transform.localPosition + new Vector3(0f, 2.3f, 0f);

                arrow.transform.localScale = Vector3.one;
            }

            
        }
    }

}
