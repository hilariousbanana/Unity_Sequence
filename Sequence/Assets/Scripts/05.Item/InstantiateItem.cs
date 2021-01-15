using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateItem : MonoBehaviour
{
    public GameObject Key;
    public GameObject HealPackA;
    public GameObject HealPackB;
    public Transform ItemDropTransform;
    public EnemyData stat;

    public void Awake()
    {
        stat = GetComponent<EnemyData>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var clone = InstantItem();

        if(clone)
            Instantiate(InstantItem(), ItemDropTransform.position, ItemDropTransform.rotation);
    }

    GameObject InstantItem()
    {
        int drop = Random.Range(0, 101);

        Debug.Log(drop + " " + stat.ItemDrop);

        if (drop <= stat.ItemDrop)
        {
            int item = Random.Range(0, (stat.KeyDrop + stat.HealPackADrop + stat.HealPackBDrop) + 1);
            Debug.Log(item);
            if (item <= stat.KeyDrop) // key
            {
                return Key;
            }
            else if (item <= stat.KeyDrop + stat.HealPackADrop) //HealPackA
            {
                ItemDropTransform.rotation = Quaternion.identity;
                return HealPackA;
            }
            else //HealPackB
            {
                ItemDropTransform.rotation = Quaternion.identity;
                return HealPackB;
            }
        }
        else
        {
            Debug.Log("Couldn't Instantiate Item According to Item Drop Rate.");
        }
        return null;
    }
}
