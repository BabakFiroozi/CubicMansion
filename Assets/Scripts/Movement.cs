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
        [SerializeField] float _coordRotSpeed = 90;

        
        Vector3 _turnPosition;
        
        bool _isTurning;


        public Transform Tr { get; private set; }

        public Vector3 MoveDirection { get; private set; }

        public Vector3 TurnDirection { get; private set; }
        
        

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
            Tr = transform;
            var pos = Tr.position;
            var forw = Tr.forward;
            _turnPosition = pos + forw;
            TurnDirection = forw;
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
            if (CoordChanging)
                GoCoord();
            else
                GoMove();
        }

        void GoMove()
        {
            Vector3 forceDir = MoveDirection;

            Vector3 bodyVel = _rigidBody.velocity;

            float moveForce = _walkForce;
            float moveSpeed = _walkSpeed;

            if (forceDir != Vector3.zero)
            {
                if (bodyVel.magnitude < moveSpeed)
                {
                    Vector3 forceVec = moveForce * Time.fixedDeltaTime * forceDir;
                    forceVec = Quaternion.LookRotation(TurnDirection, Coordinate.Instance.UpVec) * forceVec;
                    _rigidBody.AddForce(forceVec, ForceMode.Impulse);
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

            _rigidBody.angularVelocity = Vector3.zero;
            _fromCoordRot  = Quaternion.LookRotation(TurnDirection, Coordinate.Instance.UpVec);
            _rigidBody.MoveRotation(_fromCoordRot);
        }
        


        Quaternion _fromCoordRot;
        Quaternion _toCoordRot;
        float _needCoordDegree;
        float _coordDegree;
        
        public void ChangeCoord(Vector3 vec)
        {
            CoordChanging = true;
            _toCoordRot =  Quaternion.LookRotation(vec, Coordinate.Instance.UpVec);
            _needCoordDegree = Quaternion.Angle(_fromCoordRot, _toCoordRot);
            
            // print("_needCoordDegree: " + _needCoordDegree);
        }

        void GoCoord()
        {
            float step = Time.fixedDeltaTime * _coordRotSpeed;
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
