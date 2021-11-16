using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.Player;
using ATDungeon.SceneManagement;
using ATDungeon.Utility;

namespace ATDungeon.UI.Menus
{
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Misc References")]
        [SerializeField]
        private GameObject UIBlurring;

        [Header("Confirmation UI References")]
        [SerializeField]
        private GameObject confirmationUIParent;
        [SerializeField]
        private TextMeshProUGUI confirmTxt;
        [SerializeField]
        private Button confirmButton;

        private PlayerController playerController;

        bool isOpen;
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        public void OpenMenu()
        {
            Time.timeScale = 0.0f;
            UtilFunctions.instance.IsGamePaused = true;
            this.gameObject.SetActive(true);

            if (!playerController)
                playerController = UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>();

            playerController.TogglePlayerAiming(false);
            UIBlurring.SetActive(true);
            UIManager.Instance.ClosePrompt();
            isOpen = true;
        }

        public void CloseMenu()
        {
            isOpen = false;
            Time.timeScale = 1.0f;
            UtilFunctions.instance.IsGamePaused = false;
            this.gameObject.SetActive(false);

            UIBlurring.SetActive(false);
            confirmationUIParent.SetActive(false);
            playerController.TogglePlayerAiming(true);
        }

        private void DisplayConfirmation(string msg)
        {
            confirmationUIParent.SetActive(true);
            confirmTxt.text = msg;
        }

        public void QuitGameBtnPressed()
        {
            DisplayConfirmation("Are You Sure You Want To Quit The Game?");
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(delegate { ConfirmQuit(); });
        }

        public void MainMenuBtnPressed()
        {
            DisplayConfirmation("Are You Sure You Want To Return To Main Menu?");
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(delegate { MainMenuConfirm(); });
        }

        public void SettingsBtnPressed()
        {
            Time.timeScale = 1.0f;
            int buildIndex = DungeonSceneManager.Instance.GetSettingsMenuSceneBuildIndex();
            DungeonSceneManager.Instance.StartCoroutine(DungeonSceneManager.Instance.LoadScene(buildIndex, "", UnityEngine.SceneManagement.LoadSceneMode.Additive));
            Time.timeScale = 0.0f;
        }

        public void ConfirmQuit()
        {
            Application.Quit();
        }

        public void MainMenuConfirm()
        {
            CloseMenu();
            int buildIndex = DungeonSceneManager.Instance.GetMainMenuSceneBuildIndex();
            DungeonSceneManager.Instance.StartCoroutine(DungeonSceneManager.Instance.LoadScene(buildIndex, "crossfade", UnityEngine.SceneManagement.LoadSceneMode.Additive, this.gameObject.scene));
        }

    }
}
