using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Classes
{
    public class ClassManager : MonoBehaviour
    {
        [System.Serializable]
        public class PlayerClass
        {
            [Header("Class UI")]
            [SerializeField]
            private string className;
            [SerializeField]
            private Sprite classIcon;
            [SerializeField]
            private string[] otherClassInfoLines;

            [Header("Weapons And Items")]
            [SerializeField]
            private GameObject defaultWeaponPrefab;
            [SerializeField]
            private GameObject defaultItem01Prefab;
            [SerializeField]
            private GameObject defaultItem02Prefab;

            [Header("Multipliers")]
            [SerializeField]
            private float damageMultiplierChange = 0.0f;
            [SerializeField]
            private float moveSpeedMultiplier = 1.0f;
            [SerializeField]
            private float healthRegenMultiplier = 1.0f;
            [SerializeField]
            private float healthMultiplier = 1.0f;
            [SerializeField]
            private float shieldMultiplier = 1.0f;
            [SerializeField]
            private float shieldRegenMultiplier = 1.0f;

            [Header("Toggles")]
            [SerializeField]
            private bool bAutoBhop = false;
            [SerializeField]
            private bool bAutoRegenHealth = false;
            [SerializeField]
            private bool bAutoRegenShield = true;

            //UI
            public string GetClassName() { return className; }
            public string[] GetOtherClassInfo() { return otherClassInfoLines; }
            public Sprite GetClassIcon() { return classIcon; }

            //Weapons / Items
            public GameObject GetDefaultWeapon() { return defaultWeaponPrefab; }
            public GameObject GetDefaultItem01() { return defaultItem01Prefab; }
            public GameObject GetDefaultItem02() { return defaultItem02Prefab; }

            //Multipliers
            public float GetDamageMultiplierChange() { return damageMultiplierChange; }
            public float GetMoveSpeedMultiplier() { return moveSpeedMultiplier; }
            public float GetHealthMultiplier() { return healthMultiplier; }
            public float GetHealthRegenMultiplier() { return healthRegenMultiplier; }
            public float GetShieldMultiplier() { return shieldMultiplier; }
            public float GetShieldRegenMultiplier() { return shieldRegenMultiplier; }

            //Toggles
            public bool GetAutoBhop() { return bAutoBhop; }
            public bool GetAutoRegenHealth() { return bAutoRegenHealth; }
            public bool GetAutoRegenShield() { return bAutoRegenShield; }
        }

        [SerializeField]
        private PlayerClass[] playerClasses;

        public static ClassManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public PlayerClass[] GetPlayerClasses() { return playerClasses; }
    }
}
