using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;

namespace ATDungeon.Items
{
    public class DeployableShield_Item : ItemBase
    {
        [Header("Deployable Shield Settings: ")]
        [SerializeField]
        private GameObject shieldPrefab;
        [SerializeField]
        private Vector3 shieldSize;

        GameObject spawnedShield;

        protected override void Initialise()
        {
        }

        protected override void ItemAction()
        {
            spawnedShield = Instantiate(shieldPrefab, ATDungeon.Dungeon.DungeonManager.instance.GetItemSpawnParent());
            spawnedShield.transform.localPosition = UtilFunctions.instance.GetFloorPos();
            spawnedShield.transform.localScale = shieldSize;
        }

        protected override void ItemComplete()
        {
            Destroy(spawnedShield);
        }
    }
}
