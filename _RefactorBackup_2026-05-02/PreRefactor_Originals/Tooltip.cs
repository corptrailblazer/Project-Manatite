using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{

    public Vector2 toolPlacement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateInfo(Chip chipObj)
    {
        ChipSO chip = chipObj.chipSO;
        TextMeshPro title = transform.Find("Title").GetComponent<TextMeshPro>();
        title.text = chip.chipName;

        TextMeshPro effect = transform.Find("Effect").GetComponent<TextMeshPro>();
        effect.text =  chip.effect;

    }
}
