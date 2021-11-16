using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.Utility;
using ATDungeon.Player;
using ATDungeon.SceneManagement;

namespace ATDungeon.UI.Menus
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("Misc References")]
        [SerializeField]
        private GameObject UIBlurring;

        [Header("Stats References")]
        [SerializeField]
        private TextMeshProUGUI enemiesKilledTxt;
        [SerializeField]
        private TextMeshProUGUI coinsTxt;
        [SerializeField]
        private TextMeshProUGUI floorsClearedTxt;
        [SerializeField]
        private TextMeshProUGUI playerClassTxt;
        [SerializeField]
        private TextMeshProUGUI timeSurvivedTxt;

        [Header("Confirmation UI References")]
        [SerializeField]
        private GameObject confirmationUIParent;
        [SerializeField]
        private TextMeshProUGUI confirmTxt;


        private PlayerController playerController;

        private void Initialise()
        {
            enemiesKilledTxt.text = "Enemies Killed: " + playerController.GetEnemiesKilled();

            coinsTxt.text = "Coins Collected: " + playerController.GetTotalCoins();

            floorsClearedTxt.text = "Floors Cleared: " + playerController.GetFloorsCleared();

            if (playerController.GetPlayerClass() != null)
                playerClassTxt.text = "Chosen Class: " + playerController.GetPlayerClass().GetClassName();
            else
                playerClassTxt.text = "Chosen Class: NULL";

            string formatedTime = playerController.GetFormatedTime();
            timeSurvivedTxt.text = "Time Survived: " + formatedTime;

        }

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
            Initialise();
        }

        public void CloseMenu()
        {
            Time.timeScale = 1.0f;
            Persistence.PersistenceManager.Instance.SessionClear();
            UtilFunctions.instance.IsGamePaused = false;
            this.gameObject.SetActive(false);
        }

        private void DisplayConfirmation(string msg)
        {
            confirmationUIParent.SetActive(true);
            confirmTxt.text = msg;
        }

        public void MainMenuBtn()
        {
            CloseMenu();
            int buildIndex = DungeonSceneManager.Instance.GetMainMenuSceneBuildIndex();
            DungeonSceneManager.Instance.StartCoroutine(DungeonSceneManager.Instance.LoadScene(buildIndex, "crossfade", UnityEngine.SceneManagement.LoadSceneMode.Additive, this.gameObject.scene));
        }

        public void QuitGameBtnPressed()
        {
            DisplayConfirmation("Are You Sure You Want To Quit The Game?");
        }

        public void ConfirmQuit()
        {
            Application.Quit();
        }
    }
}
