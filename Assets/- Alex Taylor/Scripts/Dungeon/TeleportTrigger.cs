using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.SceneManagement;

namespace ATDungeon.Dungeon
{
    public class TeleportTrigger : MonoBehaviour
    {
        bool checkForInput = false;
        Player.PlayerController playerController;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (checkForInput)
            {
                if (Input.GetKeyDown(KeyCode.E) && playerController)
                {
                    playerController.IncreaseFloorsCleared();
                    playerController.SavePlayerStuff();
                    DungeonSceneManager.Instance.ReloadDungeon();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                playerController = other.gameObject.transform.parent.gameObject.GetComponent<Player.PlayerController>();
                if (DungeonSceneManager.Instance)
                {
                    if (playerController.GetCurrentScore() >= Progression.ProgressionManager.Instance.GetScoreNeededToTeleport())
                    {
                        checkForInput = true;
                        Utility.UIManager.Instance.DisplayPrompt("Press E To Teleport!");
                    }
                    else
                        Utility.UIManager.Instance.DisplayPrompt("You Need [ " + Progression.ProgressionManager.Instance.GetScoreNeededToTeleport() + " ] Score To Teleport!");
                }
                else
                    Debug.LogWarning("No Scene Manager Present!");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                checkForInput = false;
                Utility.UIManager.Instance.ClosePrompt();
            }
        }
    }
}
