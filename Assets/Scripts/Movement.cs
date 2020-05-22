using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Unit))]
    public class Movement : MonoBehaviour
    {
        
        [SerializeField] Rigidbody _rigidBody;
        [SerializeField] float _walkForce;
        [SerializeField] float _runForce;
        [SerializeField] float _walkSpeed;
        [SerializeField] float _runSpeed;
        [SerializeField] float _turnSpeed;
        [SerializeField] float _jumpForce;
        [SerializeField] float _coordRotSpeed = 90;
        [SerializeField] float _coordChangingForce = 10;
        
        readonly RaycastHit[] _checkOnGroundHitResultsArr = new RaycastHit[3];

        Vector3 _turnPosition;
        
        bool _isTurning;
        
        CapsuleCollider _capsuleCollider;
        
        bool _needToAddCoordChangingForce;
        
        bool _moveAsRun;
        
        int _moveForceCounter;

        bool _needJump;


        public Action<bool> LandEvent { get; set; }
        public Action JumpEvent { get; set; }

        public Transform Tr { get; private set; }

        public Vector3 MoveDirection { get; private set; }

        public Vector3 TurnDirection { get; private set; }

        public GameObject GameObj { get; private set; }
        
        public Unit Unit { get; private set; }

        public bool IsOnGround => OnGroundTime > 0;
        
        public float OnGroundTime { get; private set; }


        public void SetMoveDirection(Vector3 dir)
        {
            MoveDirection = dir;
        }

        public void SetTurnDirection(Vector3 dir)
        {
            TurnDirection = dir;
        }

        public void SetTurnPosition(Vector3 pos)
        {
            _turnPosition = pos;
            _isTurning = true;
        }

        void Awake()
        {
            GameObj = gameObject;
            Tr = transform;
            Unit = GetComponent<Unit>();
            _capsuleCollider = GameObj.GetComponent<CapsuleCollider>();
        }

        // Start is called before the first frame update
        void Start()
        {
            _turnPosition = Tr.position + Tr.forward * 3;
            TurnDirection = Tr.forward;
        }

        // Update is called once per frame
        void Update()
        {
        }

        void FixedUpdate()
        {
            CheckOnGround();

            if (CoordChanging)
            {
                GoCoord();
            }
            else
            {
                GoMove();
                BalanceRot();
                GoJump();
            }
        }

        void CheckOnGround()
        {
            var ray = new Ray();
            ray.origin = Tr.position;
            ray.direction = _rigidBody.rotation * Vector3.down;

            const float check_dist = 100;

            float validHeight = _capsuleCollider.height / 2 + .05f;

            var hitResult = new RaycastHit();

            for (int h = 0; h < _checkOnGroundHitResultsArr.Length; ++h)
                _checkOnGroundHitResultsArr[h] = default;

            Physics.RaycastNonAlloc (ray, _checkOnGroundHitResultsArr, check_dist);
            var hitsArr = _checkOnGroundHitResultsArr.ToList().OrderBy(r => r.distance).ToArray();
            foreach(var hit in hitsArr)
            {
                if(hit.collider == null)
                    continue;
                if(hit.collider.gameObject == GameObj)
                    continue;

                hitResult = hit;
                break;
            }

            bool onGround = hitResult.distance > 0 && hitResult.distance <= validHeight;

            if (onGround)
            {
                if (OnGroundTime < 0)
                {
                    OnGroundTime = 0;
                    _needToAddCoordChangingForce = false;
                    LandedOnGround(true);
                }
                OnGroundTime += Time.fixedDeltaTime;
            }
            else
            {
                if (OnGroundTime > 0)
                {
                    LandedOnGround(false);
                    OnGroundTime = 0;
                }
                OnGroundTime -= Time.fixedDeltaTime;
            }

            _rigidBody.angularDrag = 10;

            if (onGround)
            {
                _rigidBody.drag = 10;
            }
            else
            {
                _rigidBody.drag = 0;
                if(_needToAddCoordChangingForce)
                    _rigidBody.AddForce(Coordinate.Instance.DownVec * _coordChangingForce);
            }
            
            // print("drag: " + _rigidBody.drag);
            // print("_needToAddCoordChangingForce: " + _needToAddCoordChangingForce);
        }


        void LandedOnGround(bool land)
        {
            LandEvent?.Invoke(land);
        }

        public void MoveAsRun(bool run)
        {
            _moveAsRun = run;
        }

        void GoMove()
        {
            if (_moveForceCounter > 0)
                _moveForceCounter--;
            
            if (!IsOnGround)
                return;

            Vector3 forceDir = MoveDirection;

            float moveForce = _moveAsRun ? _runForce : _walkForce;
            float moveSpeed = _moveAsRun ? _runSpeed : _walkSpeed;

            if (_moveForceCounter == 0 && forceDir != Vector3.zero)
            {
                Vector3 bodyVel = _rigidBody.velocity;
                var coordUpType = Coordinate.Instance.UpVecType;
                if (coordUpType == VecTypes.UP || coordUpType == VecTypes.DOWN)
                    bodyVel.y = 0;
                if (coordUpType == VecTypes.FORWARD || coordUpType == VecTypes.BACK)
                    bodyVel.z = 0;
                if (coordUpType == VecTypes.RIGHT || coordUpType == VecTypes.LEFT)
                    bodyVel.x = 0;

                if (bodyVel.magnitude < moveSpeed)
                {
                    _moveForceCounter = 3;
                    Vector3 forceVec = moveForce * Time.fixedDeltaTime * forceDir;
                    forceVec = Quaternion.LookRotation(TurnDirection, Coordinate.Instance.UpVec) * forceVec;
                    _rigidBody.AddForce(forceVec, ForceMode.Force);
                }
            }
            
            if (_isTurning)
            {
                Vector3 turnDir = (_turnPosition - Tr.position).normalized;
                float diffAngle = Vector3.Angle(TurnDirection, turnDir);
                float rotSpeed = _turnSpeed * Time.fixedDeltaTime;

                if (diffAngle >= rotSpeed)
                {
                    float ang = Mathf.Deg2Rad * rotSpeed;
                    TurnDirection = Vector3.RotateTowards(TurnDirection, turnDir, ang, 0);
                }
                else
                {
                    TurnDirection = turnDir;
                    _isTurning = false;
                }
            }
        }


        void BalanceRot()
        {
            _rigidBody.angularVelocity = Vector3.zero;
            _fromCoordRot  = Quaternion.LookRotation(TurnDirection, Coordinate.Instance.UpVec);
            _rigidBody.MoveRotation(_fromCoordRot);
        }

        void GoJump()
        {
            if (_needJump)
            {
                _needJump = false;
                float jumpForce = _jumpForce;
                _rigidBody.AddForce(Coordinate.Instance.UpVec * jumpForce, ForceMode.Force);
                JumpEvent?.Invoke();
            }
        }
        
        public bool TryJump()
        {
            if (!IsOnGround)
                return false;
            _needJump = true;
            return true;
        }
        

        
        
        //----------------------------------------------------------------------------------
        
        Quaternion _fromCoordRot;
        Quaternion _toCoordRot;
        float _needCoordDegree;
        float _coordDegree;
        
        public void ChangeCoord(Vector3 vec)
        {
            CoordChanging = true;
            _needToAddCoordChangingForce = true;
            _toCoordRot =  Quaternion.LookRotation(vec, Coordinate.Instance.UpVec);
            _needCoordDegree = Quaternion.Angle(_fromCoordRot, _toCoordRot);
            
            // print("_needCoordDegree: " + _needCoordDegree);
        }

        void GoCoord()
        {
            float stepCoef = Coordinate.Instance.Opposite ? 1.5f : 1.0f;
            float step = Time.fixedDeltaTime * _coordRotSpeed * stepCoef;
            _coordDegree += step;
                
            var rot = Quaternion.RotateTowards(_fromCoordRot, _toCoordRot, _coordDegree);
            _rigidBody.MoveRotation(rot);
            if (_coordDegree >= _needCoordDegree)
            {
                _coordDegree = 0;
                CoordChanging = false;
            }
        }

        public bool CoordChanging { get; private set; }
    }
}
