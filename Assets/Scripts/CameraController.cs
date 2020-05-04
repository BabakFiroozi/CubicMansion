using System;
using UnityEngine;

namespace CubicMansion
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [SerializeField] Vector3 _offset;
        [SerializeField] float _rotationSpeed;

        Transform _tr;

        bool _follow = true;

        void Start()
        {
            Instance = this;
            _tr = transform;
        }

        Quaternion _rot;
        float _mouseY;
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                _follow = !_follow;
            }
        }


        void LateUpdate()
        {
            if(!_follow)
                return;
            
            var playerChar = PlayerCharacter.Instance;
            
            _tr.position = playerChar.Eye.transform.position;
            _tr.rotation = playerChar.Eye.transform.rotation;
        }

        float halfAng;
    }
}