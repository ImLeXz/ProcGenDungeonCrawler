using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Player.Movement
{
    public class PlayerView : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField]
        private Transform bodyTransform;

        [Header("Mouse Sensitivity")]
        [SerializeField]
        private float sensitivityMultiplier = 1f;
        [SerializeField]
        private float horizontalSensitivity = 1f;
        [SerializeField]
        private float verticalSensitivity = 1f;

        [Header("Rotation Restrictions")]
        [SerializeField]
        private float minYRotation = -90f;
        [SerializeField]
        private float maxYRotation = 90f;

        [Header("Debug")]
        [SerializeField]
        private bool bDoAiming;

        private Vector3 playerRotation;

        private void Start()
        {
            if (Persistence.PersistenceManager.Instance)
                sensitivityMultiplier = Persistence.PersistenceManager.Instance.GetPlayerSens();
            else
                sensitivityMultiplier = 1.0f;
            ShouldDoAiming(true);
        }

        private void Update()
        {
            if (bDoAiming)
                AimAction();
        }

        private void AimAction()
        {
            // Stop Turning Once Paused
            if (Mathf.Abs(Time.timeScale) <= 0)
                return;

            //Multiply By Time To Be Independant Of Frame Rate Then Multiply By 100 To Make Up For Loss Speed
            float xMovement = Input.GetAxisRaw("Mouse X") * horizontalSensitivity * sensitivityMultiplier;
            float yMovement = -Input.GetAxisRaw("Mouse Y") * verticalSensitivity * sensitivityMultiplier;
            xMovement *= Time.deltaTime * 100;
            yMovement *= Time.deltaTime * 100;

            playerRotation = new Vector3(Mathf.Clamp(playerRotation.x + yMovement, minYRotation, maxYRotation), playerRotation.y + xMovement, playerRotation.z);

            bodyTransform.eulerAngles = Vector3.Scale(playerRotation, new Vector3(0f, 1f, 0f)); //Rotates player body horizontally so that character model turns
            transform.eulerAngles = playerRotation; //Rotates only camera vertically so that player can look up / down without whole player character rotating
        }

        public void UpdateSensitivity()
        {
            sensitivityMultiplier = Persistence.PersistenceManager.Instance.GetPlayerSens();
        }

        public void ShouldDoAiming(bool b)
        {
            bDoAiming = b;
            if (bDoAiming)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
    }
}