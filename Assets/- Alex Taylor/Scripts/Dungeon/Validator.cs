using System.Collections;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using UnityEngine;

namespace ATDungeon.Dungeon.Setup
{
    public class Validator : MonoBehaviour
    {
        private bool isValid = true;
        private bool checkIsValid = false;

        private Collider validatorCollider;

        public Collider ValidatorCollider => validatorCollider;

        private void Start()
        {
            if (DungeonManager.instance.GetShouldCheckValidators())
                checkIsValid = true;
            else
            {
                MeshRenderer parentRenderer = this.transform.parent.gameObject.GetComponent<MeshRenderer>();
                if (parentRenderer)
                    parentRenderer.enabled = false;
            }

            validatorCollider = this.GetComponent<Collider>();
        }

        public void StartValidationChecks() { checkIsValid = true; }

        public bool GetValidationResult()
        {
            checkIsValid = false;
            return isValid;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!checkIsValid) return;
            
            foreach (var c in collision.contacts)
            {
                if (collision.gameObject.tag == "Validator")
                {
                    isValid = false;
                    if (DungeonManager.instance.GetShouldCheckValidators())
                    {
                        Debug.LogWarning("Validators Are Overlapping");
                    }
                }
            }
            
        }
    }
}
