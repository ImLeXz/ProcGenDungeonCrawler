using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Weapons;
using System;

namespace ATDungeon.Handlers
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField]
        private bool isEnemy;
        [SerializeField]
        private bool bInfiniteAmmo;
        [SerializeField]
        private float overallDamageMultiplier = 1.0f;
        [SerializeField]
        List<GameObject> weaponsList = new List<GameObject>();
        [SerializeField]
        private GameObject shootPos;
        [SerializeField]
        private Sprite emptyWeaponIcon;

        private ReloadCompletedCallBack ReloadCallback;

        private GameObject currentWeapon = null;
        private WeaponBase currentWB = null;
        private int currentWeaponCount, maxWeaponCount;

        private void Awake()
        {
            List<GameObject> instantiatedObjects = new List<GameObject>();
            for (int i = 0; i < weaponsList.Count; i++)
            {
                if (weaponsList[i] != null)
                {
                    GameObject weaponObj = Instantiate(weaponsList[i], this.transform);
                    WeaponBase wb = weaponObj.GetComponent<WeaponBase>();
                    wb.SetWeaponHandler(this);
                    wb.SetIsEnemyFiringThis(isEnemy);
                    wb.SetInfiniteAmmo(bInfiniteAmmo);
                    wb.SetCallback(ReloadCallback);
                    wb.Initialise();
                    instantiatedObjects.Add(weaponObj);
                    currentWeaponCount++;
                }
                else
                {
                    instantiatedObjects.Add(null);
                }
            }

            weaponsList = instantiatedObjects;
            currentWeapon = weaponsList[0];
            maxWeaponCount = weaponsList.Count;
            if (currentWeapon)
            {
                currentWB = currentWeapon.GetComponent<WeaponBase>();
                if (currentWB)
                    currentWB.SetWeaponHandler(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public delegate void ReloadCompletedCallBack(int weaponNum, Sprite weaponIcon, int magAmmo, int reserveAmmo);

        public void Shoot(bool hasReleasedTrigger)
        {
            WeaponBase.FiringType firingType = currentWB.GetFiringType();
            if (!hasReleasedTrigger && firingType == WeaponBase.FiringType.FullAuto)
            {
                currentWB.Shoot();
            }
            else if (hasReleasedTrigger)
            {
                currentWB.Shoot();
            }

            if (GetCurrentMagAmmo() <= 0)
                Reload();
        }

        public void Reload()
        {
            if (currentWB.GetReserveAmmo() > 0 && !currentWB.GetIsReloading())
            {
                currentWB.StartCoroutine(currentWB.ReloadAction());
            }
        }

        public void CleanUpWeaponPools()
        {
            foreach (GameObject weapon in weaponsList)
            {
                WeaponBase wb = weapon.GetComponent<WeaponBase>();
                if(wb != null)
                    wb.DeletePool();
            }
        }

        public void ReplaceCurrentWeapon(GameObject weapon)
        {
            Destroy(currentWeapon.gameObject);
            currentWeapon = Instantiate(weapon, this.transform);
            currentWB = currentWeapon.GetComponent<WeaponBase>();
            if (currentWB)
                currentWB.SetWeaponHandler(this);
        }

        public bool AddWeapon(GameObject weapon)
        {
            Debug.Log("Adding Weapon: " + weapon.name);
            bool weaponEquiped = false;
            for (int i = 0; i < maxWeaponCount; i++)
            {
                if (weaponsList[i] == null)
                {
                    weaponsList[i] = Instantiate(weapon, this.transform);
                    if (!Utility.UtilFunctions.instance.CheckIfPrefab(weapon))
                        Destroy(weapon);
                    WeaponBase wb = weaponsList[i].GetComponent<WeaponBase>();
                    wb.SetWeaponHandler(this);
                    wb.SetCallback(ReloadCallback);
                    wb.Initialise();
                    float newDamageMult = (wb.GetDamageMultiplier() * overallDamageMultiplier) - wb.GetDamageMultiplier();
                    Debug.Log(wb.GetDamageMultiplier() + " x " + overallDamageMultiplier + " - " + wb.GetDamageMultiplier());
                    wb.ModifyDamageMultiplierBy(newDamageMult);
                    Debug.Log("New Damage After Damage Multiplier: [" + newDamageMult + "]");
                    currentWeaponCount++;
                    SwitchWeapon(i);
                    weaponEquiped = true;
                    break;
                }
            }
            return weaponEquiped;
        }

        private void SwitchWeapon(int i)
        {
            if (weaponsList[i] != null)
            {
                Debug.Log("Setting Current Weapon To: " + weaponsList[i]);
                currentWeapon = weaponsList[i];
                currentWB = currentWeapon.GetComponent<WeaponBase>();
                ReloadCallback(GetCurrentWeaponNum(), currentWB.GetWeaponImage(), currentWB.GetMagAmmo(), currentWB.GetReserveAmmo());
            }
        }

        public void SwitchWeapon(bool next) // true if next weapon, false if previous weapon
        {
            //Top Weapon = index 2, CurrentWeapons = 3, Next Index = CurrentWeapon Count but exceeds index, if next weapon is out of list scope set back to first index

            int weaponNum = 0;

            if (next) //Scroll Up
            {
                weaponNum = GetCurrentWeaponNum() + 1;
                if (weaponNum >= currentWeaponCount)
                    weaponNum = 0;
                while (weaponsList[weaponNum] == null)
                    weaponNum++;
            }

            else //Scroll Down
            {
                weaponNum = GetCurrentWeaponNum() - 1;
                if (weaponNum < 0)
                    weaponNum = weaponsList.Count - 1;
                while (weaponsList[weaponNum] == null)
                    weaponNum--;
            }

            if (weaponsList[weaponNum] != null)
            {
                currentWeapon = weaponsList[weaponNum];
                currentWB = currentWeapon.GetComponent<WeaponBase>();
                ReloadCallback(GetCurrentWeaponNum(), currentWB.GetWeaponImage(), currentWB.GetMagAmmo(), currentWB.GetReserveAmmo());
            }
        }

        public void RemoveWeapon(GameObject weapon)
        {
            int weaponNum = weaponsList.FindIndex(x => x == weapon);
            if (weapon == currentWeapon)
                SwitchWeapon(false);
            ReloadCallback(weaponNum, emptyWeaponIcon, 0, 0);
            weaponsList[weaponNum] = null;
            currentWeaponCount--;
            weapon.GetComponent<WeaponBase>().DeletePool();
            Destroy(weapon);
        }

        public void SetReloadCallback(ReloadCompletedCallBack cb) { ReloadCallback = cb; }

        public Vector3 GetShootAng() { return shootPos.transform.eulerAngles; }
        public Vector3 GetShootDir() { return shootPos.transform.forward; }
        public Vector3 GetShootPos() { return shootPos.transform.position; }
        public int GetCurrentMagAmmo() { return currentWB.GetMagAmmo(); }
        public int GetCurrentReserveAmmo() { return currentWB.GetReserveAmmo(); }
        public int GetWeaponNum(GameObject w) { return weaponsList.FindIndex(x => x == w); }
        public int GetCurrentWeaponNum() { return weaponsList.FindIndex(x => x == currentWeapon); }
        public WeaponBase GetCurrentWeapon() { return currentWB; }
        public GameObject[] GetWeapons() { return weaponsList.ToArray(); }
        //public void SetOverallDamageMultipler(float n) { overallDamageMultiplier = n; }
        public void ChangeOverallDamageMultiplierBy(float n) { overallDamageMultiplier += n; }
    }
}
