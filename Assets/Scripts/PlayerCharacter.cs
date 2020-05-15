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

        [SerializeField] GameObject _defaultWeaponPrefab;
        
        
        GameObject _gameObj;
        Transform _tr;
        
        public Transform EyeTr { get; private set; }
        
        
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
            EyeTr = _eye.transform;
            
            Movement = _gameObj.GetComponent<Movement>();

            Cursor.lockState = CursorLockMode.Locked;

            var obj = Instantiate(_defaultWeaponPrefab);
            Movement.Unit.AddWeapon(obj.GetComponent<Weapon>());
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
                GetComponent<Unit>().CurrentWeapon.TryFire();
            }

            Movement.MoveAsRun(Input.GetButton("Run"));
            
            if(Input.GetButtonDown("Jump"))
                Movement.TryJump();

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
            float horAngle = mouseX * Time.deltaTime * _horRotationSpeed;
            quat *= Quaternion.AngleAxis(horAngle, Vector3.up);

            // float halfAng = angle * Mathf.Deg2Rad / 2;
            // float sin = Mathf.Sin(halfAng);
            // quat *= new Quaternion(sin * Vector3.up.x, sin * Vector3.up.y, sin * Vector3.up.z, Mathf.Cos(halfAng));

            Movement.SetTurnDirection(quat * Vector3.forward);

            float lastAngle = Vector3.SignedAngle(_eye.forward, _tr.forward, _tr.right);
            float mouseY = Input.GetAxis("Mouse Y");
            float verAngle = mouseY * Time.deltaTime * _verRotationSpeed;
            const float angle_limit = 75;
            if (lastAngle + verAngle <= angle_limit && lastAngle + verAngle >= -angle_limit)
            {
                _eye.Rotate(new Vector3(-verAngle, 0, 0), Space.Self);
            }
        }
    }
}