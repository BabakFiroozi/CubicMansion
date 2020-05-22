using System;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class DynamicBox : MonoBehaviour
    {
        [SerializeField] Rigidbody _rigidBody;

        void Start()
        {
            Coordinate.Instance.CoordinateChangedEvent += CoordinateChangedEvent;

            CoordinateChangedEvent();
        }

        void OnDestroy()
        {
            Coordinate.Instance.CoordinateChangedEvent -= CoordinateChangedEvent;            
        }

        void CoordinateChangedEvent()
        {
            _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            var upVecType = Coordinate.Instance.UpVecType;
            if (upVecType == VecTypes.UP || upVecType == VecTypes.DOWN)
                _rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionY;
            if (upVecType == VecTypes.FORWARD || upVecType == VecTypes.BACK)
                _rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            if (upVecType == VecTypes.RIGHT || upVecType == VecTypes.BACK)
                _rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionX;
        }
    }
}