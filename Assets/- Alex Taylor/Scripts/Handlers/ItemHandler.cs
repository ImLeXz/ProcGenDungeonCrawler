using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Items;
using System;

namespace ATDungeon.Handlers
{
    public class ItemHandler : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> itemsList = new List<GameObject>();
        [SerializeField]
        private bool canUseMultipleItems;
        [SerializeField]
        private Sprite emptyItemIcon;

        private bool isUsingItem = false;
        private int currentItemCount, maxItemCount;
        private int itemID = 0;
        ItemPickupCallBack itemCallBack;

        private void Awake()
        {
            List<GameObject> instantiatedObjects = new List<GameObject>();
            for (int i = 0; i < itemsList.Count; i++)
            {
                if (itemsList[i] != null)
                {
                    instantiatedObjects.Add(Instantiate(itemsList[i], this.transform));
                    currentItemCount++;
                }
                else
                {
                    instantiatedObjects.Add(null);
                }
            }

            itemsList = instantiatedObjects;
            maxItemCount = itemsList.Count;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UseItem(int itemIndex)
        {
            if (!isUsingItem || canUseMultipleItems)
            {
                GameObject itemObj = itemsList[itemIndex];
                if (itemObj != null)
                {
                    ItemBase item = itemObj.GetComponent<ItemBase>();
                    if (item != null)
                    {
                        item.UseItem();
                    }
                }
            }
        }

        public void UpdateItem(ItemBase item)
        {
            int itemIndex = GetItemNum(item.gameObject);
            itemCallBack(itemIndex, item.GetItemImage(), item.GetUsesRemaining(), item.GetCurrentState());
        }

        private int GetItemNum(GameObject item) { return itemsList.FindIndex(x => x == item); }

        public bool AddItem(GameObject item)
        {
            Debug.Log("Adding item: " + item.name);
            bool itemEquiped = false;
            ItemBase itemB = null;
            int itemNum = 0;
            for (int i = 0; i < maxItemCount; i++)
            {
                if (itemsList[i] == null)
                {
                    itemNum = i;
                    GameObject itemObj = Instantiate(item, this.transform);
                    itemObj.name += itemID;
                    itemID++;
                    itemsList[i] = itemObj;
                    itemB = itemsList[i].GetComponent<ItemBase>();
                    itemB.SetCurrentUses(item.GetComponent<ItemBase>().GetCurrentUses());
                    if (!Utility.UtilFunctions.instance.CheckIfPrefab(item))
                        Destroy(item);
                    itemB.SetItemHandler(this);
                    currentItemCount++;
                    itemEquiped = true;
                    break;
                }
            }

            if(itemB != null)
                itemCallBack(itemNum, itemB.GetItemImage(), itemB.GetUsesRemaining(), itemB.GetCurrentState());

            return itemEquiped;
        }

        public void RemoveItem(GameObject item)
        {
            int itemNum = itemsList.FindIndex(x => x == item);
            itemsList[itemNum] = null;
            currentItemCount--;
            Destroy(item);
            itemCallBack(itemNum, emptyItemIcon, 0, ItemBase.ItemState.Idle);
        }

        public GameObject[] GetItems() { return itemsList.ToArray(); }
        public bool GetIsUsingItem() { return isUsingItem; }
        public void SetIsUsingItem(bool b) { isUsingItem = b; }
        public void SetItemPickupCallback(ItemPickupCallBack cb) { itemCallBack = cb; }

        public delegate void ItemPickupCallBack(int itemNum, Sprite itemIcon, int itemUses, Items.ItemBase.ItemState itemState);
    }
}
