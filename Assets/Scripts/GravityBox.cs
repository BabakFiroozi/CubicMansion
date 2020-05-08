using System;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class GravityBox : MonoBehaviour
    {
        [SerializeField] VecTypes _upVec = VecTypes.UP;
        
        Organ _damageable;

        void Start()
        {
            _damageable = gameObject.GetComponent<Organ>();
            _damageable.OnDamage += OnDamage;
        }

        void OnDamage(float dmg, Vector3 pos, GameObject prej)
        {
            print("OnDamage");

            var tr = transform;
            Coordinate.Instance.Change(_upVec);
        }
    }
}