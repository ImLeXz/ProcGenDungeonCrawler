using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;

namespace ATDungeon.Items
{
    public class Blink_Item : ItemBase
    {
        [Header("Blink Settings")]
        [SerializeField]
        private float maxBlinkDistance;
        [SerializeField]
        private float floorDetectionMaxDistance;
        [SerializeField]
        private float floorOffset;

        protected override void Initialise()
        {
        }

        protected override void ItemAction()
        {
            int layerMask = 1 << LayerMask.NameToLayer("Default");
            RaycastHit hit;
            RaycastHit hit2;
            Camera fpsCam = UtilFunctions.instance.GetFPSCamera();
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            Vector3 ray2Origin = rayOrigin;

            GameObject finalHitPoint = new GameObject("Raycast Hitpoint");
            finalHitPoint.transform.forward = fpsCam.transform.forward;

            //First Ray
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, maxBlinkDistance, layerMask))
            {
                //Debug.DrawRay(rayOrigin, fpsCam.transform.forward * hit.distance, Color.yellow);
                //Debug.Log("Did Hit");

                ray2Origin += fpsCam.transform.forward * hit.distance;
            }
            else
            {
                //Debug.DrawRay(rayOrigin, fpsCam.transform.forward * maxBlinkDistance, Color.yellow);

                ray2Origin += fpsCam.transform.forward * maxBlinkDistance;
            }

            //Second Ray For Floor
            if (Physics.Raycast(ray2Origin, -Vector3.up, out hit2, floorDetectionMaxDistance, layerMask))
            {
                //Debug.DrawRay(ray2Origin, -Vector3.up * hit2.distance, Color.yellow);
                //Debug.Log("Did Hit Floor");

                finalHitPoint.transform.localPosition = ray2Origin;
                finalHitPoint.transform.localPosition += -Vector3.up * hit2.distance;
                finalHitPoint.transform.localPosition = new Vector3(finalHitPoint.transform.localPosition.x, finalHitPoint.transform.localPosition.y + floorOffset, finalHitPoint.transform.localPosition.z);
                GameObject playerObj = UtilFunctions.instance.GetPlayerObj();
                playerObj.transform.localPosition = finalHitPoint.transform.localPosition;
            }
            else
            {
                //Debug.DrawRay(ray2Origin, -Vector3.up * floorDetectionMaxDistance, Color.yellow);
                Debug.Log("Blink Raycast Didn't Hit Floor, Returning Item Use");
                base.UndoAction(); //Undos Use
            }
            Destroy(finalHitPoint);
        }

        protected override void ItemComplete()
        {

        }
    }
}
