using System;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed = 0;
        [SerializeField] float _damage = 0;
        [SerializeField] float _gravity = 0;
        [SerializeField] float _maxDistance = 0;
        [SerializeField] int _bulletsCount = 0;

        public Action<GameObject> OnHitEvent { get; private set; }

        Transform _tr;
        float _distance;
        Vector3 _initPos;

        public Unit _sourceUnit { get; private set; }
	
        RaycastHit[] _hitsArr = new RaycastHit[5];

        void Awake()
        {
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
                if (obj == _sourceUnit.gameObject)
                    continue;
                Hit (obj);
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
            _sourceUnit = unit;
        }

        void Hit(GameObject hitObj)
        {
            OnHitEvent?.Invoke(hitObj);

            var organ = hitObj.GetComponent<Organ>();
            if (organ != null)
            {
                organ.DoDamage(_damage, Vector2.zero, gameObject);
            }

            Destroy(gameObject);
        }
    }
}