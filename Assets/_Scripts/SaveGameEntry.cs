using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class SaveGameEntry
    {
        public string SaveGameName { get; set; }
        public List<EnemyData> EnemyDataList { get; set; }

        public PlayerData PlayerData { get; set; }

        public SaveGameEntry(string saveGameName, PlayerData playerData, List<EnemyData> enemyDataList)
        {

            SaveGameName = saveGameName;
            EnemyDataList = enemyDataList;
            PlayerData = playerData;
        }

    }
}