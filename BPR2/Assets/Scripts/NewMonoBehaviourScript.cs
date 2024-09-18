using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class VRDataListener : MonoBehaviour
{
    private TcpListener server;
    public GameObject Blaser; // Reference to the cube prefab

    async void Start()
    {
        // Start the server to listen for connections
        await StartListener();
    }

    private async Task StartListener()
    {
        // Set up a TCP listener on port 13000
        server = new TcpListener(IPAddress.Any, 13000);
        server.Start();
        Debug.Log("Server started. Listening for connections...");

        while (true)
        {
            // Accept a client connection asynchronously
            TcpClient client = await server.AcceptTcpClientAsync();
            Debug.Log("Client connected.");

            // Process the client in a separate task
            _ = ProcessClient(client);
        }
    }

    private async Task ProcessClient(TcpClient client)
    {NetworkStream stream = client.GetStream();
        byte[] data = new byte[256];
        int bytes = await stream.ReadAsync(data, 0, data.Length);
        string receivedData = Encoding.ASCII.GetString(data, 0, bytes);
        Debug.Log($"Received data: {receivedData}");

        // If the message received is "spawn", instantiate a cube in VR
        if (receivedData.Trim() == "spawn")
        {
            if (Blaser != null)
            {
                Debug.Log("Spawning prefab...");
                Instantiate(Blaser, new Vector3(0, 1, 0), Quaternion.identity);
            }
            else
            {
                Debug.LogError("Blaser prefab is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("Received data does not match 'spawn': " + receivedData);
        }

        client.Close();
    }

    void OnDestroy()
    {
        // Stop the server when the object is destroyed
        server.Stop();
    }
}