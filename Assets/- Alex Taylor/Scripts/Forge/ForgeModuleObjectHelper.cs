using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ATDungeon.UI.Forge
{
    public class ForgeModuleObjectHelper : MonoBehaviour
    {
        [System.Serializable]
        public class ModuleObject
        {
            public ForgeUI.ForgeModule forgeModule;
            public GameObject parent;
            public Toggle selectToggle;
            public TextMeshProUGUI costTxt;
            public TextMeshProUGUI nameTxt;
            public Image moduleImage;
            public GameObject moduleSelectedImage;
        }

        [SerializeField]
        private ModuleObject moduleObject;

        public ModuleObject GetModuleObject() { return moduleObject; }
    }
}
