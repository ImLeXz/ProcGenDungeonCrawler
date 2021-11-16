using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Weapons
{
    public class WeaponProjectile : MonoBehaviour
    {
        [SerializeField]
        private float floatTime = 0.0f;

        private WeaponBase weapon;

        private Rigidbody rb;
        private WeaponBase.DamageType damageType;
        private bool ignoreShield;
        private float damage;
        private float aliveTime;
        private float lifeTime;
        private Collider col;
        private CollisionCallBack colCallback;
        private GameObject parentObject;
        private Vector3 initialPosition;
        private Vector3 initialRotation;
        bool bStartLife;

        [Header("Debug Stuff")]
        [SerializeField]
        private bool firedByEnemy;

        public bool FiredByEnemy { get => firedByEnemy; set => firedByEnemy = value; }

        private void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
            col = this.GetComponent<Collider>();
            initialRotation = this.transform.localEulerAngles;
            initialPosition = this.transform.localPosition;
        }

        // Start is called before the first frame update
        void Start()
        {
            damageType = weapon.GetDamageType();
            damage = weapon.GetDamage();
            ignoreShield = weapon.GetIgnoreShield();
        }

        private void OnEnable()
        {
            aliveTime = 0.0f;
            bStartLife = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (bStartLife)
            {
                aliveTime += Time.deltaTime;
                if (aliveTime > lifeTime)
                    parentObject.SetActive(false);
                else if(aliveTime > floatTime && !rb.useGravity)
                    rb.useGravity = true;
            }
        }

        public delegate void CollisionCallBack(Vector3 collisionPoint, WeaponProjectile weaponProjectile);

        private void OnDisable()
        {
            DeactiveProjectile();
        }

        private void DeactiveProjectile()
        {
            bStartLife = false;
            if (floatTime > 0.0f && rb != null)
                rb.useGravity = false;
        }

        private void CollisionAction(CollisionCallBack callback, Vector3 collisionPoint)
        {
            callback(collisionPoint, this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            CollisionAction(colCallback, collision.GetContact(0).point);
        }

        public void SetParent(GameObject parent) { parentObject = parent; }
        public void SetCollisionCallback(CollisionCallBack cb) { colCallback = cb; }
        public void SetWeapon(WeaponBase wb) { weapon = wb; }
        public void SetLifetime(float t) { lifeTime = t; }
        public float GetDamage() { return damage; }
        public bool GetIgnoreShield() { return ignoreShield; }
        public Collider GetCollider() { return col; }
        public Vector3 GetInitialPosition() { return initialPosition; }
        public Vector3 GetInitialRotation() { return initialRotation; }
    }
}
