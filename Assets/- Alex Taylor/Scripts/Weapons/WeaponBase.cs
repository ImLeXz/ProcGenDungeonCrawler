using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;
using ATDungeon.Weapons.Modules;

namespace ATDungeon.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public enum DamageType { Default, Fire, Poison, Ice, Piercing, AoE, Explosive }
        public enum FiringType { SingleShot, FullAuto }

        [Header("Damage Settings")]
        [SerializeField]
        private float damage = 10.0f;
        [SerializeField]
        private DamageType damageType = DamageType.Default;

        [Header("Projectile Firing Settings")]
        [SerializeField]
        private GameObject projectile;
        [SerializeField]
        private Vector2 projectileInaccuracyHV;
        [SerializeField]
        private float projectileLifetime = 1.5f;
        [SerializeField]
        private float projectileForce = 5.0f;
        [SerializeField]
        private FiringType firingType = FiringType.SingleShot;

        [Header("Ammo Settings")]
        [SerializeField]
        private int magSize = 20;
        [SerializeField]
        private int maxReserveAmmo = 100;
        [SerializeField]
        private float reloadTime = 1.25f;
        [SerializeField]
        private float delayBetweenShots = 0.15f;
        [SerializeField]
        private bool bInfiniteAmmo;

        [Header("Module Settings")]
        [SerializeField]
        private int maxEquipableModules = 2;
        [SerializeField]
        private int currentEquipedModules = 0;

        [Header("Miscellaneous")]
        [SerializeField]
        private string weaponName = "";
        [SerializeField]
        private bool bCanDiscardWeapon = true;
        [SerializeField]
        private int weaponPurchaseValue = 0;
        [SerializeField]
        private int weaponDiscardValue = 0;
        [SerializeField]
        private Sprite weaponImage;

        private WeaponHandler weaponHandler;
        private int currentAmmoInMag;
        private int currentAmmoInReserve;
        private float timeSinceLastShot;
        private bool isReloading = false;
        private Transform projSpawnParent;
        private int instanceNum = 0;
        private ObjectPooler.Pool pool = null;
        private bool ignoreShield = false;
        protected bool isEnemyFiringThis;

        WeaponHandler.ReloadCompletedCallBack ReloadCallBack;

        //MultipliersForModules
        private float damageMultiplier = 1.0f;
        private float magSizeMultiplier = 1.0f;
        private float reloadTimeMultiplier = 1.0f;
        private float fireRateMultiplier = 1.0f;
        private float projectileForceMultiplier = 1.0f;

        //Original Values
        private float ogDamage;
        private int ogMagSize;
        private float ogReloadTime;
        private float ogDelayBetweenShots;
        private float ogProjectileForce;


        private void Awake()
        {
            currentAmmoInMag = magSize;
            currentAmmoInReserve = maxReserveAmmo;
        }

        private void Start()
        {
        }

        public void Initialise()
        {
            //This sets the size of the pool to be the very maximum amount of bullets that can be fired at once, with two added for buffer
            int poolSize = Mathf.RoundToInt((projectileLifetime / delayBetweenShots) + 2);
            GameObject poolParent = new GameObject();
            poolParent.transform.parent = ObjectPooler.Instance.gameObject.transform;
            pool = new ObjectPooler.Pool
            {
                tag = projectile.name,
                prefab = projectile,
                spawnParent = poolParent.transform,
                size = poolSize,
            };
            ObjectPooler.Instance.AddPool(pool);
            projSpawnParent = poolParent.transform;
            instanceNum = ObjectPooler.Instance.ReturnInstanceNum(projectile.name);
            foreach(Transform tf in projSpawnParent.transform)
            {
                if (isEnemyFiringThis)
                    tf.gameObject.layer = LayerMask.NameToLayer("ProjectileEnemy");
                else
                    tf.gameObject.layer = LayerMask.NameToLayer("ProjectilePlayer");
            }

            timeSinceLastShot = delayBetweenShots;

            ogDamage = damage;
            ogMagSize = magSize;
            ogReloadTime = reloadTime;
            ogDelayBetweenShots = delayBetweenShots;
            ogProjectileForce = projectileForce;
        }

        private void Update()
        {
            timeSinceLastShot += Time.deltaTime;
        }

        public void DeletePool()
        {
            ObjectPooler.Instance.RemovePool(pool);
        }

        private void CollisionCallback(Vector3 colPoint, WeaponProjectile weaponProjectile)
        {
            //Callback not overriden, but action function is
            ToggleProjectileHitBox(weaponProjectile, false);
            DoCollisionAction(colPoint, weaponProjectile);
        }

        protected virtual void AdditionalShootAction(GameObject projParent) { }
        protected abstract void DoCollisionAction(Vector3 colPoint, WeaponProjectile weaponProjectile);
        protected abstract void ResetValues();

        private void ToggleProjectileHitBox(WeaponProjectile proj, bool state)
        {
            proj.GetCollider().enabled = state;
        }

        public void Shoot()
        {
            //Make sure time since last shot is greater than distance between shots
            //Spawn Bullet Prefab, fire it in direction player is facing
            //Bullet takes damage value and damage type as input
            if(timeSinceLastShot > delayBetweenShots && currentAmmoInMag > 0)
            {
                ResetValues();
                timeSinceLastShot = 0.0f;
                GameObject proj = ObjectPooler.Instance.SpawnFromPool(projectile.name + "_" + instanceNum, projSpawnParent, 0.0f);
                WeaponProjectile[] wpArray = proj.GetComponentsInChildren<WeaponProjectile>(true);
                foreach (WeaponProjectile wp in wpArray)
                {
                    wp.FiredByEnemy = isEnemyFiringThis;
                    wp.gameObject.SetActive(true);
                    wp.SetParent(proj);
                    wp.SetCollisionCallback(CollisionCallback);
                    wp.SetWeapon(this);
                    wp.SetLifetime(projectileLifetime);
                    ToggleProjectileHitBox(wp, true);

                    Rigidbody _rb = wp.gameObject.GetComponent<Rigidbody>();
                    _rb.velocity = Vector3.zero;

                    wp.gameObject.transform.localPosition = wp.GetInitialPosition();
                    wp.gameObject.transform.localEulerAngles = wp.GetInitialRotation();

                    proj.gameObject.transform.position = weaponHandler.GetShootPos();
                    proj.gameObject.transform.eulerAngles = weaponHandler.GetShootAng();

                    Vector3 weaponShootDir = weaponHandler.GetShootDir();
                    float inaccuracyH = UnityEngine.Random.Range(-projectileInaccuracyHV.x, projectileInaccuracyHV.x);
                    float inaccuracyV = UnityEngine.Random.Range(-projectileInaccuracyHV.y, projectileInaccuracyHV.y);
                    Vector3 weaponInaccurateShootDir = new Vector3(weaponShootDir.x + inaccuracyH, weaponShootDir.y + inaccuracyV, weaponShootDir.z);
                    _rb.AddForce(weaponInaccurateShootDir * projectileForce, ForceMode.Impulse);
                }

                AdditionalShootAction(proj);

                int newAmmo = currentAmmoInMag - 1;

                if(newAmmo <= 0)
                    currentAmmoInMag = 0;
                else
                    currentAmmoInMag = newAmmo;
            }
        }

        public IEnumerator ReloadAction()
        {
            isReloading = true;
            yield return new WaitForSeconds(reloadTime);
            int newAmmo = currentAmmoInReserve - (magSize - currentAmmoInMag);

            if (newAmmo < 0)
            {
                currentAmmoInReserve = 0;
                currentAmmoInMag = magSize + newAmmo;
            }

            else
            {
                currentAmmoInReserve = newAmmo;
                currentAmmoInMag = magSize;
            }

            if (bInfiniteAmmo && currentAmmoInReserve == 0)
                currentAmmoInReserve = maxReserveAmmo;
            isReloading = false;
            ReloadCallBack?.Invoke(weaponHandler.GetWeaponNum(this.gameObject), weaponImage, currentAmmoInMag, currentAmmoInReserve);
        }

        public void UpdateWeaponValues()
        {
            damage = ogDamage * damageMultiplier;
            magSize = Mathf.RoundToInt(ogMagSize * magSizeMultiplier);
            reloadTime = ogReloadTime * reloadTimeMultiplier;
            delayBetweenShots = delayBetweenShots * fireRateMultiplier;
            projectileForce = ogProjectileForce * projectileForceMultiplier;
        }

        public float GetDamage() { return damage; }
        public float GetDamageMultiplier() { return damageMultiplier; }
        public float GetReloadTime() { return reloadTime; }
        public bool GetIsReloading() { return isReloading; }
        public int GetMagAmmo() { return currentAmmoInMag; }
        public int GetReserveAmmo() { return currentAmmoInReserve; }
        public bool GetIgnoreShield() { return ignoreShield; }
        public DamageType GetDamageType() { return damageType; }
        public FiringType GetFiringType() { return firingType; }
        public void SetWeaponHandler(WeaponHandler wh) { weaponHandler = wh; }
        public Sprite GetWeaponImage() { return weaponImage; }
        public string GetWeaponName() { return weaponName; }
        public int GetWeaponPurchaseValue() { return weaponPurchaseValue; }
        public int GetWeaponDiscardValue() { return weaponDiscardValue; }
        public bool CanDiscardWeapon() { return bCanDiscardWeapon; }
        public void ChangeModuleCountBy(int n) { currentEquipedModules += n; }
        public bool CanAddModule() { return currentEquipedModules + 1 <= maxEquipableModules; }
        public void SetIsEnemyFiringThis(bool b) { isEnemyFiringThis = b; }
        public void SetInfiniteAmmo(bool b) { bInfiniteAmmo = b; }

        public void DoCallback() { ReloadCallBack?.Invoke(weaponHandler.GetWeaponNum(this.gameObject), weaponImage, currentAmmoInMag, currentAmmoInReserve); }
        public void SetCallback(WeaponHandler.ReloadCompletedCallBack cb) { ReloadCallBack = cb; }

        //Multipliers
        public void ModifyDamageMultiplierBy(float n) { damageMultiplier += n; UpdateWeaponValues(); }
        public void ModifyMagSizeMultiplierBy(float n) { magSizeMultiplier += n; UpdateWeaponValues(); }
        public void ModifyReloadTimeMultiplierBy(float n) { reloadTimeMultiplier += n; UpdateWeaponValues(); }
        public void ModifyFireRateMultiplierBy(float n) { fireRateMultiplier += n; UpdateWeaponValues(); }
        public void ModifyProjectileForceMultiplierBy(float n) { projectileForceMultiplier += n; UpdateWeaponValues(); }
    }
}
