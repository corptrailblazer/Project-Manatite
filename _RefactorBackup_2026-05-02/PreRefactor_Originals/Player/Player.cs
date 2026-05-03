using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public int hp = 5;
    [SerializeField] GameObject shieldPrefab;
    [SerializeField] GameObject shieldContainer;
    [SerializeField] RuntimeAnimatorController DieAnim;
    [SerializeField] GameObject defeat;
    [SerializeField] Image backgroundFade;
    [SerializeField] SaveLoader saveLoader;
    Animator animator;

    
    void Start()
    {
        animator = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                TakeDamage(other.gameObject.GetComponent<Enemy>().damage, false);
            }
                
        }
    }

    public virtual void TakeDamage(float dmg, bool isLoad)
    {
        hp -= (int)dmg;

        for (int i = shieldContainer.transform.childCount - 1; i >= 5 - dmg; i--)
        {
            Destroy(shieldContainer.transform.GetChild(i).gameObject);
        }
        
        if (!isLoad)
        {
            saveLoader.Save();
            GameObject shield = Instantiate(shieldPrefab, transform);
            Destroy(shield, .5f);
        }
        

        if (hp <= 0)
        {
            Die();
            return;
        }
    }

    protected virtual void Die()
    {
        string json = JsonUtility.ToJson(null, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "chips.json"),json);
        animator.runtimeAnimatorController = DieAnim;
        StartCoroutine(FadeRoutine());
        StartCoroutine(PauseGame());
        defeat.transform.localPosition = new Vector3(0f, 4f, 0f);

        foreach (Transform child in this.gameObject.GetComponentsInChildren<Transform>(true))
        {
            if (child == this.transform) continue;
            Destroy(child.gameObject);
        }
       
    }

    IEnumerator PauseGame()
    {
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0f;
    }

    IEnumerator FadeRoutine()
    {
        float start = 0f;
        float end = 0.95f;
        float t = 0f;
        float duration = 1.4f;

        Color c = backgroundFade.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(start, end, t / duration);
            backgroundFade.color = c;
            yield return null;
        }

        c.a = end;
        backgroundFade.color = c;
    }
}
