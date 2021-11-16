using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Handlers;
using ATDungeon.Utility;
using ATDungeon.Player;

namespace ATDungeon.Items
{
    public class HealthPotion_Item : ItemBase
    {
        [Header("Health Potion Settings: ")]
        [SerializeField]
        private float healthAmount;

        private HitBoxHandler playerHitBox;

        protected override void Initialise()
        {
            playerHitBox = UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>().GetHitBox();
        }

        protected override void ItemAction()
        {
            playerHitBox.GenerateHit(-healthAmount, true);
        }

        protected override void ItemComplete()
        {

        }
    }
}
