using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.Weapons.Modules;
using ATDungeon.Utility;
using ATDungeon.Player;
using ATDungeon.Weapons;

namespace ATDungeon.UI.Forge
{
    public class ForgeUI : MonoBehaviour
    {
        [System.Serializable]
        public class ForgeModule
        {
            public GameObject modulePrefab;
            [Range(0.0f, 1.0f)] public float moduleFrequencyPercentage;
            public string moduleName;
            public int moduleCost;
        }

        [Header("Misc References")]
        [SerializeField]
        private GameObject UIBlurring;
        [SerializeField]
        private TextMeshProUGUI currentCoinTxt;

        [Header("Module Selection References")]
        [SerializeField]
        private GameObject moduleSelectionUIParent;
        [SerializeField]
        private Image selectedModuleImage;
        [SerializeField]
        private GameObject moduleObjectTemplate;
        [SerializeField]
        private Transform moduleObjectParent;
        [SerializeField]
        private ToggleGroup moduleToggleGroup;
        [SerializeField]
        private ForgeModule selectedModule;

        [Header("Weapon Selection References")]
        [SerializeField]
        private GameObject weaponSelectionUIParent;
        [SerializeField]
        private Image selectedWeaponImage;
        [SerializeField]
        private GameObject weaponObjectTemplate;
        [SerializeField]
        private Transform weaponObjectParent;
        [SerializeField]
        private ToggleGroup weaponToggleGroup;
        [SerializeField]
        private WeaponBase selectedWeapon;

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

        [Header("Forge Modules")]
        [SerializeField]
        private ForgeModule[] forgeModules;

        PlayerController playerController;
        private GameObject[] playerWeapons;
        int currentCoins = 0;
        bool firstOpen = true;

        bool isOpen;
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        public void OpenForge()
        {
            this.gameObject.SetActive(true);
            if (firstOpen)
            {
                playerController = UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>();
                InitialiseModuleSelection();
                firstOpen = false;
            }
            playerController.TogglePlayerAiming(false);
            UpdateVariables();
            GenerateWeaponSelection();
            UIBlurring.SetActive(true);
            UIManager.Instance.ClosePrompt();
            isOpen = true;
        }

        public void CloseForge()
        {
            if (this.gameObject.activeInHierarchy)
            {
                isOpen = false;
                playerController.TogglePlayerAiming(true);
                UIBlurring.SetActive(false);
                confirmationUIParent.SetActive(false);
                errorUIParent.SetActive(false);
                moduleSelectionUIParent.SetActive(false);
                weaponSelectionUIParent.SetActive(false);
                this.gameObject.SetActive(false);
            }
        }

        private void UpdateVariables()
        {
            currentCoins = playerController.GetCurrentCoins();
            playerWeapons = playerController.GetCurrentWeapons();
            currentCoinTxt.text = "Current Coins: " + currentCoins.ToString();
        }

        private void InitialiseModuleSelection()
        {
            int moduleCount = 0;
            for (int i = 0; i < forgeModules.Length; i++)
            {
                float freqPercent = forgeModules[i].moduleFrequencyPercentage;
                float[] weight = new float[] { 1.0f - freqPercent, freqPercent };
                int shouldAddEntry = UtilFunctions.instance.GetWeightedRandomValue(weight, 0);

                if (shouldAddEntry == 1)
                {
                    GameObject instantiatedEntryUI = Instantiate(moduleObjectTemplate, moduleObjectParent);
                    ForgeModuleObjectHelper forgeModuleObjectHelper = instantiatedEntryUI.GetComponent<ForgeModuleObjectHelper>();
                    ForgeModuleObjectHelper.ModuleObject moduleObject = forgeModuleObjectHelper.GetModuleObject();
                    moduleObject.forgeModule = forgeModules[i];
                    moduleObject.nameTxt.text = moduleObject.forgeModule.moduleName;
                    moduleObject.costTxt.text = moduleObject.forgeModule.moduleCost.ToString();

                    moduleObject.selectToggle.group = moduleToggleGroup;
                    moduleObject.selectToggle.onValueChanged.AddListener(delegate { ToggleModuleSelection(moduleObject.selectToggle.isOn, moduleObject.forgeModule); });
                    moduleObject.moduleImage.sprite = moduleObject.forgeModule.modulePrefab.GetComponent<ModuleBase>().GetModuleImage();

                    if (moduleCount == 0)
                    {
                        moduleObject.selectToggle.isOn = true;
                        selectedModule = moduleObject.forgeModule;
                    }
                    else
                        moduleObject.selectToggle.isOn = false;

                    moduleCount++;
                }
            }
        }

