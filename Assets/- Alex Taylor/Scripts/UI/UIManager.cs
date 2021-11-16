using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ATDungeon.Utility
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Prompt References")]
        [SerializeField]
        private GameObject promptUI;
        [SerializeField]
        private TextMeshProUGUI promptTxt;

        [Header("UI References")]
        [SerializeField]
        private GameObject shopUI;
        [SerializeField]
        private GameObject forgeUI;
        [SerializeField]
        private GameObject inventoryUI;
        [SerializeField]
        private GameObject pauseMenuUI;
        [SerializeField]
        private GameObject gameOverUI;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ClosePrompt()
        {
            promptUI.SetActive(false);
        }

        public void DisplayPrompt(string txt)
        {
            promptTxt.text = txt;
            promptUI.SetActive(true);
        }

        public GameObject GetShopUI() { return shopUI; }
        public GameObject GetForgeUI() { return forgeUI; }
        public GameObject GetInventoryUI() { return inventoryUI; }
        public GameObject GetPauseMenuUI() { return pauseMenuUI; }
        public GameObject GetGameOverUI() { return gameOverUI; }
    }
}
