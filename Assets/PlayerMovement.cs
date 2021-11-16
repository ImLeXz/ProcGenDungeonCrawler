using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Player.Movement
{
    public class PlayerMovement : MonoBehaviour, IPlayerMovement
    {
        [Header("Physics Settings")]
        [SerializeField]
        private Vector3 colliderSize = new Vector3(1f, 2f, 1f);
        [SerializeField]
        private float weight = 75f;
        [SerializeField]
        private float rigidbodyPushForce = 2f;
        [SerializeField]
        private bool solidCollider = false;

        [Header("View Settings")]
        [SerializeField]
        private Transform viewTransform;
        [SerializeField]
        private Transform playerRotationTransform;

        [Header("Crouch Settings")]
        [SerializeField]
        private float crouchingHeightMultiplier = 0.5f;
        [SerializeField]
        private float crouchingSpeed = 10f;

        [Header("Movement Toggles")]
        [SerializeField]
        private bool crouchingEnabled = true;
        [SerializeField]
        private bool jumpingEnabled = true;

        [Header("Movement Config")]
        [SerializeField]
        public PlayerMovementConfig movementConfig;

        private GameObject _groundObject;
        private Vector3 _baseVelocity;
        private Collider _collider;
        private Vector3 _angles;
        private Vector3 _startPosition;
        private GameObject _colliderObject;
        private GameObject _cameraWaterCheckObject;
        private CameraWaterCheck _cameraWaterCheck;

        private PlayerMoveData _moveData = new PlayerMoveData();
        //private SurfController _controller = new SurfController();

        private Rigidbody rb;

        private List<Collider> triggers = new List<Collider>();
        private int numberOfTriggers = 0;

        private bool underwater = false;

        ///// Properties /////

        public MoveType moveType { get { return MoveType.Walk; } }
        public PlayerMovementConfig moveConfig { get { return movementConfig; } }
        public PlayerMoveData moveData { get { return _moveData; } }
        public new Collider collider { get { return _collider; } }

        public GameObject groundObject
        {

            get { return _groundObject; }
            set { _groundObject = value; }

        }

        public Vector3 baseVelocity { get { return _baseVelocity; } }

        public Vector3 forward { get { return viewTransform.forward; } }
        public Vector3 right { get { return viewTransform.right; } }
        public Vector3 up { get { return viewTransform.up; } }

        Vector3 prevPosition;

        private bool canCrouch = true;
        private float defaultHeight;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //Used To Draw The Size Of The Player Collider
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, colliderSize);
        }
    }
}
