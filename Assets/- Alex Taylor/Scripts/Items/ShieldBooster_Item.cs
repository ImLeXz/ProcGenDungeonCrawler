using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;
using ATDungeon.Player;

namespace ATDungeon.Items
{
    public class ShieldBooster_Item : ItemBase
    {
        [Header("Shield Booster Settings: ")]
        [SerializeField]
        private float maxShieldIncrease;

        private HealthHandler playerHealthHandler;
        private HitBoxHandler playerHitBox;

        protected override void Initialise()
        {
            PlayerController playerController = UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>();
            playerHitBox = playerController.GetHitBox();
            playerHealthHandler = playerController.GetHealthHandler();
        }

        protected override void ItemAction()
        {
            playerHealthHandler.ChangeMaxShieldBy(maxShieldIncrease);
        }

        protected override void ItemComplete()
        {
            playerHealthHandler.ChangeMaxShieldBy(-maxShieldIncrease);
            playerHitBox.GenerateHit(0, false); //This Is To Update Shield UI To Display Proper Max Shield
        }
    }
}
