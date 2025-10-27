using System;
using UnityEngine;

namespace Shooter
{
    public class GameManager: Singleton<GameManager>
    {
        public LevelParameters LevelParameters;
    }


    public class GameLogic: MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.LevelParameters = new LevelParameters();
        }
    }
}