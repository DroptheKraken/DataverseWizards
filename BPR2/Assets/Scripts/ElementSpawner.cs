using System.Collections.Generic;
using UnityEngine;

public class ElementSpawner
{
    private GameObject cubePrefab;
    private GameObject cylinderPrefab;

    public ElementSpawner(GameObject cube, GameObject cylinder)
    {
        this.cubePrefab = cube;
        this.cylinderPrefab = cylinder;
    }

    public void SpawnElementAtCoordinates(DesignElement element, Dimensions dimensions, float wallDistanceX, float wallDistanceZ)
    {
        GameObject prefabToSpawn = GetPrefabByName(element.ElementName);

        if (prefabToSpawn != null)
        {
            // Assuming the incoming element coordinates are in pixels
            float pixelX = element.X; // X coordinate from JSON
            float pixelZ = element.Z; // Z coordinate from JSON

            // Scale down the coordinates from pixels to meters (1:100 scaling)
            float scaleFactor = 0.01f;
            float scaledX = pixelX * scaleFactor; // Convert X from pixels to meters
            float scaledZ = pixelZ * scaleFactor; // Convert Z from pixels to meters
            
            // Adjust to spawn with (0,0,0) as top-left
            Vector3 spawnPosition = new Vector3(scaledX, 1, wallDistanceZ - scaledZ);

            // Adjust scale
            prefabToSpawn.transform.localScale = new Vector3(100, 100, 100);

            Quaternion rotation = Quaternion.Euler(0, 180, 0);
            
            Object.Instantiate(prefabToSpawn, spawnPosition, rotation);

            Debug.Log($"Spawned {element.ElementName} at {spawnPosition} with a 180-degree Y-axis flip.");
        }
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
}