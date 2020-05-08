using System;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Movement))]
    public class PlayerCharacter : MonoBehaviour
    {
        public static PlayerCharacter Instance { get; private set; }

        [SerializeField] Transform _eye;
        [SerializeField] float _horRotationSpeed = 60;
        [SerializeField] float _verRotationSpeed = 90;
        [SerializeField] GameObject _modelObj;
        
        
        GameObject _gameObj;
        Transform _tr;
        
        
        public Movement Movement { get; private set; }

        public Transform Eye => _eye;
        
        
        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _modelObj.SetActive(false);
            _gameObj = gameObject;
            _tr = transform;
            
            Movement = _gameObj.GetComponent<Movement>();

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Cursor.lockState = CursorLockMode.None;
            if (Input.GetKeyDown(KeyCode.V))
                Cursor.lockState = CursorLockMode.Locked;
            if (Input.GetMouseButton(0) && Cursor.lockState == CursorLockMode.None)
                return;

            if (Input.GetButtonDown("Fire1"))
            {
                GetComponent<Unit>().Weapon.TryFire();
            }

            if (!Movement.CoordChanging)
            {
                GoSetMove();
                GoRotate();
            }
        }

        void GoSetMove()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            Vector3 moveDir = new Vector3();
            moveDir.z = Input.GetAxis("Vertical");
            moveDir.x = Input.GetAxis("Horizontal");

            Movement.SetMoveDirection(moveDir);
        }

        void GoRotate()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;
            
            float mouseX = Input.GetAxis("Mouse X");
            
            Quaternion quat = Quaternion.LookRotation(Movement.TurnDirection, Coordinate.Instance.UpVec);
            float angle = mouseX * Time.deltaTime * _horRotationSpeed;
            quat *= Quaternion.AngleAxis(angle, Vector3.up);

            // float halfAng = angle * Mathf.Deg2Rad / 2;
            // float sin = Mathf.Sin(halfAng);
            // quat *= new Quaternion(sin * Vector3.up.x, sin * Vector3.up.y, sin * Vector3.up.z, Mathf.Cos(halfAng));

            Movement.SetTurnDirection(quat * Vector3.forward);
            
            float mouseY = Input.GetAxis("Mouse Y");
            angle = mouseY * Time.deltaTime * _verRotationSpeed;
            _eye.Rotate(new Vector3(-angle, 0, 0), Space.Self);
        }
    }
}