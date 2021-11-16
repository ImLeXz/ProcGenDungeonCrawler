using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.Classes;
using ATDungeon.Items;
using ATDungeon.Weapons;
using ATDungeon.SceneManagement;
using ATDungeon.Persistence;

namespace ATDungeon.UI.ClassSelection
{
    public class ClassSelectionUI : MonoBehaviour
    {

        [Header("Misc References")]
        [SerializeField]
        private GameObject UIBlurring;
        [SerializeField]
        private GameObject classObjectTemplate;
        [SerializeField]
        private Transform classObjectParent;

        [Header("Selected Class References")]
        [SerializeField]
        private ToggleGroup classToggleGroup;
        [SerializeField]
        private Image scWepIcon;
        [SerializeField]
        private TextMeshProUGUI scWepNameTxt;
        [SerializeField]
        private Image scItem01Icon;
        [SerializeField]
        private TextMeshProUGUI scItem01NameTxt;
        [SerializeField]
        private Image scItem02Icon;
        [SerializeField]
        private TextMeshProUGUI scItem02NameTxt;
        [SerializeField]
        private TextMeshProUGUI scOtherInfoTxt;

        [Header("Error UI References")]
        [SerializeField]
        private GameObject errorUIParent;
        [SerializeField]
        private TextMeshProUGUI errorTxt;

        [Header("Confirmation UI References")]
        [SerializeField]
        private GameObject confirmationUIParent;
        [SerializeField]
        private TextMeshProUGUI confirmTxt;

        private ClassManager.PlayerClass[] playerClasses;
        private ClassObjHelper.ClassObject selectedClass;

        private void Start()
        {
            playerClasses = ClassManager.Instance.GetPlayerClasses();
            Initialise();
        }

        private void Initialise()
        {
            for (int i = 0; i < playerClasses.Length; i++)
            {
                GameObject instantiatedClassUI = Instantiate(classObjectTemplate, classObjectParent);
                ClassObjHelper classObjHelper = instantiatedClassUI.GetComponent<ClassObjHelper>();
                ClassObjHelper.ClassObject classObject = classObjHelper.GetClassObject();
                classObject.playerClass = playerClasses[i];

                classObject.classNameTxt.text = classObject.playerClass.GetClassName();
                classObject.classIcon.sprite = classObject.playerClass.GetClassIcon();

                WeaponBase dfWep = null;
                ItemBase dfItem01 = null;
                ItemBase dfItem02 = null;
                if (classObject.playerClass.GetDefaultWeapon())
                    dfWep = classObject.playerClass.GetDefaultWeapon().GetComponent<WeaponBase>();
                if(classObject.playerClass.GetDefaultItem01())
                    dfItem01 = classObject.playerClass.GetDefaultItem01().GetComponent<ItemBase>();
                if(classObject.playerClass.GetDefaultItem02())
                    dfItem02 = classObject.playerClass.GetDefaultItem02().GetComponent<ItemBase>();

                classObject.defaultWeapon = dfWep;
                classObject.defaultItem01 = dfItem01;
                classObject.defaultItem02 = dfItem02;

                classObject.selectionToggle.group = classToggleGroup;
                classObject.selectionToggle.onValueChanged.AddListener(delegate { UpdateSelectedClassUI(classObject.selectionToggle.isOn, classObject); });

                if (i == 0)
                {
                    classObject.selectionToggle.isOn = true;
                    selectedClass = classObject;
                }
                else
                    classObject.selectionToggle.isOn = false;
            }
        }

        public void UpdateSelectedClassUI(bool b, ClassObjHelper.ClassObject selected)
        {
            if (b)
            {
                selectedClass = selected;

                if (selectedClass.defaultWeapon)
                {
                    scWepIcon.sprite = selectedClass.defaultWeapon.GetWeaponImage();
                    scWepNameTxt.text = selectedClass.defaultWeapon.GetWeaponName();
                }

                if (selectedClass.defaultItem01)
                {
                    scItem01Icon.sprite = selectedClass.defaultItem01.GetItemImage();
                    scItem01NameTxt.text = selectedClass.defaultItem01.GetItemName();
                }

                if (selectedClass.defaultItem02)
                {
                    scItem02Icon.sprite = selectedClass.defaultItem02.GetItemImage();
                    scItem02NameTxt.text = selectedClass.defaultItem02.GetItemName();
                }

                string[] textLines = selectedClass.playerClass.GetOtherClassInfo();
                string otherInfoText = textLines[0];
                for(int i = 1; i < textLines.Length; i++)
                {
                    otherInfoText += "\n" + textLines[i];
                }
                scOtherInfoTxt.text = otherInfoText;
            }
        }

        private void DisplayError(string errorMsg)
        {
            errorUIParent.SetActive(true);
            errorTxt.text = errorMsg;
        }

        private void DisplayConfirmation(string className)
        {
            confirmationUIParent.SetActive(true);
            confirmTxt.text = "Start Playing As: [ " + className + " ] ?";
        }

        public void ConfirmBtnPressed()
        {
            DisplayConfirmation(selectedClass.playerClass.GetClassName());
        }

        public void ConfirmClassSelection()
        {
            PersistenceManager.Instance.SetPlayerClass(selectedClass.playerClass);
            DungeonSceneManager.Instance.StartCoroutine(DungeonSceneManager.Instance.LoadScene(DungeonSceneManager.Instance.GetGameSceneBuildIndex(), "crossfade", UnityEngine.SceneManagement.LoadSceneMode.Additive, this.gameObject.scene));
        }

        public void ReturnToMainMenu()
        {
            DungeonSceneManager.Instance.StartCoroutine(DungeonSceneManager.Instance.LoadScene(DungeonSceneManager.Instance.GetMainMenuSceneBuildIndex(), "circlewipe", UnityEngine.SceneManagement.LoadSceneMode.Additive, this.gameObject.scene));
        }

    }
}
