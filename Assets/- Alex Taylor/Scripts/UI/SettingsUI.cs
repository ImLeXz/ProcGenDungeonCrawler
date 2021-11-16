using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.SceneManagement;
using System;
using ATDungeon.Persistence;

namespace ATDungeon.UI.Menus
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("Confirmation UI References")]
        [SerializeField]
        private GameObject confirmationUIParent;
        [SerializeField]
        private TextMeshProUGUI confirmTxt;
        [SerializeField]
        private Button confirmButton;

        [Header("Sensitivity Slider References")]
        [SerializeField]
        private Slider sensSlider;
        [SerializeField]
        private float maxSens;
        [SerializeField]
        private TextMeshProUGUI sensTxt;

        float selectedSens = 0.5f;

        private void Start()
        {
            selectedSens = PersistenceManager.Instance.GetPlayerSens();
            sensSlider.maxValue = maxSens;
            sensSlider.value = selectedSens;
        }

        private void DisplayConfirmation(string msg)
        {
            confirmationUIParent.SetActive(true);
            confirmTxt.text = msg;
        }

        public void ConfirmDiscard()
        {
            DungeonSceneManager.Instance.UnloadScene(DungeonSceneManager.Instance.GetSettingsMenuSceneBuildIndex());
        }

        public void ConfirmChanges()
        {
            PersistenceManager.Instance.SetPlayerSens(selectedSens);
            PersistenceManager.Instance.UpdateSensitivity();
            DungeonSceneManager.Instance.UnloadScene(DungeonSceneManager.Instance.GetSettingsMenuSceneBuildIndex());
        }

        public void DiscardBtnPressed()
        {
            DisplayConfirmation("Are You Sure You Want To Discard Your Changes?");
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(delegate { ConfirmDiscard(); } );
        }

        public void ConfirmSettingsBtn()
        {
            DisplayConfirmation("Are You Sure You Want To Confirm Your Changes?");
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(delegate { ConfirmChanges(); });
        }

        public void AdjustSensitivity(float v)
        {
            selectedSens = (float)Math.Round(v, 2);
            sensTxt.text = selectedSens.ToString();
        }

    }
}