        private void GenerateWeaponSelection()
        {
            int weaponCount = 0;
            foreach (Transform child in weaponObjectParent.transform)
                Destroy(child.gameObject);
            for (int i = 0; i < playerWeapons.Length; i++)
            {
                if (playerWeapons[i] != null)
                {
                    GameObject instantiatedEntryUI = Instantiate(weaponObjectTemplate, weaponObjectParent);
                    ForgeWeaponObjectHelper forgeWeaponObjectHelper = instantiatedEntryUI.GetComponent<ForgeWeaponObjectHelper>();
                    ForgeWeaponObjectHelper.WeaponObject weaponObject = forgeWeaponObjectHelper.GetWeaponObject();
                    weaponObject.weaponPrefab = playerWeapons[i];

                    weaponObject.selectToggle.group = weaponToggleGroup;
                    weaponObject.selectToggle.onValueChanged.AddListener(delegate { ToggleWeaponSelection(weaponObject.selectToggle.isOn, weaponObject.weaponPrefab.GetComponent<WeaponBase>()); });
                    weaponObject.weaponImage.sprite = weaponObject.weaponPrefab.GetComponent<WeaponBase>().GetWeaponImage();

                    if (weaponCount == 0)
                    {
                        weaponObject.selectToggle.isOn = true;
                        selectedWeapon = weaponObject.weaponPrefab.GetComponent<WeaponBase>();
                    }
                    else
                        weaponObject.selectToggle.isOn = false;

                    weaponCount++;
                }
            }
        }

        private void DisplayError(string errorMsg)
        {
            errorUIParent.SetActive(true);
            errorTxt.text = errorMsg;
        }

        private void DisplayConfirmation(string moduleName, string weaponName, int cost)
        {
            confirmationUIParent.SetActive(true);
            confirmTxt.text = "Add: [" + moduleName + "] To: [" + weaponName +  "] For: [" + cost + "] ?";
        }

        public void ToggleModuleSelection(bool b, ForgeModule selected)
        {
            if (b)
                selectedModule = selected;
        }

        public void ToggleWeaponSelection(bool b, WeaponBase selected)
        {
            if (b)
                selectedWeapon = selected;
        }

        public void ConfirmModuleSelection()
        {
            selectedModuleImage.sprite = selectedModule.modulePrefab.GetComponent<ModuleBase>().GetModuleImage();
            moduleSelectionUIParent.SetActive(false);
        }

        public void ConfirmWeaponSelection()
        {
            selectedWeaponImage.sprite = selectedWeapon.GetWeaponImage();
            weaponSelectionUIParent.SetActive(false);
        }

        public void PurchaseButtonPressed()
        {

            if (currentCoins - selectedModule.moduleCost < 0)
                DisplayError("Not enough coins to purchase this module!");

            else if (!selectedWeapon.CanAddModule())
                DisplayError("This Weapon Already Has Max Modules, Or Does Not Support Them!");

            else
                DisplayConfirmation(selectedModule.moduleName, selectedWeapon.GetWeaponName(), selectedModule.moduleCost);
        }

        public void ConfirmPurchase()
        {
            GameObject weapon = selectedWeapon.gameObject;
            GameObject modulePrefab = Instantiate(selectedModule.modulePrefab);
            ModuleBase ogModule = modulePrefab.GetComponent<ModuleBase>();
            UtilFunctions.instance.CopyComponent(ogModule, weapon);
            Destroy(modulePrefab);

            ModuleBase newModule = weapon.GetComponent<ModuleBase>();
            newModule.SetAttachedWeapon(selectedWeapon);
            newModule.InitialiseModule();

            playerController.ChangeCoinsBy(-selectedModule.moduleCost);
            confirmationUIParent.SetActive(false);
            UpdateVariables();
        }
    }
}
