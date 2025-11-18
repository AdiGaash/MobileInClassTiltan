using System.Collections.Generic; // For lists
using System.IO; // For file operations
using UnityEngine; // For Application.persistentDataPath
using Newtonsoft.Json;
using Shooter;

public static class JsonWithNewtonsoft
{
    

    

    // Generic function to read a list of any type from JSON
    public static List<T> ReadJsonList<T>(string filePath)
    {
        try
        {
            // Read the JSON file content
            string jsonContent = File.ReadAllText(filePath);
        
            // Deserialize the JSON to List<T>
            List<T> items = JsonConvert.DeserializeObject<List<T>>(jsonContent);
        
            Debug.Log($"Successfully read {items.Count} items from {filePath}");
            return items;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading JSON file: {e.Message}");
            return new List<T>();
        }
    }

    public static void AddSaveGameToJsonFile(PlayerData playerData, List<EnemyData> enemyDataList, string saveGameName, string allSaveGameFileName)
    {
        // Define the path to the save file
        string path = Path.Combine(Application.persistentDataPath, allSaveGameFileName + ".json");

        // Check if the file already exists and load its contents
        List<SaveGameEntry> existingEntries = new List<SaveGameEntry>();
        if (File.Exists(path))
        {
            existingEntries = ReadJsonList<SaveGameEntry>(path);
        }

        // Create a new SaveGameEntry and add it to the list of entries
        SaveGameEntry newEntry = new SaveGameEntry(saveGameName, playerData, enemyDataList);
        existingEntries.Add(newEntry);

        // Serialize the updated list back to JSON and write to file
        string json = JsonConvert.SerializeObject(existingEntries, Formatting.Indented);
        File.WriteAllText(path, json);

        Debug.Log($"Save game '{saveGameName}' added to {path}");
    }

    public static List<SaveGameEntry> LoadSavedGamesList(string allSaveGameFileName)
    {
        // Define the path to the save file
        string path = Path.Combine(Application.persistentDataPath, allSaveGameFileName + ".json");

        // Read and return the list of saved games
        return ReadJsonList<SaveGameEntry>(path);
    }
}