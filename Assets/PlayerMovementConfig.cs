using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Player.Movement
{
    [System.Serializable]
    public class PlayerMovementConfig
    {
        [Header("Jumping and gravity")]
        [SerializeField]
        private bool autoBhop = true;
        [SerializeField]
        private float gravity = 20f;
        [SerializeField]
        private float jumpForce = 6.5f;

        [Header("General physics")]
        [SerializeField]
        private float friction = 6f;
        [SerializeField]
        private float maxSpeed = 6f;
        [SerializeField]
        private float maxVelocity = 50f;
        [SerializeField]
        [Range(30f, 75f)] private float slopeLimit = 45f;

        [Header("Air movement")]
        [SerializeField]
        public bool clampAirSpeed = true;
        [SerializeField]
        public float airCap = 0.4f;
        [SerializeField]
        public float airAcceleration = 12f;
        [SerializeField]
        public float airFriction = 0.4f;

        [Header("Ground movement")]
        public float walkSpeed = 7f;
        public float sprintSpeed = 12f;
        public float acceleration = 14f;
        public float deceleration = 10f;

        [Header("Crouch movement")]
        public float crouchSpeed = 4f;
        public float crouchAcceleration = 8f;
        public float crouchDeceleration = 4f;
        public float crouchFriction = 3f;
    }
}
