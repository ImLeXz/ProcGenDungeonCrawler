using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;

namespace ATDungeon.Items
{
    public class RadioActiveShield_Item : ItemBase
    {
        [Header("Radio Active Shield Settings: ")]
        [SerializeField]
        private float shieldDamage;
        [SerializeField]
        private float radius;

        protected override void Initialise()
        {
        }

        protected override void ItemAction()
        {
            GenerateSheildWave();
        }

        protected override void ItemComplete()
        {
            
        }

        private void GenerateSheildWave()
        {
            Vector3 pos = UtilFunctions.instance.GetPlayerPos();
            FXManager.Instance.PlayEffect(FXManager.EffectType.ShieldPulse, pos, null);
            Collider[] hitColliders = Physics.OverlapSphere(pos, radius);
            foreach (Collider hitCol in hitColliders)
            {
                if (hitCol.gameObject.layer == LayerMask.NameToLayer("HitBoxEnemy"))
                {
                    HitBoxHandler hitBox = hitCol.gameObject.GetComponent<HitBoxHandler>();
                    hitBox.GenerateHit(shieldDamage, false);
                }
            }
        }
    }
}
