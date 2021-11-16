using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;
using ATDungeon.Player.Movement;
using ATDungeon.UI;
using ATDungeon.Persistence;
using ATDungeon.Classes;
using Fragsurf.Movement;

namespace ATDungeon.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerUI playerUI;
        [SerializeField]
        private PlayerView playerView;
        [SerializeField]
        private HitBoxHandler hitBox;
        [SerializeField]
        private GameObject fallBackWeapon;

        private HealthHandler healthHandler = null;
        private WeaponHandler weaponHandler = null;
        private ItemHandler itemHandler = null;
        private UI.Inventory.InventoryUI inventoryUI = null;
        private UI.Shop.ShopUI shopUI = null;
        private UI.Forge.ForgeUI forgeUI = null;
        private UI.Menus.PauseMenuUI pauseMenuUI = null;
        private UI.Menus.GameOverUI gameOverUI = null;
        private List<int> keyIDList = new List<int>();

        private int currentCoins = 0;
        private int totalCoins = 0;
        private int enemiesKilled = 0;
        private int bossesKilled = 0;
        private int floorsCleared = 0;

        private bool hasReleasedTrigger = true;
        private bool runShopControls = false;
        private bool runForgeControls = false;
        private bool bIsDead = false;

        bool isOpen;
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        private void Awake()
        {
            healthHandler = this.GetComponent<HealthHandler>();
            weaponHandler = this.GetComponent<WeaponHandler>();
            weaponHandler.SetReloadCallback(ReloadCallBack);
            itemHandler = this.GetComponent<ItemHandler>();
            forgeUI = UIManager.Instance.GetForgeUI().GetComponent<UI.Forge.ForgeUI>();
            shopUI = UIManager.Instance.GetShopUI().GetComponent<UI.Shop.ShopUI>();
            inventoryUI = UIManager.Instance.GetInventoryUI().GetComponent<UI.Inventory.InventoryUI>();
            pauseMenuUI = UIManager.Instance.GetPauseMenuUI().GetComponent<UI.Menus.PauseMenuUI>();
        }

        private void ReloadCallBack(int weaponNum, Sprite weaponIcon, int magAmmo, int reserveAmmo)
        {
            playerUI.UpdateAmmoUI(weaponNum, weaponHandler.GetCurrentWeaponNum(), weaponIcon, magAmmo, reserveAmmo);
        }

        private void ItemPickupCallback(int itemNum, Sprite itemIcon, int itemUses, Items.ItemBase.ItemState itemState)
        {
            playerUI.UpdateItemUI(itemNum, itemIcon, itemUses, itemState);
        }

        // Start is called before the first frame update
        void Start()
        {
            hitBox.SetHitBoxCallBack(ApplyDamage);
            itemHandler.SetItemPickupCallback(ItemPickupCallback);

            if (PersistenceManager.Instance)
                floorsCleared = PersistenceManager.Instance.GetFloorsCleared();

            else
                floorsCleared = 0;

            PlayerClassStuff();
            UpdateAllUI();
        }

        private void PlayerClassStuff()
        {
            if (PersistenceManager.Instance)
            {
                ClassManager.PlayerClass pClass = PersistenceManager.Instance.GetPlayerClass();

                //Damage
                weaponHandler.ChangeOverallDamageMultiplierBy(pClass.GetDamageMultiplierChange());

                //Items and Weapons
                if (floorsCleared == 0) //Only give weapons / items on first floor
                {
                    weaponHandler.AddWeapon(pClass.GetDefaultWeapon());
                    itemHandler.AddItem(pClass.GetDefaultItem01());
                    itemHandler.AddItem(pClass.GetDefaultItem02());
                }
                else
                    LoadPlayerStuff();

                //Health
                float newHealthRegen = (healthHandler.GetHealthRegenMultiplier() * pClass.GetHealthRegenMultiplier()) - healthHandler.GetHealthRegenMultiplier();
                healthHandler.ChangeHealthRegenMultiplierBy(newHealthRegen);
                float newHealth = (healthHandler.GetMaxHealth() * pClass.GetHealthMultiplier()) - healthHandler.GetMaxHealth();
                Debug.Log("New Health: [" + healthHandler.GetMaxHealth() + " - " + newHealth + " = " + (healthHandler.GetMaxHealth() + newHealth).ToString() + "]");
                healthHandler.ChangeMaxHealthBy(newHealth);
                healthHandler.SetCanAutoRegenHealth(pClass.GetAutoRegenHealth());

                //Shield
                float newShieldRegen = (healthHandler.GetShieldRegenMultiplier() * pClass.GetShieldRegenMultiplier()) - healthHandler.GetShieldRegenMultiplier();
                healthHandler.ChangeShieldRegenMultiplierBy(newShieldRegen);
                float newShield = (healthHandler.GetMaxShield() * pClass.GetShieldMultiplier()) - healthHandler.GetMaxShield();
                healthHandler.ChangeMaxShieldBy(newShield);
                healthHandler.SetCanAutoRegenShield(pClass.GetAutoRegenShield());

                healthHandler.Initialise();

                MovementConfig movementConfig = this.GetComponent<SurfCharacter>().movementConfig;
                movementConfig.autoBhop = pClass.GetAutoBhop();

                movementConfig.walkSpeed *= pClass.GetMoveSpeedMultiplier();
                movementConfig.sprintSpeed *= pClass.GetMoveSpeedMultiplier();
                movementConfig.crouchSpeed *= pClass.GetMoveSpeedMultiplier();
            }
            else
            {
                weaponHandler.AddWeapon(fallBackWeapon);
                healthHandler.Initialise();
            }
        }

        private void UpdateAllUI()
        {
            playerUI.SetupHealthSliders(healthHandler.GetMaxHealth(), healthHandler.GetMaxShield());
            playerUI.UpdateShieldUI(healthHandler.GetShield());
            playerUI.UpdateHealthUI(healthHandler.GetHealth());
            playerUI.UpdateCoinsUI(currentCoins);
            playerUI.UpdateFloorsUI(floorsCleared);
            playerUI.UpdateKilledUI(enemiesKilled);
            playerUI.UpdateScoreUI(GetCurrentScore());
            for(int i = 0; i < weaponHandler.GetWeapons().Length + 1; i++)
                weaponHandler.SwitchWeapon(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (!bIsDead)
            {
                if (healthHandler.GetCanAutoRegenShield())
                    playerUI.UpdateShieldUI(healthHandler.GetShield());
                if (healthHandler.GetCanAutoRegenHealth())
                    playerUI.UpdateHealthUI(healthHandler.GetHealth());
                InventoryControls();

                if (runShopControls)
                    ShopControls();

                else if (runForgeControls)
                    ForgeControls();

                else if (!inventoryUI.IsOpen && !pauseMenuUI.IsOpen)
                {
                    WeaponControls();
                    ItemControls();
                }
            }
        }

        private void DebugControls()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                healthHandler.ChangeHealthBy(-10.0f, false);
                playerUI.UpdateHealthUI(healthHandler.GetHealth());
            }
        }

        private bool IsOtherUIOpen()
        {
            return shopUI.IsOpen || forgeUI.IsOpen;
        }

        private void WeaponControls()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                weaponHandler.Shoot(hasReleasedTrigger);
                hasReleasedTrigger = false;
                playerUI.UpdateAmmoUI(weaponHandler.GetCurrentWeaponNum(), weaponHandler.GetCurrentWeaponNum(), weaponHandler.GetCurrentWeapon().GetWeaponImage(), weaponHandler.GetCurrentMagAmmo(), weaponHandler.GetCurrentReserveAmmo());
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                hasReleasedTrigger = true;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                weaponHandler.Reload();
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                weaponHandler.SwitchWeapon(true);
            }

            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                weaponHandler.SwitchWeapon(false);
            }
        }

        private void ItemControls()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                itemHandler.UseItem(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                itemHandler.UseItem(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                itemHandler.UseItem(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                itemHandler.UseItem(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                itemHandler.UseItem(4);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                itemHandler.UseItem(5);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                itemHandler.UseItem(6);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                itemHandler.UseItem(7);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                itemHandler.UseItem(8);
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                itemHandler.UseItem(9);
            }
        }

        private void ShopControls()
        {
            if (Input.GetKeyDown(KeyCode.E) && !shopUI.IsOpen)
                shopUI.OpenShop();
            else if(Input.GetKeyDown(KeyCode.Escape) && shopUI.IsOpen)
                shopUI.CloseShop();
        }

        private void ForgeControls()
        {
            if (Input.GetKeyDown(KeyCode.E) && !forgeUI.IsOpen)
                forgeUI.OpenForge();
            else if (Input.GetKeyDown(KeyCode.Escape) && forgeUI.IsOpen)
                forgeUI.CloseForge();
        }

        private void InventoryControls()
        {
            if (Input.GetKeyDown(KeyCode.I) && !inventoryUI.IsOpen && !IsOtherUIOpen() && !pauseMenuUI.IsOpen)
                inventoryUI.OpenInventory();

            if (Input.GetKeyDown(KeyCode.Escape) && !IsOtherUIOpen())
            {
                if (inventoryUI.IsOpen)
                    inventoryUI.CloseInventory();

                else if (!UtilFunctions.instance.IsGamePaused && !pauseMenuUI.IsOpen)
                    pauseMenuUI.OpenMenu();

                else if (UtilFunctions.instance.IsGamePaused && pauseMenuUI.IsOpen)
                    pauseMenuUI.CloseMenu();
            }


        }

        public void ChangeCoinsBy(int amount)
        {
            currentCoins += amount;
            if (amount > 0)
                totalCoins += amount;
            playerUI.UpdateCoinsUI(currentCoins);
        }

        public bool EquipDrop(GameObject droppedItem)
        {
            bool equippedDrop = false;

            if(droppedItem.tag == "Weapon")
                equippedDrop = weaponHandler.AddWeapon(droppedItem);

            else if(droppedItem.tag == "Item")
                equippedDrop = itemHandler.AddItem(droppedItem);

            return equippedDrop;
        }

        private void ApplyDamage(float dmg, bool ignoreShield)
        {
            //Debug.Log("Player Hit For: [" + dmg + "]");
            healthHandler.ChangeHealthBy(-dmg, ignoreShield);
            if (healthHandler.GetHealth() <= 0)
                KillPlayer();
            playerUI.UpdateHealthUI(healthHandler.GetHealth());
        }

        private void KillPlayer()
        {
            if (!bIsDead)
            {
                bIsDead = true;
                SavePlayerStuff();
                gameOverUI = UIManager.Instance.GetGameOverUI().GetComponent<UI.Menus.GameOverUI>();
                gameOverUI.OpenMenu();
            }
        }

        public void AddKeyID(int id)
        {
            keyIDList.Add(id);
            playerUI.UpdateKeyUI(id-1, true);
        }

        public bool HasKeyID(int id)
        {
            if (keyIDList.Contains(id))
                return true;
            else return false;
        }

        public void IncreaseEnemiesKilled()
        {
            enemiesKilled++;
            playerUI.UpdateKilledUI(enemiesKilled);
            playerUI.UpdateScoreUI(GetCurrentScore());
        }

        public void IncreaseBossesKilled()
        {
            bossesKilled++;
            playerUI.UpdateScoreUI(GetCurrentScore());
        }

        public int GetCurrentScore()
        {
            int enemyScore = enemiesKilled * Progression.ProgressionManager.Instance.GetScorePerEnemy();
            int bossScore = bossesKilled * Progression.ProgressionManager.Instance.GetScorePerBoss();
            return enemyScore + bossScore;
        }

        public void LoadPlayerStuff()
        {
            currentCoins = PersistenceManager.Instance.GetPlayerCoins();
            totalCoins = PersistenceManager.Instance.GetPlayerTotalCoins();
            enemiesKilled = PersistenceManager.Instance.GetEnemiesKilled();
            bossesKilled = PersistenceManager.Instance.GetBossesKilled();
            playerUI.SetTimeSurvived(PersistenceManager.Instance.GetTimeSurvived());

            GameObject[] weaponsArray = PersistenceManager.Instance.GetPlayerWeapons();
            foreach(GameObject weaponObj in weaponsArray)
                if(weaponObj != null)
                    weaponHandler.AddWeapon(weaponObj);

            GameObject[] itemsArray = PersistenceManager.Instance.GetPlayerItems();
            foreach (GameObject itemObj in itemsArray)
                if(itemObj != null)
                    itemHandler.AddItem(itemObj);
        }

        public void SavePlayerStuff()
        {
            PersistenceManager.Instance.SetFloorsCleared(floorsCleared);
            PersistenceManager.Instance.SetPlayerCoins(currentCoins);
            PersistenceManager.Instance.SetPlayerTotalCoins(totalCoins);
            PersistenceManager.Instance.SetEnemiesKilled(enemiesKilled);
            PersistenceManager.Instance.SetBossesKilled(bossesKilled);
            PersistenceManager.Instance.SetTimeSurvived(GetTimeSurvived());
            PersistenceManager.Instance.SetPlayerWeapons(GetCurrentWeapons());
            PersistenceManager.Instance.SetPlayerItems(GetCurrentItems());
            PersistenceManager.Instance.CalculateScore();
        }

        public void IncreaseFloorsCleared() { floorsCleared++; }
        public void TogglePlayerAiming(bool b) { playerView.ShouldDoAiming(b); }
        public void RunShopControls(bool b) { runShopControls = b; }
        public void RunForgeControls(bool b) { runForgeControls = b; }

        public int GetCurrentCoins() { return currentCoins; }
        public HealthHandler GetHealthHandler() { return healthHandler; }
        public HitBoxHandler GetHitBox() { return hitBox; }

        public void RemoveWeapon(GameObject wp) { weaponHandler.RemoveWeapon(wp); }
        public void RemoveItem(GameObject itm) { itemHandler.RemoveItem(itm); }
        public GameObject[] GetCurrentWeapons() { return weaponHandler.GetWeapons(); }
        public GameObject[] GetCurrentItems() { return itemHandler.GetItems(); }

        public int GetEnemiesKilled() { return enemiesKilled; }
        public int GetTotalCoins() { return totalCoins; }
        public int GetFloorsCleared() { return floorsCleared; }
        public ClassManager.PlayerClass GetPlayerClass() { if (PersistenceManager.Instance) { return PersistenceManager.Instance.GetPlayerClass(); } else return null; }
        public float GetTimeSurvived() { return playerUI.GetTimeSurvived(); }
        public string GetFormatedTime() { return playerUI.GetFormatedTime(); }
    }
}
