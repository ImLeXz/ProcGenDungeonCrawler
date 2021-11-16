using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;
using ATDungeon.Player;

namespace ATDungeon.UI.Shop
{
    public class ShopTrigger : MonoBehaviour
    {
        [SerializeField]
        private Transform itemSpawnPos;

        private ShopUI shopUI;
        private GameObject promptUI;

        // Start is called before the first frame update
        void Start()
        {
            shopUI = UIManager.Instance.GetShopUI().GetComponent<ShopUI>();
            shopUI.SetPickupSpawnPos(itemSpawnPos.position);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>().RunShopControls(true);
                UIManager.Instance.DisplayPrompt("Press E To Open Shop");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                UtilFunctions.instance.GetPlayerObj().GetComponent<PlayerController>().RunShopControls(false);
                UIManager.Instance.ClosePrompt();
                shopUI.CloseShop();
            }
        }
    }
}
