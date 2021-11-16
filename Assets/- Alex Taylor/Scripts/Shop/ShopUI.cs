using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.Items;
using ATDungeon.Weapons;
using ATDungeon.Utility;
using ATDungeon.Player;

namespace ATDungeon.UI.Shop
{
    public class ShopUI : MonoBehaviour
    {
        public enum EntryType { Weapon, Item };

        [System.Serializable]
        public class ShopEntry
        {
            public EntryType entryType;
            public GameObject entryPrefab;
            [Range(0.0f, 1.0f)]public float entryFrequencyPercentage;

            public void SetEntryCost(int n) { entryCost = n; }
            public int GetEntryCost() { return entryCost; }
            private int entryCost;

            public void SetEntryName(string n) { entryName = n; }
            public string GetEntryName() { return entryName; }
            private string entryName;
        }

        [Header("Misc References")]
        [SerializeField]
        private GameObject UIBlurring;
        [SerializeField]
        private TextMeshProUGUI currentCoinTxt;
        [SerializeField]
        private GameObject entryObjectTemplate;
        [SerializeField]
        private Transform entryObjectParent;

        [Header("Error UI References")]
        [SerializeField]
        private GameObject errorUIParent;
        [SerializeField]
        private TextMeshProUGUI errorTxt;

        [Header("Confirmation UI References")]
        [SerializeField]
        private GameObject confirmationUIParent;
        [SerializeField]
        private TextMeshProUGUI confirmTxt;

        [Header("Purchased Item Drops")]
        [SerializeField]
        private GameObject pickupPrefab;

        [Header("Shop Entries")]
        [SerializeField]
        private ShopEntry[] shopEntries;

        PlayerController playerController;
        private Vector3 pickupSpawnPos;
        private ShopEntry purchasedEntry;
        int currentCoins = 0;
        bool firstOpen = true;

        bool isOpen;
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        public void OpenShop()
        {
            this.gameObject.SetActive(true);
            if (firstOpen)
            {
                playerController = UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>();
                UpdateVariables();
                Initialise();
                UIBlurring.SetActive(true);
                firstOpen = false;
            }

            else
            {
                UIBlurring.SetActive(true);
                UpdateVariables();
            }
            isOpen = true;
            UIManager.Instance.ClosePrompt();
            playerController.TogglePlayerAiming(false);
        }

        private void UpdateVariables()
        {
            currentCoins = playerController.GetCurrentCoins();
            currentCoinTxt.text = "Current Coins: " + currentCoins.ToString();
        }

        public void CloseShop()
        {
            if (this.gameObject.activeInHierarchy)
            {
                isOpen = false;
                playerController.TogglePlayerAiming(true);
                UIBlurring.SetActive(false);
                confirmationUIParent.SetActive(false);
                errorUIParent.SetActive(false);
                this.gameObject.SetActive(false);
            }
        }

        private void Initialise()
        {
            for(int i = 0; i < shopEntries.Length; i++)
            {
                float freqPercent = shopEntries[i].entryFrequencyPercentage;
                float[] weight = new float[] { 1.0f - freqPercent, freqPercent };
                int shouldAddEntry = UtilFunctions.instance.GetWeightedRandomValue(weight,0);

                if (shouldAddEntry == 1)
                {
                    GameObject instantiatedEntryUI = Instantiate(entryObjectTemplate, entryObjectParent);
                    ShopEntryObjectHelper shopEntryObjectHelper = instantiatedEntryUI.GetComponent<ShopEntryObjectHelper>();
                    ShopEntryObjectHelper.EntryObjectHelper entryObjectHelper = shopEntryObjectHelper.GetEntryObject();
                    entryObjectHelper.shopEntry = shopEntries[i];

                    Sprite entryImg = null;
                    int entryCost = -1;
                    string entryName = "";
                    switch (entryObjectHelper.shopEntry.entryType)
                    {
                        case EntryType.Item:
                            ItemBase itemBase = entryObjectHelper.shopEntry.entryPrefab.GetComponent<ItemBase>();
                            entryCost = itemBase.GetItemPurchaseValue();
                            entryImg = itemBase.GetItemImage();
                            entryName = itemBase.GetItemName();
                            break;
                        case EntryType.Weapon:
                            WeaponBase weaponBase = entryObjectHelper.shopEntry.entryPrefab.GetComponent<WeaponBase>();
                            entryCost = weaponBase.GetWeaponPurchaseValue();
                            entryImg = weaponBase.GetWeaponImage();
                            entryName = weaponBase.GetWeaponName();
                            break;
                    }
                    entryObjectHelper.entryImage.sprite = entryImg;
                    entryObjectHelper.costTxt.text = entryCost.ToString();
                    entryObjectHelper.nameTxt.text = entryName;
                    entryObjectHelper.shopEntry.SetEntryCost(entryCost);
                    entryObjectHelper.shopEntry.SetEntryName(entryName);
                    entryObjectHelper.purchaseBtn.onClick.AddListener(delegate { PurchaseButtonPressed(entryObjectHelper, entryCost); });
                }
            }
        }

        private void DisplayError(string errorMsg)
        {
            errorUIParent.SetActive(true);
            errorTxt.text = errorMsg;
        }

        private void DisplayConfirmation(string name, int cost)
        {
            confirmationUIParent.SetActive(true);
            confirmTxt.text = "Purchase: [" + name + "] for: [" + cost + "] ?";
        }

        public void PurchaseButtonPressed(ShopEntryObjectHelper.EntryObjectHelper entryObjectHelper, int cost)
        {
            if (currentCoins - cost < 0)
                DisplayError("Not enough coins to purchase this product!");

            else
            {
                purchasedEntry = entryObjectHelper.shopEntry;
                DisplayConfirmation(entryObjectHelper.shopEntry.GetEntryName(), cost);
            }
        }

        public void ConfirmPurchase()
        {
            GameObject purchasedPickup = Instantiate(pickupPrefab);
            RandomDrop drop = purchasedPickup.GetComponent<RandomDrop>();
            drop.SetObject(purchasedEntry.entryPrefab);
            purchasedPickup.transform.localPosition = pickupSpawnPos;
            purchasedPickup.transform.parent = Dungeon.DungeonManager.instance.GetItemSpawnParent();

            playerController.ChangeCoinsBy(-purchasedEntry.GetEntryCost());
            confirmationUIParent.SetActive(false);
            UpdateVariables();
        }

        public void SetPickupSpawnPos(Vector3 pos) { pickupSpawnPos = pos; }
    }
}
