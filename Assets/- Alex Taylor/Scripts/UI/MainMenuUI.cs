using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.SceneManagement;

namespace ATDungeon.UI.Menus
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Confirmation UI References")]
        [SerializeField]
        private GameObject confirmationUIParent;
        [SerializeField]
        private TextMeshProUGUI confirmTxt;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void ConfirmQuit()
        {
            Application.Quit();
        }

        public void QuitGameBtnPressed()
        {
            DisplayConfirmation("Are You Sure You Want To Quit The Game?");
        }

        public void StartGameBtn()
        {
            DungeonSceneManager.Instance.StartCoroutine(DungeonSceneManager.Instance.LoadScene(DungeonSceneManager.Instance.GetClassSelectionSceneBuildIndex(), "circlewipe", UnityEngine.SceneManagement.LoadSceneMode.Additive, this.gameObject.scene));
            //classSelectionUI.SetActive(true);
        }

        private void DisplayConfirmation(string msg)
        {
            confirmationUIParent.SetActive(true);
            confirmTxt.text = msg;
        }

        public void SettingsBtnPressed()
        {
            int buildIndex = DungeonSceneManager.Instance.GetSettingsMenuSceneBuildIndex();
            DungeonSceneManager.Instance.StartCoroutine(DungeonSceneManager.Instance.LoadScene(buildIndex, "", UnityEngine.SceneManagement.LoadSceneMode.Additive));
        }

        public void StatisticsBtnPressed()
        {
            int buildIndex = DungeonSceneManager.Instance.GetStatisticsSceneBuildIndex();
            DungeonSceneManager.Instance.StartCoroutine(DungeonSceneManager.Instance.LoadScene(buildIndex, "", UnityEngine.SceneManagement.LoadSceneMode.Additive));
        }
    }
}
