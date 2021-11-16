using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.UI;
using ATDungeon.Utility;

namespace ATDungeon.Loot
{
    public class Chest : MonoBehaviour
    {
        [SerializeField]
        private BarUI chestUI;
        [SerializeField]
        private GameObject itemToDrop;
        [SerializeField]
        private float timeNeededInTrigger;
        [SerializeField]
        private Transform itemSpawnLocation;

        private float timeSpentInTrigger;
        // Start is called before the first frame update

        void Start()
        {
            chestUI.SetupBarSlider(timeNeededInTrigger);
            chestUI.UpdateBarUI(timeSpentInTrigger, "Stand Next To Chest To Open: ");
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                timeSpentInTrigger += Time.deltaTime;
                chestUI.UpdateBarUI(timeSpentInTrigger, "Opening Chest: ");
                if (timeSpentInTrigger >= timeNeededInTrigger)
                {
                    GameObject item = Instantiate(itemToDrop, itemSpawnLocation);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.parent = Dungeon.DungeonManager.instance.GetItemSpawnParent();
                    DestroyChest();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                timeSpentInTrigger = 0.0f;
                chestUI.UpdateBarUI(timeSpentInTrigger, "Stand Next To Chest To Open: ");
            }
        }

        private void DestroyChest()
        {
            FXManager.Instance.PlayEffect(FXManager.EffectType.ChestDestroy, this.gameObject.transform.position, gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            Destroy(this.gameObject);
        }
    }
}
