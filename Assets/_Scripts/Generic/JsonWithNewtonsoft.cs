using System.Collections.Generic; // For lists
using System.IO; // For file operations
using UnityEngine; // For Application.persistentDataPath
using Newtonsoft.Json; // For JSON serialization and deserialization

[System.Serializable]
public class GameData
{
    // Unique identifier for the game data entry
    public string Id { get; set; }

    // Name or title of the game data
    public string Name { get; set; }

    // A generic value that can store various types of game-related information
    public object Value { get; set; }

    // Timestamp for when the data was created or last modified
    public System.DateTime Timestamp { get; set; }

    // Optional metadata or additional properties
    public Dictionary<string, string> Metadata { get; set; }

    // Default constructor
    public GameData()
    {
        Id = System.Guid.NewGuid().ToString();
        Timestamp = System.DateTime.UtcNow;
        Metadata = new Dictionary<string, string>();
    }

    // Constructor with parameters
    public GameData(string name, object value) : this()
    {
        Name = name;
        Value = value;
    }
}

public static class JsonWithNewtonsoft
{
    // This function will save a list of GameData objects as a JSON file
    public static void SaveGameDataList(List<GameData> dataList, string fileName)
    {
        // Convert the list of objects directly to JSON format using Newtonsoft.Json
        string json = JsonConvert.SerializeObject(dataList, Formatting.Indented); // 'Indented' makes the output human-readable

        // Create a file path (this saves the file in the persistent data path of the game)
        string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
        
        // Write the JSON data to a file
        File.WriteAllText(path, json);
        
        Debug.Log("List of data saved as JSON to: " + path); // For debugging purposes
    }
    
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
}

 


