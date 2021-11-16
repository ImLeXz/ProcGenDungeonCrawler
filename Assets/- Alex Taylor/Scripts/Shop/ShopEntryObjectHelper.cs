using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ATDungeon.UI.Shop
{
    public class ShopEntryObjectHelper : MonoBehaviour
    {
        [System.Serializable]
        public class EntryObjectHelper
        {
            public ShopUI.ShopEntry shopEntry;
            public GameObject parent;
            public Button purchaseBtn;
            public TextMeshProUGUI costTxt;
            public TextMeshProUGUI nameTxt;
            public Image entryImage;
        }

        [SerializeField]
        private EntryObjectHelper entryObjectHelper;

        public EntryObjectHelper GetEntryObject() { return entryObjectHelper; }
    }
}
