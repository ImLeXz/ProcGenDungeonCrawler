using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.Player;
using ATDungeon.Utility;
using ATDungeon.Weapons;
using ATDungeon.Items;

namespace ATDungeon.UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Misc References")]
        [SerializeField]
        private GameObject UIBlurring;
        [SerializeField]
        private TextMeshProUGUI currentCoinTxt;

        [Header("Invetory References")]
        [SerializeField]
        private GameObject inventoryObjectTemplate;
        [SerializeField]
        private Transform weaponObjectParent;
        [SerializeField]
        private Transform itemObjectParent;

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

        private List<ItemBase> selectedItems = new List<ItemBase>();
        private List<WeaponBase> selectedWeapons = new List<WeaponBase>();
        private PlayerController playerController = null;
        private GameObject[] playerWeapons, lastAttemptedWeapons;
        private GameObject[] playerItems, lastAttemptedItems;
        int currentCoins = 0;

        bool isOpen;
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        public void OpenInventory()
        {
            isOpen = true;
            this.gameObject.SetActive(true);
            if (playerController is null)
                playerController = UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>();

            playerController.TogglePlayerAiming(false);
            UpdateVariables();
            GenerateItemSelection();
            GenerateWeaponSelection();
            UIBlurring.SetActive(true);
            UIManager.Instance.ClosePrompt();
        }

        public void CloseInventory()
        {
            isOpen = false;
            playerController.TogglePlayerAiming(true);
            selectedItems.Clear();
            selectedWeapons.Clear();
            UIBlurring.SetActive(false);
            confirmationUIParent.SetActive(false);
            errorUIParent.SetActive(false);
            this.gameObject.SetActive(false);
        }

        private void UpdateVariables()
        {
            currentCoins = playerController.GetCurrentCoins();
            playerWeapons = playerController.GetCurrentWeapons();
            playerItems = playerController.GetCurrentItems();
            currentCoinTxt.text = "Current Coins: " + currentCoins.ToString();
        }

        private void GenerateWeaponSelection()
        {
            if (playerWeapons != lastAttemptedWeapons)
            {
                foreach (Transform child in weaponObjectParent.transform)
                    Destroy(child.gameObject);
                for (int i = 0; i < playerWeapons.Length; i++)
                {
                    if (playerWeapons[i] != null && playerWeapons[i].GetComponent<WeaponBase>().CanDiscardWeapon())
                    {
                        GameObject instantiatedInvUI = Instantiate(inventoryObjectTemplate, weaponObjectParent);
                        InventoryObjHelper invObjectHelper = instantiatedInvUI.GetComponent<InventoryObjHelper>();
                        InventoryObjHelper.InventoryObject invObject = invObjectHelper.GetInventoryObject();
                        invObject.objPrefab = playerWeapons[i];
                        WeaponBase weaponBase = invObject.objPrefab.GetComponent<WeaponBase>();

                        if (weaponBase != null)
                        {
                            invObject.nameTxt.text = weaponBase.GetWeaponName();
                            invObject.worthTxt.text = weaponBase.GetWeaponDiscardValue().ToString();
                            invObject.selectToggle.onValueChanged.AddListener(delegate { ToggleWeaponSelection(invObject.selectToggle.isOn, weaponBase); });
                            invObject.objImage.sprite = weaponBase.GetWeaponImage();
                        }
                        else
                        {
                            Debug.LogError("Error Getting WeaponBase Whilst Generating WEAPON Selection For Inventory UI");
                        }
                    }
                }
            }
        }

        private void GenerateItemSelection()
        {
            if (playerItems != lastAttemptedItems)
            {
                foreach (Transform child in itemObjectParent.transform)
                    Destroy(child.gameObject);
                for (int i = 0; i < playerItems.Length; i++)
                {
                    if (playerItems[i] != null)
                    {
                        GameObject instantiatedInvUI = Instantiate(inventoryObjectTemplate, itemObjectParent);
                        InventoryObjHelper invObjectHelper = instantiatedInvUI.GetComponent<InventoryObjHelper>();
                        InventoryObjHelper.InventoryObject invObject = invObjectHelper.GetInventoryObject();
                        invObject.objPrefab = playerItems[i];
                        ItemBase itemBase = invObject.objPrefab.GetComponent<ItemBase>();

                        if (itemBase != null)
                        {
                            invObject.nameTxt.text = itemBase.GetItemName();
                            invObject.worthTxt.text = itemBase.GetItemDiscardValue().ToString();
                            invObject.selectToggle.onValueChanged.AddListener(delegate { ToggleItemSelection(invObject.selectToggle.isOn, itemBase); });
                            invObject.objImage.sprite = itemBase.GetItemImage();
                        }
                        else
                        {
                            Debug.LogError("Error Getting ItemBase Whilst Generating ITEM Selection For Inventory UI");
                        }
                    }
                }
            }
        }

        public void ToggleWeaponSelection(bool b, WeaponBase selected)
        {
            if (b)
                selectedWeapons.Add(selected);
            else
                selectedWeapons.Remove(selected);
        }

        public void ToggleItemSelection(bool b, ItemBase selected)
        {
            if (b)
                selectedItems.Add(selected);
            else
                selectedItems.Remove(selected);
        }

        private void DisplayError(string errorMsg)
        {
            errorUIParent.SetActive(true);
            errorTxt.text = errorMsg;
        }

        private void DisplayConfirmation()
        {
            confirmationUIParent.SetActive(true);
            confirmTxt.text = "Are You Sure You Want To Discard [" + selectedWeapons.Count + "] Weapons And  [" + selectedItems.Count + "] Items?";
        }

        public void DiscardButtonPressed()
        {
            if (selectedWeapons.Count == 0 && selectedItems.Count == 0)
                DisplayError("No Items Or Weapons Selected, Please Select At Least One!");
            else
                DisplayConfirmation();
        }

        public void ConfirmDiscard()
        {
            foreach (ItemBase item in selectedItems)
            {
                playerController.ChangeCoinsBy(item.GetItemDiscardValue());
                playerController.RemoveItem(item.gameObject);
            }

            foreach (WeaponBase wpn in selectedWeapons)
            {
                playerController.ChangeCoinsBy(wpn.GetWeaponDiscardValue());
                playerController.RemoveWeapon(wpn.gameObject);
            }

            selectedItems.Clear();
            selectedWeapons.Clear();

            CloseInventory();
            OpenInventory();
        }

    }
}
