using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;



[System.Serializable]
public struct EquippedChips
{
    public string name;
    public string eqturret;

}

[System.Serializable]
public struct SaveStruct
{
    public List<EquippedChips> eqChips;
    public List<string> chips;
    public int qntyMantatite;
    public int health;
}



public class SaveLoader : MonoBehaviour
{

    [SerializeField] private GameObject inventory;
    [SerializeField] private List<GameObject> turrets;
    [SerializeField] private Player playerScript;
    [SerializeField] private Coins coinsScript;
    private SaveStruct data;
    string path;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data = new SaveStruct();
        path = Path.Combine(Application.persistentDataPath, "Save.json");
        Load();
        
    }


    public void Save()
    {
        SaveChip();
        SaveConditions();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path,json);

    }


    void SaveConditions()
    {
        data.qntyMantatite = coinsScript.qntyMantatite;
        data.health = playerScript.hp;
    }



    void SaveChip()
    {
        List<string> chips = new List<string>();
        List<EquippedChips> equippedChips = new List<EquippedChips>();

        foreach (Transform child in inventory.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("Chip"))
                chips.Add(child.name.Replace("(Clone)", "").Trim());
        }

        foreach(GameObject turret in turrets)
        {
            EquippedChips eqchip = new EquippedChips();

            foreach (Transform child in turret.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag("Chip"))
                {
                    eqchip.name = child.name.Replace("(Clone)", "").Trim();
                    eqchip.eqturret = turret.name;
                    equippedChips.Add(eqchip);
                }
                    

            }

        data.eqChips = equippedChips;
        data.chips = chips;

            
        }

    }

    void Load()
    {
        string json = File.ReadAllText(path);
        data = JsonUtility.FromJson<SaveStruct>(json);
        StartCoroutine(LoadChips());
        LoadConditions();
    }

    void LoadConditions()
    {
        coinsScript.updateCoin(data.qntyMantatite);
        playerScript.TakeDamage(5 - data.health, true);
    }

    IEnumerator LoadChips()
    {
        yield return new WaitForSeconds(1f);
        

        List<string> chips = data.chips;

        foreach (string name in chips)
        {
            GameObject chipPrefab = Resources.Load<GameObject>(name);
            GameObject chip = Instantiate(chipPrefab);
            yield return new WaitForSeconds(.01f);
            chip.GetComponent<Chip>().Collect();

        }

        List<EquippedChips> eqChips = data.eqChips;

        foreach (EquippedChips eqChip in eqChips)
        {
            GameObject chipPrefab = Resources.Load<GameObject>(eqChip.name);
            GameObject chip = Instantiate(chipPrefab);
            yield return new WaitForSeconds(.01f);
            chip.GetComponent<Chip>().Collect();
            GameObject turret = GameObject.Find(eqChip.eqturret);
            SlotBehaviour slotScript = null;
            foreach (Transform slot in turret.GetComponentsInChildren<Transform>(true))
            {
                var curSlot = slot.gameObject.GetComponent<SlotBehaviour>();
                if (curSlot == null) continue;
                if(curSlot.occupiedChip == null)
                {
                    slotScript = curSlot;
                    break;
                }
                    
            }  
            Chip isChip = chip.GetComponent<Chip>();
            slotScript.LoadEquip(isChip);

        }


    }

}
