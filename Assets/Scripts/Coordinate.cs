using UnityEngine;

namespace CubicMansion
{
    public class Coordinate : MonoBehaviour
    {
        public static Coordinate Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Init()
        {
            ForwardVec = Vector3.forward;
            RightVec = Vector3.right;
            UpVec = Vector3.up;
        }


        public Vector3 ForwardVec { get; private set; } 
        public Vector3 RightVec { get; private set; }
        public Vector3 UpVec { get; private set; } 

        
        public void Change(Vector3 forward, Vector3 right, Vector3 up)
        {
            ForwardVec = forward;
            RightVec = right;
            UpVec = up;

            var tr = PlayerCharacter.Instance.Movement.Tr;
            // tr.forward = forward;
            // tr.right = right;
            tr.up = up;
            PlayerCharacter.Instance.Movement.SetTurnDirection(forward);

            Physics.gravity = -UpVec;

            print("Gravity Changed...");
        }
        
    }
}