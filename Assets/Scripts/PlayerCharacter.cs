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
            // _modelObj.SetActive(false);
            _gameObj = gameObject;
            _tr = transform;
            
            Movement = _gameObj.GetComponent<Movement>();

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.None;
                else if (Cursor.lockState == CursorLockMode.None)
                    Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                GetComponent<Unit>().Weapon.TryFire();
            }

            GoMove();
            GoRotate();
        }

        void GoMove()
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
            
            Vector3 upVec = Coordinate.Instance.UpVec;
            
            Quaternion quat = Quaternion.LookRotation(Movement.TurnDirection, upVec);
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