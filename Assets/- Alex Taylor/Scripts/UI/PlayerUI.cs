using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fragsurf.Movement;
using System;

namespace ATDungeon.Player
{
    public class PlayerUI : MonoBehaviour
    {

        [System.Serializable]
        public class ItemObject
        {
            public Image itemImg;
            public TextMeshProUGUI itemQtyTxt;
            public Image itemStateIndicatorImg;
        }

        [System.Serializable]
        public class WeaponObject
        {
            public Image weaponImg;
            public Image weaponActiveImg;
            public TextMeshProUGUI weaponAmmoTxt;
        }

        [Header("Health UI")]
        [SerializeField]
        private TextMeshProUGUI healthTxt;
        [SerializeField]
        private TextMeshProUGUI shieldTxt;
        [SerializeField]
        private Slider healthSlider;
        [SerializeField]
        private Slider shieldSlider;

        [Header("Weapons UI")]
        [SerializeField]
        private WeaponObject[] weaponsUI;
        [SerializeField]
        private Sprite emptyWeaponImg;
        [SerializeField]
        private Color weaponActiveColor;
        [SerializeField]
        private Color weaponNotActiveColor;

        [Header("Keys UI")]
        [SerializeField]
        private Image[] keyIcons;

        [Header("Items UI")]
        [SerializeField]
        private Color itemActiveColor;
        [SerializeField]
        private Color itemCoolDownColor;
        [SerializeField]
        private ItemObject[] itemsUI;

        [Header("Stats UI")]
        [SerializeField]
        private TextMeshProUGUI scoreTxt;
        [SerializeField]
        private TextMeshProUGUI floorsTxt;
        [SerializeField]
        private TextMeshProUGUI timeTxt;
        [SerializeField]
        private TextMeshProUGUI killedTxt;
        [SerializeField]
        private TextMeshProUGUI coinsTxt;

        [Header("Other References")]
        [SerializeField]
        private TextMeshProUGUI velocityTxt;
        [SerializeField]
        private SurfCharacter surfCharacterObj;

        private float timeSurvived = 0.0f;

        // Start is called before the first frame update
        private void Start()
        {
            InvokeRepeating("UpdateTimeUI", 1f, 1f);  //1s delay, repeat every 1s
        }

        private void UpdateTimeUI()
        {
            timeSurvived += 1.0f;
            string formatedTime = FormatTime(timeSurvived);
            timeTxt.text = "Time: " + formatedTime;
        }

        private string FormatTime(float time)
        {
            int hours = (int)time / 3600;
            int minutes = (int)time / 60;
            int seconds = (int)time / 1 - 60 * minutes;
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateVelocityUI();
        }

        public void SetupHealthSliders(float maxHealth, float maxShield)
        {
            healthSlider.maxValue = maxHealth;
            shieldSlider.maxValue = maxShield;
        }

        private void UpdateVelocityUI()
        {
            float velVal = surfCharacterObj.moveData.velocity.magnitude * 10;
            velocityTxt.text = ("Current Velocity: " + (float)Math.Round(velVal, 2));
        }

        public void UpdateScoreUI(int n)
        {
            if (n >= 999999)
                n = 999999;
            string formated = string.Format("{0:000000}", n);
            scoreTxt.text = "Current Score: " + formated;
        }

        public void UpdateKilledUI(int n)
        {
            if (n >= 9999)
                n = 9999;
            string formated = string.Format("{0:0000}", n);
            killedTxt.text = "Killed: " + formated;
        }

        public void UpdateFloorsUI(int n)
        {
            if (n >= 99)
                n = 99;
            string formated = string.Format("{0:00}", n);
            floorsTxt.text = ("Floors: " + formated);
        }

        public void UpdateHealthUI(float v)
        {
            healthTxt.text = ("Health: " + (float)Math.Round(v, 2));
            healthSlider.value = v;
        }

        public void UpdateShieldUI(float v)
        {
            shieldTxt.text = ("Shield: " + (float)Math.Round(v, 2));
            shieldSlider.value = v;
        }

        public void UpdateCoinsUI(int n)
        {
            if (n >= 9999)
                n = 9999;
            string formated = string.Format("{0:0000}", n);
            coinsTxt.text = ("Coins: " + formated);
        }

        public void UpdateAmmoUI(int weaponNum, int currentWeaponNum, Sprite weaponIcon, int magAmmo, int reserveAmmo)
        {
            weaponsUI[weaponNum].weaponImg.sprite = weaponIcon;
            weaponsUI[weaponNum].weaponAmmoTxt.text = magAmmo + " / " + reserveAmmo;
            weaponsUI[currentWeaponNum].weaponActiveImg.color = weaponActiveColor;

            foreach (WeaponObject wObj in weaponsUI)
            {
                if (wObj != weaponsUI[currentWeaponNum])
                    wObj.weaponActiveImg.color = weaponNotActiveColor;
                if (wObj.weaponImg.sprite == null)
                    wObj.weaponImg.sprite = emptyWeaponImg;
            }
        }

        public void UpdateKeyUI(int keyNum, bool hasKey)
        {
            if (hasKey)
                keyIcons[keyNum].color = Color.green;
            else
                keyIcons[keyNum].color = Color.red;
        }

        public void UpdateItemUI(int itemNum, Sprite itemIcon, int itemUses, Items.ItemBase.ItemState itemState)
        {
            if(itemIcon != null)
                itemsUI[itemNum].itemImg.sprite = itemIcon;

            switch(itemState)
            {
                case Items.ItemBase.ItemState.Idle:
                    itemsUI[itemNum].itemStateIndicatorImg.gameObject.SetActive(false);
                    break;
                case Items.ItemBase.ItemState.Active:
                    itemsUI[itemNum].itemStateIndicatorImg.gameObject.SetActive(true);
                    itemsUI[itemNum].itemStateIndicatorImg.color = itemActiveColor;
                    break;
                case Items.ItemBase.ItemState.Cooldown:
                    itemsUI[itemNum].itemStateIndicatorImg.gameObject.SetActive(true);
                    itemsUI[itemNum].itemStateIndicatorImg.color = itemCoolDownColor;
                    break;
            }

            itemsUI[itemNum].itemQtyTxt.text = itemUses.ToString();
        }

        public string GetFormatedTime() { return FormatTime(timeSurvived); }
        public float GetTimeSurvived() { return timeSurvived; }
        public void SetTimeSurvived(float v) { timeSurvived = v; }
    }
}
