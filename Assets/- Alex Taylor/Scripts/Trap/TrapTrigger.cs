using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;

namespace ATDungeon.Dungeon.Setup
{
    public class TrapTrigger : MonoBehaviour
    {
        [SerializeField]
        private float chanceToTrigger = 1.0f;
        [SerializeField]
        private float trapDuration = 5.0f;
        [SerializeField]
        private GameObject trapWallParent;
        [SerializeField]
        private Collider[] colliders;

        bool bDoOnce = true;
        bool bTrapActive = false;
        float trapActiveTime = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            trapWallParent.SetActive(false);
            SetupColliders();
        }

        // Update is called once per frame
        void Update()
        {
            if (bTrapActive)
            {
                trapActiveTime += Time.deltaTime;
                if (trapActiveTime >= trapDuration)
                {
                    ToggleTrap(false);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (bDoOnce)
            {
                if (other.gameObject.tag == "Player")
                {
                    bDoOnce = false;
                    DestroyTrapTriggers();
                    RandomiseTrap();
                }
            }
        }

        private void SetupColliders()
        {
            foreach (Collider c in colliders)
            {
                BoxCollider newCollider = this.gameObject.AddComponent<BoxCollider>();
                newCollider.center = c.gameObject.transform.localPosition;
                newCollider.size = c.bounds.size;
                newCollider.isTrigger = c.isTrigger;
                Destroy(c.gameObject);
            }
        }

        private void DestroyTrapTriggers()
        {
            foreach(Collider c in colliders)
            {
                Destroy(c);
            }
        }

        private void RandomiseTrap()
        {
            float[] weights = new float[] { 1.0f - chanceToTrigger, chanceToTrigger };
            int shouldTrigger = UtilFunctions.instance.GetWeightedRandomValue(weights, 0);
            if (shouldTrigger == 1)
                ToggleTrap(true);
        }

        private void ToggleTrap(bool state)
        {
            bTrapActive = state;
            trapWallParent.SetActive(state);

        }

    }
}
