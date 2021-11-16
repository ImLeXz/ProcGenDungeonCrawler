using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Player.Movement
{
    public class PlayerMovementTest : MonoBehaviour
    {
        public CharacterController controller;

        [Header("Physics Settings")]
        [SerializeField]
        private Vector3 colliderSize = new Vector3(1f, 2f, 1f);
        [SerializeField]
        private float gravityStrength = -9.81f;
        [SerializeField]
        private float movementSpeed = 5f;
        [SerializeField]
        private float jumpHeight = 250f;

        [Header("Player Transforms")]
        [SerializeField]
        private Transform viewTransform;
        [SerializeField]
        private Transform playerRotationTransform;

        [Header("Grounder Settings")]
        [SerializeField]
        private Transform groundCheckTransform;
        [SerializeField]
        private float groundDistance = 0.2f;
        [SerializeField]
        private LayerMask groundMask;

        [Header("Crouch Settings")]
        [SerializeField]
        private float crouchingHeightMultiplier = 0.5f;
        [SerializeField]
        private float crouchingSpeedMultiplier = 0.5f;

        [Header("Sprint Settings")]
        [SerializeField]
        private float sprintSpeedMultiplier = 1.25f;

        [Header("Movement Toggles")]
        [SerializeField]
        private bool crouchingEnabled = true;
        [SerializeField]
        private bool jumpingEnabled = true;
        [SerializeField]
        private bool sprintingEnabled = true;


        private Vector3 initialScale;
        private Vector3 velocity;
        private bool isGrounded;

        private void Start()
        {
            initialScale = this.transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            MovePlayer();
        }

        //Used To Draw The Size Of The Player Collider
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, colliderSize);
        }

        private void MovePlayer()
        {
            //Grounded Check
            isGrounded = Physics.CheckSphere(groundCheckTransform.transform.position, groundDistance, groundMask);

            //Reset Velocity If On Ground
            if (isGrounded && velocity.y < 0)
                velocity.y = -2.0f;

            float verticalAxis = Input.GetAxisRaw("Vertical");
            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            float crouchMult = 1.0f;
            float sprintMult = 1.0f;

            if (Input.GetButton("Crouch"))
            {
                crouchMult = crouchingSpeedMultiplier;
                this.transform.localScale = new Vector3(initialScale.x, initialScale.y * crouchingHeightMultiplier, initialScale.z);
            }
            else
            {
                crouchMult = 1.0f;
                this.transform.localScale = initialScale;
            }

            if (Input.GetButton("Sprint"))
                sprintMult = sprintSpeedMultiplier;
            else
                sprintMult = 1.0f;


            //Movement Physics
            Vector3 move = transform.right * horizontalAxis + transform.forward * verticalAxis;
            controller.Move(move * movementSpeed * crouchMult * sprintMult * Time.deltaTime);

            //Jumping Based On Jump Height Equation
            if (Input.GetButtonDown("Jump") && isGrounded)
                velocity.y = Mathf.Sqrt(jumpHeight * crouchingHeightMultiplier * -2f * gravityStrength);



            //Gravity Physics
            velocity.y += gravityStrength * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

        }
    }
}
