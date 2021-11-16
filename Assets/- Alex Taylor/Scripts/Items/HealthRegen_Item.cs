using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;
using ATDungeon.Player;

namespace ATDungeon.Items
{
    public class HealthRegen_Item : ItemBase
    {
        [Header("Health Regen Settings: ")]
        [SerializeField]
        private float healthRegenMultiplierIncrease;

        private HealthHandler playerHealthHandler;

        protected override void Initialise()
        {
            PlayerController playerController = UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>();
            playerHealthHandler = playerController.GetHealthHandler();
        }

        protected override void ItemAction()
        {
            playerHealthHandler.ChangeHealthRegenMultiplierBy(healthRegenMultiplierIncrease);
        }

        protected override void ItemComplete()
        {
            playerHealthHandler.ChangeHealthRegenMultiplierBy(-healthRegenMultiplierIncrease);
        }
    }
}
