using System;
using System.Collections.Generic;
using UnityEngine;

namespace CubicMansion
{
    public class Levels : MonoBehaviour
    {
        public static Levels Instance { get; private set; }

        [SerializeField] List<GameObject> _buildBoxes;

        public GameObject GetBuildBox(BuildBoxTypes type)
        {
            var obj = _buildBoxes[(int) type];
            return obj;
        }
        
        void Start()
        {
            Instance = this;
        }
    }
}