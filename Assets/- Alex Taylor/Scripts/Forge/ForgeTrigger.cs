using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Player;
using ATDungeon.Utility;

namespace ATDungeon.UI.Forge
{
    public class ForgeTrigger : MonoBehaviour
    {

        private ForgeUI forgeUI;
        private GameObject promptUI;

        // Start is called before the first frame update
        void Start()
        {
            forgeUI = UIManager.Instance.GetForgeUI().GetComponent<ForgeUI>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>().RunForgeControls(true);
                UIManager.Instance.DisplayPrompt("Press E To Open Forge");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>().RunForgeControls(false);
                UIManager.Instance.ClosePrompt();
                forgeUI.CloseForge();
            }
        }
    }
}
