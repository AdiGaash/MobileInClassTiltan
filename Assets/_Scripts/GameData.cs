[System.Serializable]
public class GameData
{
    // Unique identifier for the game data entry
    public string Id;

    // Name or title of the game data
    public string Name;
    

    // Timestamp for when the data was created or last modified
    public System.DateTime Timestamp;
    
    
    // Default constructor
    public GameData()
    {
        Id = System.Guid.NewGuid().ToString();
        Timestamp = System.DateTime.UtcNow;
    }
    
}