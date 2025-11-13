using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json; // Make sure you have Newtonsoft.Json in your project


namespace Shooter
{
   // Example: We'll collect data from objects that have this example component
   // You can replace ExampleComponent with any custom component type you have
  
  
    public class ComponentDataCollector : MonoBehaviour
    {
        // Define which component type to collect (you can also set this in Inspector)
        public string targetComponentName = "ExampleComponent";

        // The file name for the JSON output
        public string outputFileName = "CollectedComponentData.json";

        // This will hold the collected data
        [System.Serializable]
        public class ComponentData
        {
            public string objectName;
            public string componentType;
            public Vector3 position;
            public Dictionary<string, string> extraData; // You can add any key/value info
        }

        public void CollectAndSaveData()
        {
            // Prepare the list
            List<ComponentData> collected = new List<ComponentData>();

            // Find all MonoBehaviours in the scene
            MonoBehaviour[] allComponents = FindObjectsOfType<MonoBehaviour>();

            foreach (var comp in allComponents)
            {
                // If this is the component type we want
                if (comp.GetType().Name == targetComponentName)
                {
                    GameObject go = comp.gameObject;

                    // Create a data entry
                    ComponentData data = new ComponentData
                    {
                        objectName = go.name,
                        componentType = comp.GetType().Name,
                        position = go.transform.position,
                        extraData = new Dictionary<string, string>()
                    };

                    // --- Example: Collect some extra info from the component ---
                    // (You can change this part depending on your component)
                    var type = comp.GetType();
                    var fieldInfos = type.GetFields(); // gets public fields

                    foreach (var field in fieldInfos)
                    {
                        object value = field.GetValue(comp);
                        if (value != null)
                            data.extraData[field.Name] = value.ToString();
                    }

                    collected.Add(data);
                }
            }

            // Convert to JSON string
            string json = JsonConvert.SerializeObject(collected, Formatting.Indented);

            // Define save path (in this example: in project folder)
            string path = Path.Combine(Application.dataPath, outputFileName);

            // Save to file
            File.WriteAllText(path, json);

            Debug.Log($"Data saved to: {path}");
        }

        // Example: Run collection when you press a key (or call manually)
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                CollectAndSaveData();
            }
        }
    }

}
