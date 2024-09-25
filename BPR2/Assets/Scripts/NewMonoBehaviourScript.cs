using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class VRDataListener : MonoBehaviour
{
    private TcpListener server;
    
    public GameObject Cube;  // Square
    public GameObject Cylinder;  // Polygon (Cylinder)

    async void Start()
    {
        // Start the TCP listener to listen for connections
        await StartListener();
    }

    private async Task StartListener()
    {
        
        server = new TcpListener(IPAddress.Any, 13000);
        server.Start();
        Debug.Log("Server started. Listening for connections...");

        while (true)
        {
            
            TcpClient client = await server.AcceptTcpClientAsync();
            Debug.Log("Client connected.");

            
            _ = ProcessClient(client);
        }
    }

    private async Task ProcessClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] data = new byte[1024];
        int bytes = await stream.ReadAsync(data, 0, data.Length);
        string receivedData = Encoding.ASCII.GetString(data, 0, bytes);
        Debug.Log($"Received data: {receivedData}");
        
        try
        {
            // Assuming receivedData is an array of DesignElement
            var elements = JsonConvert.DeserializeObject<List<DesignElement>>(receivedData);

            foreach (var element in elements)
            {
                
                SpawnElement(element);
            }
        }
        catch (JsonException ex)
        {
            Debug.LogError("Failed to parse JSON: " + ex.Message);
        }
        
        
        
        
        // UNCOMMENT THIS TO SPAWN ITEMS AT THEIR SPECIFIC COORDINATES
        // try
        // {
        //     // Assuming receivedData is an array of DesignElement
        //     var elements = JsonConvert.DeserializeObject<List<DesignElement>>(receivedData);
        //
        //     foreach (var element in elements)
        //     {
        //         GameObject modelToSpawn = null;
        //         if (element.ElementName == "Square1")
        //         {
        //             modelToSpawn = Resources.Load<GameObject>("Cube");
        //         }
        //         else if (element.ElementName == "Polygon1")
        //         {
        //             modelToSpawn = Resources.Load<GameObject>("Cylinder");
        //         }
        //
        //         if (modelToSpawn != null)
        //         {
        //             Vector3 spawnPosition = new Vector3(element.X, 1, element.Y); // Adjust Y as necessary
        //             Instantiate(modelToSpawn, spawnPosition, Quaternion.identity);
        //             Debug.Log($"Spawned {element.ElementName} at {spawnPosition}");
        //         }
        //         else
        //         {
        //             Debug.LogWarning("Model to spawn not found for " + element.ElementName);
        //         }
        //     }
        // }
        // catch (JsonException ex)
        // {
        //     Debug.LogError("Failed to parse JSON: " + ex.Message);
        // }

        client.Close();
    }




    private GameObject GetPrefabByName(string name)
    {
        if (name.StartsWith("Square"))
        {
            return Resources.Load<GameObject>("Cube"); 
        }
        else if (name.StartsWith("Polygon"))
        {
            return Resources.Load<GameObject>("Cylinder"); 
        }
    
        return null; 
    }




    private void SpawnElement(DesignElement element)
    {
        GameObject prefabToSpawn = GetPrefabByName(element.ElementName); 

        if (prefabToSpawn != null)
        {
            
            Vector3 cameraPosition = Camera.main.transform.position;

           
            float spawnX = Mathf.Clamp(cameraPosition.x + 1.0f + Random.Range(-2.0f, 2.0f), -50, 50); // Offset from camera with random value
            float spawnZ = Mathf.Clamp(cameraPosition.z + 1.0f + Random.Range(-2.0f, 2.0f), -50, 50); // Offset from camera with random value
        
            Vector3 spawnPosition = new Vector3(spawnX, 0, spawnZ); // Set Y to 0 for the canvas height

           
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            
            spawnedObject.transform.localScale = new Vector3(150, 150, 150); 

            
            Debug.Log($"Spawned {element.ElementName} at {spawnPosition}");
        }
        else
        {
            Debug.LogWarning("Model to spawn not found for " + element.ElementName);
        }
    }







    void OnDestroy()
    {
       
        server.Stop();
    }
}


[System.Serializable]
public class DesignElement
{
    public string ElementName { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
}
