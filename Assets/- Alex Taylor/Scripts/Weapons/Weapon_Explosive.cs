using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;

namespace ATDungeon.Weapons
{
    public class Weapon_Explosive : WeaponBase
    {

        [Header("Explosive Weapon Settings")]
        [SerializeField]
        private float explosionRadius;

        protected override void DoCollisionAction(Vector3 colPoint, WeaponProjectile weaponProjectile)
        {
            FXManager.Instance.PlayEffect(FXManager.EffectType.Explosion, colPoint, weaponProjectile.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            weaponProjectile.gameObject.SetActive(false);
            Collider[] hitColliders = Physics.OverlapSphere(colPoint, explosionRadius);
            foreach (Collider hitCol in hitColliders)
            {
                bool flag1 = !weaponProjectile.FiredByEnemy && hitCol.gameObject.layer == LayerMask.NameToLayer("HitBoxEnemy");
                bool flag2 = weaponProjectile.FiredByEnemy && hitCol.gameObject.layer == LayerMask.NameToLayer("HitBoxPlayer");

                if (flag1 || flag2)
                {
                    //Debug.Log("Flag1 was: " + flag1 + " - Flag2 was: " + flag2);
                    HitBoxHandler hitBox = hitCol.gameObject.GetComponent<HitBoxHandler>();
                    hitBox.GenerateHit(this.GetDamage(), false);
                }

            }
        }

        protected override void ResetValues()
        {
        }
    }
}