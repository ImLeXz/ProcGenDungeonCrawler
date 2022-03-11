using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ATDungeon.Handlers;
using ATDungeon.Utility;
using ATDungeon.Dungeon;
using ATDungeon.UI;
using UnityEngine.Animations;

namespace ATDungeon.Enemies
{
    public class EnemyBase : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField]
        private bool bIsBoss;
        [SerializeField]
        private EnemyManager.EnemyType enemyType;
        [SerializeField]
        private float followDistance;
        [SerializeField]
        private float stopDistanceFromPlayer;
        [SerializeField]
        private bool shouldDropObject;

        [Header("Attack Settings")]
        [SerializeField]
        private float spawnGracePeriod = 1.5f;
        [SerializeField]
        private float distanceToStartAttacking;
        [SerializeField]
        private float aimingThreshold = 25.0f;
        [SerializeField]
        private Vector2 weaponSwitchInterval;

        [Header("References")]
        [SerializeField]
        private BarUI enemyUI;
        [SerializeField]
        private HitBoxHandler hitBox;
        [SerializeField]
        private GameObject objectToDrop;
        [SerializeField]
        private NavMeshAgent agent;

        HealthHandler healthHandler;
        WeaponHandler weaponHandler;

        private float timeNeededForWeaponSwitch = 0.0f;
        private float timeSinceLastWeaponSwitch = 0.0f;
        private NavMeshPath pathToUse;
        private Queue<Vector3> cornerQueue;
        private Vector3 currentDestination;
        bool hasPath;
        private float currentDistance;
        private float minDistanceArrived = 1.0f;
        Vector3 direction;
        private float moveSpeed = 1.0f;
        AimConstraint aimConstraint;
        private bool gracePeriodEnded;

        bool bIsDead = false;

        private void Awake()
        {
            healthHandler = this.GetComponent<HealthHandler>();
            weaponHandler = this.GetComponent<WeaponHandler>();

            if (Persistence.PersistenceManager.Instance)
                if (Persistence.PersistenceManager.Instance.GetFloorsCleared() > 0)
                    AdjustDifficulty();
        }

        // Start is called before the first frame update
        void Start()
        {
            SetupEnemy();
        }

        // Update is called once per frame
        void Update()
        {
            EnemyAction();
            WeaponSwitchInterval();
            if(!gracePeriodEnded) GracePeriod();
        }

        private void GracePeriod()
        {
            spawnGracePeriod -= Time.deltaTime;
            if (spawnGracePeriod <= 0f)
            {
                gracePeriodEnded = true;
            }
        }

        private void WeaponSwitchInterval()
        {
            if (timeNeededForWeaponSwitch > 0.1f)
            {
                timeSinceLastWeaponSwitch += Time.deltaTime;
                if (timeSinceLastWeaponSwitch >= timeNeededForWeaponSwitch)
                {
                    weaponHandler.SwitchWeapon(true);
                    timeSinceLastWeaponSwitch = 0.0f;
                    timeNeededForWeaponSwitch = UnityEngine.Random.Range(weaponSwitchInterval.x, weaponSwitchInterval.y);
                }
            }
        }

        private void EnemyAction()
        {
            Vector3 playerPos = UtilFunctions.instance.GetPlayerPos();
            float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
            
            if (distanceToPlayer <= followDistance && distanceToPlayer > stopDistanceFromPlayer)
            {
                aimConstraint.constraintActive = false;
                agent.SetDestination(playerPos);
            }
            
            else
            {
                aimConstraint.constraintActive = true;
            }


            if (IsFacingPlayer(playerPos) && distanceToPlayer <= distanceToStartAttacking && gracePeriodEnded)
            {
                weaponHandler.Shoot(true);
            }
        }

        private void AdjustDifficulty()
        {
            Debug.Log("Adjusting Enemy Difficulty...");
            int floorsCleared = Persistence.PersistenceManager.Instance.GetFloorsCleared();
            healthHandler.ChangeMaxHealthBy(Progression.ProgressionManager.Instance.GetEnemyHealthIncrease() * floorsCleared);
            weaponHandler.ChangeOverallDamageMultiplierBy(Progression.ProgressionManager.Instance.GetEnemyDamageMultIncrease() * floorsCleared);
        }

        private void SetupEnemy()
        {
            healthHandler.Initialise();
            hitBox.SetHitBoxCallBack(ApplyDamage);
            enemyUI.SetupBarSlider(healthHandler.GetHealth());
            enemyUI.UpdateBarUI(healthHandler.GetHealth(), "Health: ");
            weaponHandler.SetReloadCallback(ReloadCallBack);
            agent.stoppingDistance = stopDistanceFromPlayer;
            timeNeededForWeaponSwitch = UnityEngine.Random.Range(weaponSwitchInterval.x, weaponSwitchInterval.y);

            aimConstraint = this.gameObject.AddComponent<AimConstraint>();
            ConstraintSource aimSource = new ConstraintSource();
            aimSource.sourceTransform = UtilFunctions.instance.GetPlayerObj().transform;
            aimSource.weight = 1.0f;
            aimConstraint.AddSource(aimSource);
            aimConstraint.constraintActive = false;
        }

        private bool IsFacingPlayer(Vector3 playerPos)
        {
            float ang = Vector3.Angle(transform.forward, transform.position - playerPos);
            float minThreshold = 180.0f - aimingThreshold; // 180 = opposite direction / facing
            float maxThreshold = 180.0f + aimingThreshold;
            float thresholdResult = (ang - minThreshold) * (maxThreshold - ang);
            return thresholdResult >= 0;
        }

        public void ReloadCallBack(int weaponNum, Sprite weaponIcon, int magAmmo, int reserveAmmo)
        {
            //Not Currently Used
        }

        private void ApplyDamage(float dmg, bool ignoreShield)
        {
            healthHandler.ChangeHealthBy(-dmg, ignoreShield);
            if (healthHandler.GetHealth() <= 0)
                KillEnemy();
            enemyUI.UpdateBarUI(healthHandler.GetHealth(), "Health: ");
        }

        private void KillEnemy()
        {
            if (!bIsDead)
            {
                bIsDead = true;
                FXManager.Instance.PlayEffect(FXManager.EffectType.Blood, this.gameObject.transform.position, null);
                if (shouldDropObject)
                {
                    GameObject item = Instantiate(objectToDrop, this.transform);
                    item.transform.parent = null;
                }
                if(!bIsBoss)
                    UtilFunctions.instance.GetPlayerObj().GetComponent<Player.PlayerController>().IncreaseEnemiesKilled();
                else
                    UtilFunctions.instance.GetPlayerObj().GetComponent<Player.PlayerController>().IncreaseBossesKilled();
                weaponHandler.CleanUpWeaponPools();
                Destroy(this.gameObject);
            }
        }
    }
}
