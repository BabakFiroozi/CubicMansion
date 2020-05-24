using System;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class DynamicBox : MonoBehaviour
    {
        [SerializeField] Rigidbody _rigidBody;

        [SerializeField] Organ _organ;

        [SerializeField] Material _freezeMaterial;
        [SerializeField] Material _freeMaterial;

        [SerializeField] MeshRenderer _meshRenderer;
        
        public bool IsFreezed { get; private set; }
        

        void Start()
        {
            _organ = gameObject.GetComponent<Organ>();
            _organ.DamageEvent = OnDamage;
            Coordinate.Instance.CoordinateChangedEvent += HandleCoordinateChangedEvent;

            HandleCoordinateChangedEvent();
        }

        void OnDestroy()
        {
            Coordinate.Instance.CoordinateChangedEvent -= HandleCoordinateChangedEvent;            
            _organ.DamageEvent = null;
        }

        void HandleCoordinateChangedEvent()
        {
            if(IsFreezed)
                return;
            
            _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            var upVecType = Coordinate.Instance.UpVecType;
            if (upVecType == VecTypes.UP || upVecType == VecTypes.DOWN)
                _rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionY;
            if (upVecType == VecTypes.FORWARD || upVecType == VecTypes.BACK)
                _rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            if (upVecType == VecTypes.RIGHT || upVecType == VecTypes.LEFT)
                _rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionX;
        }
        
        void OnDamage(GameObject prej, float dmg, Vector3 pos, Vector3 norm)
        {
            var projectile  = prej.GetComponent<Projectile>();
            
            if(projectile == null)
                return;
            
            if (projectile.SourceUnit != PlayerCharacter.Instance.Movement.Unit)
                return;

            if (projectile.ProjectileType != ProjectileTypes.DynamicFreeze)
                return;

            IsFreezed = !IsFreezed;

            if (IsFreezed)
            {
                _meshRenderer.material = _freezeMaterial;
                Freeze();
            }
            else
            {
                _meshRenderer.material = _freeMaterial;
                UnFreeze();
            }
        }

        void Freeze()
        {
            _rigidBody.isKinematic = true;
        }

        void UnFreeze()
        {
            _rigidBody.isKinematic = false;
            HandleCoordinateChangedEvent();
        }
    }
}