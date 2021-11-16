using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Weapons;

namespace ATDungeon.Handlers
{
    public class HitBoxHandler : MonoBehaviour
    {
        [SerializeField]
        private Collider[] colliders;

        private HitBoxCollisionCallBack hitBoxCallback;

        // Start is called before the first frame update
        void Start()
        {
            SetupColliders();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Projectile")
            {
                WeaponProjectile wp = collision.gameObject.GetComponent<WeaponProjectile>();
                HitAction(hitBoxCallback, wp.GetDamage(), wp.GetIgnoreShield());
            }
        }

        public void GenerateHit(float damage, bool ignoreShield)
        {
            Debug.Log("Generating Hit For: [" + damage + "] On: [" + this.gameObject.name + "]");
            HitAction(hitBoxCallback, damage, ignoreShield);
        }

        public delegate void HitBoxCollisionCallBack(float dmg, bool ignoreShield);

        private void HitAction(HitBoxCollisionCallBack callback, float damage, bool ignoreShield)
        {
            callback(damage, ignoreShield);
        }

        private void SetupColliders()
        {
            foreach (Collider c in colliders)
            {
                BoxCollider newCollider = this.gameObject.AddComponent<BoxCollider>();
                newCollider.size = c.bounds.size;
                Destroy(c.gameObject);
            }
        }

        public void SetHitBoxCallBack(HitBoxCollisionCallBack cb) { hitBoxCallback = cb; }
    }
}
