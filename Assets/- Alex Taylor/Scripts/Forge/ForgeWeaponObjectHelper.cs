using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ATDungeon.UI.Forge
{
    public class ForgeWeaponObjectHelper : MonoBehaviour
    {
        [System.Serializable]
        public class WeaponObject
        {
            public GameObject weaponPrefab;
            public GameObject parent;
            public Toggle selectToggle;
            public Image weaponImage;
            public GameObject weaponSelectedImage;
        }

        [SerializeField]
        private WeaponObject weaponObject;

        public WeaponObject GetWeaponObject() { return weaponObject; }
    }
}
