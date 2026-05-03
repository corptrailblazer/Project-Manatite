using UnityEngine;
using TMPro;
using System.Collections;


public class DamageTextBeheviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    TextMeshPro damageTxt;
    Vector3 movementTxt = new Vector3(0, 0.001f, 0);

    void Start()
    {
        damageTxt = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(fadeOut());
    }


    IEnumerator fadeOut()
    {

        while (damageTxt.alpha > 0)
        {
            damageTxt.alpha -= 0.001f;
            transform.position = transform.position + movementTxt;
            yield return new WaitForSeconds(0.1f);

        }

        Destroy(gameObject);
        
    }
}
