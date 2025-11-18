using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Add this to use LINQ
using Newtonsoft.Json;
using TMPro; // Add this for JSON serialization

namespace Shooter
{
    public class SaveGameStatus : MonoBehaviour
    {
        public Player Player;
        public Transform ObjectPoolManagerTransform;
        public string AllSaveGameFileName = "AllSaveGames";
        public ObjectPoolManager ObjectPoolManager;
        public GameObject SavedGameButtonPrefab;
        public Transform SavedGameListContainer;

        public void OpenLoadGameStatusMenu()
        {
            // Clear existing buttons
            foreach (Transform child in SavedGameListContainer)
            {
                Destroy(child.gameObject);
            }

            List<SaveGameEntry> savedGames = JsonWithNewtonsoft.LoadSavedGamesList(AllSaveGameFileName);

            foreach (SaveGameEntry saveGameEntry in savedGames)
            {
                GameObject buttonGameObject = Instantiate(SavedGameButtonPrefab, SavedGameListContainer);
                // Set button text to show relevant save game information
                string displayText = saveGameEntry.SaveGameName;

                buttonGameObject.GetComponentInChildren<TextMeshProUGUI>().text = displayText;
                // Use a lambda to pass the parameter
                buttonGameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    LoadGameStatus(saveGameEntry.PlayerData, saveGameEntry.EnemyDataList);
                });

            }

            SavedGameListContainer.gameObject.SetActive(true);
        }


        public void LoadGameStatus(PlayerData playerData, List<EnemyData> enemyDataList)
        {
            
            SavedGameListContainer.gameObject.SetActive(false);
            if (Player == null)
                Player = FindFirstObjectByType<Player>();

            // Load Player Data
            Vector3 worldPosition = new Vector3(playerData.WorldPositionx, playerData.WorldPositiony, playerData.WorldPositionz);
            Player.transform.position = worldPosition;
            Player.Health = playerData.Health;
            Player.Lives = playerData.Lives;
            Player.Score = playerData.Score;

            // Clear existing enemies in the Object Pool Manager
            var existingEnemies = ObjectPoolManagerTransform.GetComponentsInChildren<Enemy>(false);
            foreach (var enemy in existingEnemies)
            {
                ObjectPoolManager.ReturnToPool(enemy.gameObject);
            }

            // Load Enemy Data
            foreach (var enemyData in enemyDataList)
            {
                // Assuming you have a method to get the enemy prefab by name
                GameObject loadedEnemy = ObjectPoolManager.GetPooledObject(enemyData.PrefabName);

                if (loadedEnemy != null)
                {

                    Enemy enemyComponent = loadedEnemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.health = enemyData.Health;
                    }
                }
                else
                {
                    Debug.LogWarning($"Enemy prefab '{enemyData.PrefabName}' not found in Resources.");
                }
            }
            
        }


        public void SaveStatus()
        {
            if (Player == null)
                Player = FindFirstObjectByType<Player>();

            PlayerData playerData = new PlayerData(
                Player.transform.position,
                Player.Health,
                Player.Lives,
                Player.Score
            );

            // Find all active GameObjects under the Object Pool Manager that have an Enemy component
            var enemies = ObjectPoolManagerTransform.GetComponentsInChildren(typeof(Enemy), false);

            // Create a list to store enemy data
            List<EnemyData> enemyDataList = new List<EnemyData>();

            foreach (var enemy in enemies)
            {

                string prefabName = enemy.gameObject.name;
                Vector3 worldPosition = enemy.transform.position;
                int health = enemy.GetComponent<Enemy>().health;

                // Add the data to the list
                enemyDataList.Add(new EnemyData(prefabName, worldPosition, health));
            }

            // Save the list as JSON using JsonWithNewtonsoft
            string saveGameName = "SaveGameStatus" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            JsonWithNewtonsoft.AddSaveGameToJsonFile(playerData, enemyDataList, saveGameName, AllSaveGameFileName);
        }

    }

    [Serializable]
    public class PlayerData
    {
        public float WorldPositionx { get; set; }
        public float WorldPositiony { get; set; }
        public float WorldPositionz { get; set; }
        public int Health { get; set; }
        public int Lives { get; set; }
        public int Score { get; set; }

        public PlayerData(Vector3 worldPosition, int health, int lives, int score)
        {
            WorldPositionx = worldPosition.x;
            WorldPositiony = worldPosition.y;
            WorldPositionz = worldPosition.z;
            
            Health = health;
            Lives = lives;
            Score = score;
        }
    }
    [Serializable]
    public class EnemyData
    {
        public string PrefabName { get; set; }
        public Vector3 WorldPosition { get; set; }
        public int Health { get; set; }

        public EnemyData(string prefabName, Vector3 worldPosition, int health)
        {
            PrefabName = prefabName;
            WorldPosition = worldPosition;
            Health = health;
        }
    }
}

