using System;
using UnityEngine;

namespace Shooter
{
    public class Player : MonoBehaviour
    {
        public int Health = 100;
        public int Lives = 3;
        public int Score = 0;


        private void Start()
        {
            Score = 0;
        }
    }
}