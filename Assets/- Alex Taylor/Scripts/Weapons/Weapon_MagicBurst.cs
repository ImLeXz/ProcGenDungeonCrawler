using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;

namespace ATDungeon.Weapons
{
    public class Weapon_MagicBurst : WeaponBase
    {
        protected override void ResetValues()
        {
        }
        protected override void DoCollisionAction(Vector3 colPoint, WeaponProjectile weaponProjectile)
        {
            FXManager.Instance.PlayEffect(FXManager.EffectType.Explosion, colPoint, weaponProjectile.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            weaponProjectile.gameObject.SetActive(false);
        }
    }
}