using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private PlayerController player;
    public enum ItemType
    {
        Key,
        HealPackA,
        HealPackB
    }

    private SphereCollider col;
    public ItemType type;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<SphereCollider>();
        player = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            switch (type)
            {
                case ItemType.Key:
                    DataController.instance.data.HaveKey = true;
                    break;
                case ItemType.HealPackA:
                    DataController.instance.data.CurrentPotionCount++;
                    player.ChangeHP(30);
                    break;
                case ItemType.HealPackB:
                    DataController.instance.data.CurrentPotionCount++;
                    player.ChangeHP(60);
                    break;
            }

            Destroy(gameObject);
        }
    }
}
