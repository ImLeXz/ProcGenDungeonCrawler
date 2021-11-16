using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Weapons.Modules
{
    public class ModuleBase : MonoBehaviour
    {
        [SerializeField]
        private Sprite moduleImage;
        [SerializeField]
        private WeaponBase attachedWeapon;

        [Header("Multiplier Changes")]
        [SerializeField]
        private float damageMultiplierChange;
        [SerializeField]
        private float magSizeMultiplierChange;
        [SerializeField]
        private float reloadTimeMultiplierChange;
        [SerializeField]
        private float fireRateMultiplierChange;
        [SerializeField]
        private float projectileForceMultiplierChange;

        // Start is called before the first frame update
        private void Start()
        {
        }

        public void InitialiseModule()
        {
            AdjustMultipliers();
        }

        private void AdjustMultipliers()
        {
            attachedWeapon.ModifyDamageMultiplierBy(damageMultiplierChange);
            attachedWeapon.ModifyMagSizeMultiplierBy(magSizeMultiplierChange);
            attachedWeapon.ModifyReloadTimeMultiplierBy(reloadTimeMultiplierChange);
            attachedWeapon.ModifyFireRateMultiplierBy(fireRateMultiplierChange);
            attachedWeapon.ModifyProjectileForceMultiplierBy(projectileForceMultiplierChange);
            attachedWeapon.DoCallback();
            attachedWeapon.UpdateWeaponValues();
        }

        private void RemoveModule()
        {
            //Does The Opposite To Reset The Effects Of The Module
            attachedWeapon.ModifyDamageMultiplierBy(-damageMultiplierChange);
            attachedWeapon.ModifyMagSizeMultiplierBy(-magSizeMultiplierChange);
            attachedWeapon.ModifyReloadTimeMultiplierBy(-reloadTimeMultiplierChange);
            attachedWeapon.ModifyFireRateMultiplierBy(-fireRateMultiplierChange);
            attachedWeapon.ModifyProjectileForceMultiplierBy(-projectileForceMultiplierChange);
        }

        public Sprite GetModuleImage() { return moduleImage; }
        public void SetAttachedWeapon(WeaponBase wb) { attachedWeapon = wb; }

    }
}