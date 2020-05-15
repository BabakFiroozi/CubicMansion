using System;
using System.Collections;
using System.Collections.Generic;
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
        
        RaycastHit[] _checkOnGroundHitResultsArr = new RaycastHit[5];

        Vector3 _turnPosition;
        
        bool _isTurning;
        
        CapsuleCollider _capsuleCollider;
        
        bool _needToAddCoordChangingForce;
        
        bool _moveAsRun;

        public Action<bool> LandedOnGroundEvent { get; set; }
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
            var pos = Tr.position;
            var forw = Tr.forward;
            _turnPosition = pos + forw;
            TurnDirection = forw;

            _capsuleCollider = GameObj.GetComponent<CapsuleCollider>();
            Unit = GameObj.GetComponent<Unit>();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        void FixedUpdate()
        {
            CheckOnGround();

            if (CoordChanging)
                GoCoord();
            else
                GoMove();
        }

        void CheckOnGround()
        {
            var ray = new Ray();
            ray.origin = Tr.position;
            ray.direction = _rigidBody.rotation * Vector3.down;

            const float checkDist = 100;

            float validHeight = _capsuleCollider.height / 2 + .03f;

            var hitResult = new RaycastHit();

            for (int h = 0; h < _checkOnGroundHitResultsArr.Length; ++h)
                _checkOnGroundHitResultsArr[h] = default;

            Physics.RaycastNonAlloc (ray, _checkOnGroundHitResultsArr, checkDist);
            foreach(var hit in _checkOnGroundHitResultsArr)
            {
                if(hit.collider == null)
                    continue;
                if(hit.collider.gameObject == GameObj)
                    continue;

                hitResult = hit;
                break;
            }

            bool isOnGround = hitResult.distance > 0 && hitResult.distance <= validHeight;

            if (isOnGround)
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

            if (isOnGround)
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
            LandedOnGroundEvent?.Invoke(land);
        }
        

        void GoMove()
        {
            if (IsOnGround)
            {
                Vector3 forceDir = MoveDirection;

                float moveForce = _moveAsRun ? _runForce : _walkForce;
                float moveSpeed = _moveAsRun ? _runSpeed : _walkSpeed;

                if (forceDir != Vector3.zero)
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

            _rigidBody.angularVelocity = Vector3.zero;
            _fromCoordRot  = Quaternion.LookRotation(TurnDirection, Coordinate.Instance.UpVec);
            _rigidBody.MoveRotation(_fromCoordRot);
        }

        public void MoveAsRun(bool run)
        {
            _moveAsRun = run;
        }

        public void TryJump()
        {
            if (!IsOnGround)
                return;
            Jump();
        }

        void Jump()
        {
            float jumpForce = _jumpForce;
            _rigidBody.AddForce(Coordinate.Instance.UpVec * jumpForce, ForceMode.Force);
            JumpEvent?.Invoke();
        }


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
            float stepCoef = Coordinate.Instance.Opposite ? 2 : 1;
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
