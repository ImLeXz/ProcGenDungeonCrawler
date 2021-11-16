using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.Classes;
using ATDungeon.Weapons;
using ATDungeon.Items;

namespace ATDungeon.UI.ClassSelection
{
    public class ClassObjHelper : MonoBehaviour
    {
        [System.Serializable]
        public class ClassObject
        {
            public ClassManager.PlayerClass playerClass;
            public GameObject parent;
            public Toggle selectionToggle;
            public Image classIcon;
            public Image classSelectedImage;
            public TextMeshProUGUI classNameTxt;

            [HideInInspector]
            public WeaponBase defaultWeapon;
            [HideInInspector]
            public ItemBase defaultItem01;
            [HideInInspector]
            public ItemBase defaultItem02;
        }

        [SerializeField]
        private ClassObject classObj;

        public ClassObject GetClassObject() { return classObj; }
    }
}
