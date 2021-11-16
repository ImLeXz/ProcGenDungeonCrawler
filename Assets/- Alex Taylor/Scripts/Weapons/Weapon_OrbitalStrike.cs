using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;

namespace ATDungeon.Weapons
{
    public class Weapon_OrbitalStrike : WeaponBase
    {

        [Header("Orbital Strike Settings")]
        [SerializeField]
        private float lockOnRadius;
        [SerializeField]
        private float explosionRadius;

        protected override void AdditionalShootAction(GameObject projParent)
        {
            Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, lockOnRadius);
            foreach (Collider hitCol in hitColliders)
            {
                bool flag1 = !isEnemyFiringThis && hitCol.gameObject.layer == LayerMask.NameToLayer("HitBoxEnemy");
                bool flag2 = isEnemyFiringThis && hitCol.gameObject.layer == LayerMask.NameToLayer("HitBoxPlayer");

                if (flag1 || flag2)
                {
                    Vector3 circleSpawn = new Vector3(hitCol.gameObject.transform.position.x, UtilFunctions.instance.GetFloorPos().y, hitCol.gameObject.transform.position.z);
                    FXManager.Instance.PlayEffect(FXManager.EffectType.WarningCircle, hitCol.gameObject.transform.position, null);
                    Debug.Log("Orbital Strike Locking On To: " + hitCol.gameObject.name);
                    projParent.transform.position = hitCol.gameObject.transform.position;
                    break;
                }

            }
        }
        protected override void ResetValues()
        {
        }

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
    }
}