using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;
using ATDungeon.Player;

namespace ATDungeon.Utility
{
    public class RandomDrop : MonoBehaviour
    {
        [System.Serializable]
        public class RandomObj
        {
            public GameObject prefab;
            public float frequencyPercentage;
        }

        [SerializeField]
        private RandomObj[] randomObjects;

        private RandomObj chosenObj = null;
        bool equipSuccess;

        // Start is called before the first frame update
        void Start()
        {
            ChooseObject();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetObject(GameObject obj)
        {
            RandomObj selectedObj = new RandomObj { prefab = obj, frequencyPercentage = 1.0f };
            randomObjects[0] = selectedObj;
        }

        private void ChooseObject()
        {
            float[] weights = new float[randomObjects.Length];
            for (int i = 0; i < randomObjects.Length; i++)
                weights[i] = randomObjects[i].frequencyPercentage;
            int randomIndex = UtilFunctions.instance.GetWeightedRandomValue(weights, 0);
            chosenObj = randomObjects[randomIndex];
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                if (!equipSuccess)
                    equipSuccess = other.gameObject.transform.parent.GetComponent<PlayerController>().EquipDrop(chosenObj.prefab);
                else
                    DestroyDrop();
            }
        }

        private void DestroyDrop()
        {
            //Death Animation
            Destroy(this.gameObject);
        }
    }
}
