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
            _damageable.OnDamageEvent += OnDamage;
        }

        void OnDamage(float dmg, Vector3 pos, GameObject prej)
        {
            print("OnDamage");

            var projectile  = prej.GetComponent<Projectile>();
            
            if(projectile == null)
                return;

            var unit = projectile._sourceUnit;
            
            if(unit != PlayerCharacter.Instance.Movement.Unit)
                return;

            float dist = (PlayerCharacter.Instance.Movement.Tr.position - pos).magnitude;
            Coordinate.Instance.Change(_upVec, dist);
        }
    }
}