using System;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] ProjectileTypes _projectileType;
        [SerializeField] float _speed = 0;
        [SerializeField] float _damage = 0;
        [SerializeField] float _gravity = 0;
        [SerializeField] float _maxDistance = 0;
        [SerializeField] int _bulletsCount = 0;

        public Weapon Weapon { get; private set; }

        public ProjectileTypes ProjectileType => _projectileType;
        
        
        public Action<GameObject, Vector3, Vector3> HitEvent { get; private set; }

        Transform _tr;
        float _distance;
        Vector3 _initPos;
        GameObject _gameObj;

        public Unit SourceUnit { get; private set; }
	
        readonly RaycastHit[] _hitsArr = new RaycastHit[5];

        void Awake()
        {
            _gameObj = gameObject;
            _tr = transform;
            _initPos = _tr.position;
            _distance = 0;
        }

        // Use this for initialization
        void Start ()
        {
	
        }
	
        // Update is called once per frame
        void Update ()
        {
        }

        void FixedUpdate()
        {
            Vector3 dir = _tr.forward;
            Ray ray = new Ray (_tr.position, dir);

            float dist = _speed * Time.fixedDeltaTime;

            _distance += dist;

            Physics.RaycastNonAlloc (ray, _hitsArr, dist);
            foreach(var hit in _hitsArr)
            {
                if(hit.collider == null)
                    continue;
                
                var obj = hit.collider.gameObject;
                if (obj == SourceUnit.gameObject)
                    continue;
                Hit(obj, hit.point, hit.normal);
                return;
            }

            if(dist > _maxDistance)
            {
                Destroy(gameObject);
                return;
            }

            _tr.position += dir * dist;

            if(_gravity != 0)
            {
                Vector3 pos = _tr.position;
                pos.y -= _gravity * Time.fixedDeltaTime;
                _tr.position = pos;
            }
        }

        public void SetSourceUnit(Unit unit)
        {
            SourceUnit = unit;
            Weapon = unit.CurrentWeapon;
        }

        void Hit(GameObject objHit, Vector3 pos, Vector3 normal)
        {
            HitEvent?.Invoke(objHit, pos, normal);
            
            var organ = objHit.GetComponent<Organ>();
            if (organ != null)
            {
                organ.DoDamage(_gameObj, _damage, pos, normal);
            }

            Destroy(_gameObj);
        }
    }

    public enum ProjectileTypes
    {
        Graviter,
        BuilderRemove,
        BuilderAdd
    }
}