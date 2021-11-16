using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ATDungeon.UI.Inventory
{
    public class InventoryObjHelper : MonoBehaviour
    {
        [System.Serializable]
        public class InventoryObject
        {
            public GameObject objPrefab;
            public GameObject parent;
            public Toggle selectToggle;
            public TextMeshProUGUI worthTxt;
            public TextMeshProUGUI nameTxt;
            public Image objImage;
            public GameObject objSelectedImage;
        }

        [SerializeField]
        private InventoryObject inventoryObject;

        public InventoryObject GetInventoryObject() { return inventoryObject; }
    }
}