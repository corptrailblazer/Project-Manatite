    using UnityEngine;
    using TMPro;

public class Coins : MonoBehaviour
{
    public int qntyMantatite = 0;
    [SerializeField] TextMeshPro text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = qntyMantatite.ToString();
    }

    public void updateCoin(int qnt)
    {
        qntyMantatite += qnt;
        text.text = qntyMantatite.ToString();
    }

    
}
