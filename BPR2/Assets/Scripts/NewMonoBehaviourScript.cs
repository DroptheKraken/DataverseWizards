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
    
    private WallBuilder wallBuilder;
    private ElementSpawner elementSpawner;

    async void Start()
    {
        wallBuilder = new WallBuilder();
        elementSpawner = new ElementSpawner(Cube, Cylinder);

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
            // Deserialize into the StoreData class
            var storeData = JsonConvert.DeserializeObject<StoreData>(receivedData);
            
            // Build walls and get distances
            var (wallDistanceX, wallDistanceZ) = wallBuilder.BuildStore(storeData.dimensions);
            
            // Iterate through element positions and spawn them
            foreach (var element in storeData.ElementPositions)
            {
                elementSpawner.SpawnElementAtCoordinates(element, storeData.dimensions, wallDistanceX, wallDistanceZ);
            }
        }
        catch (JsonException ex)
        {
            Debug.LogError("Failed to parse JSON: " + ex.Message);
        }

        client.Close();
    }

    void OnDestroy()
    {
        server.Stop();
    }
}

[System.Serializable]
public class StoreData
{
    public Dimensions dimensions { get; set; }
    public List<DesignElement> ElementPositions { get; set; }
}

[System.Serializable]
public class Dimensions
{
    public float X { get; set; }
    public float Z { get; set; }
    public float Y { get; set; }
}

[System.Serializable]
public class DesignElement
{
    public string ElementName { get; set; }
    public float X { get; set; }
    public float Z { get; set; }
}