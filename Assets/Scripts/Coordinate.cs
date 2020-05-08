using UnityEngine;

namespace CubicMansion
{
    
    public class Coordinate : MonoBehaviour
    {
       
        public static Coordinate Instance { get; private set; }
        
        public VecTypes UpVecType { get; private set; }
        
        
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
            UpVecType = VecTypes.UP;
        }


        public Vector3 ForwardVec { get; private set; } 
        public Vector3 RightVec { get; private set; }
        public Vector3 UpVec { get; private set; } 

        
        public void Change(VecTypes vecType)
        {
            print("<color=yellow>Gravity Changed...</color>");

            Vector3 forward = PlayerCharacter.Instance.Movement.TurnDirection;

            Vector3 up = UpVec;
            if(vecType == VecTypes.UP)
                up = Vector3.up;
            if(vecType == VecTypes.DOWN)
                up = Vector3.down;
            if(vecType == VecTypes.RIGHT)
                up = Vector3.right;
            if(vecType == VecTypes.LEFT)
                up = Vector3.left;
            if(vecType == VecTypes.FORWARD)
                up = Vector3.forward;
            if(vecType == VecTypes.BACK)
                up = Vector3.back;
            
            float diffAngle = -Vector3.SignedAngle(forward, -up, UpVec);
            print("diff: " + diffAngle);

            ForwardVec = UpVec;
            UpVec = up;
            RightVec = Vector3.Cross(ForwardVec, UpVec);
            
            // tr.forward = forward;
            // tr.right = right;
            // tr.up = up;

            // Vector3 vec = Quaternion.AngleAxis(-diffAngle, UpVec) * forward;
            // PlayerCharacter.Instance.Movement.SetTurnDirection(vec);

            Vector3 vec = Quaternion.AngleAxis(diffAngle, UpVec) * ForwardVec;
            PlayerCharacter.Instance.Movement.ChangeCoord(vec);
            PlayerCharacter.Instance.Movement.SetTurnDirection(vec);

            
            Physics.gravity = -UpVec;
        }
        
    }
}