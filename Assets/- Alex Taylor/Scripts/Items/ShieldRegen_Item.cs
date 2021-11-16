using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;
using ATDungeon.Player;

namespace ATDungeon.Items
{
    public class ShieldRegen_Item : ItemBase
    {
        [Header("Shield Regen Settings: ")]
        [SerializeField]
        private float shieldRegenMultiplierIncrease;

        private HealthHandler playerHealthHandler;

        protected override void Initialise()
        {
            PlayerController playerController = UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>();
            playerHealthHandler = playerController.GetHealthHandler();
        }

        protected override void ItemAction()
        {
            playerHealthHandler.ChangeShieldRegenMultiplierBy(shieldRegenMultiplierIncrease);
        }

        protected override void ItemComplete()
        {
            playerHealthHandler.ChangeShieldRegenMultiplierBy(-shieldRegenMultiplierIncrease);
        }
    }
}
