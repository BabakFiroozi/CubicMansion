using System;
using UnityEngine;

namespace CubicMansion
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

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
            if (Input.GetKeyDown(KeyCode.C))
            {
                _follow = !_follow;
            }
        }


        void LateUpdate()
        {
            if(!_follow)
                return;
            
            var playerChar = PlayerCharacter.Instance;

            _tr.position = playerChar.EyeTr.position;
            _tr.rotation = playerChar.EyeTr.rotation;
        }
    }
}