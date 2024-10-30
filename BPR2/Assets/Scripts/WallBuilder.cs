using System.Collections.Generic;
using UnityEngine;

public class WallBuilder : MonoBehaviour
{
    public GameObject wallPrefab; // Prefab for walls
    public float wallThickness = 0.1f; // Thickness of the walls
    public float scaleFactorX = 0.01f; // Scaling factor for X
    public float scaleFactorY = 0.01f; // Scaling factor for Y

    // HashSet to track spawned wall positions
    private HashSet<Vector3> spawnedWallPositions = new HashSet<Vector3>();

    public (float wallDistanceX, float wallDistanceZ) BuildStore(Dimensions dimensions)
    {
        Vector3 wallScaleX = new Vector3(dimensions.X * scaleFactorX, dimensions.Y * scaleFactorY, wallThickness); // Thin wall in X direction
        Vector3 wallScaleZ = new Vector3(wallThickness, dimensions.Y * scaleFactorY, dimensions.Z * scaleFactorX); // Thin wall in Z direction

        // Build walls, with 0,0,0 as the top-left corner of the box
        CreateWall(wallScaleX, new Vector3(dimensions.X * scaleFactorX / 2, (dimensions.Y * scaleFactorY) / 2, dimensions.Z * scaleFactorX));
        CreateWall(wallScaleX, new Vector3(dimensions.X * scaleFactorX / 2, (dimensions.Y * scaleFactorY) / 2, 0));
        CreateWall(wallScaleZ, new Vector3(dimensions.X * scaleFactorX, (dimensions.Y * scaleFactorY) / 2, dimensions.Z * scaleFactorX / 2));
        CreateWall(wallScaleZ, new Vector3(0, (dimensions.Y * scaleFactorY) / 2, dimensions.Z * scaleFactorX / 2));

        // Calculate distances
        float wallDistanceX = dimensions.X * scaleFactorX; // Total distance on the X axis
        float wallDistanceZ = dimensions.Z * scaleFactorX; // Total distance on the Z axis

        return (wallDistanceX, wallDistanceZ);
    }

    private void CreateWall(Vector3 scale, Vector3 position)
    {
        // Check if the wall has already been spawned at this position
        if (!spawnedWallPositions.Contains(position))
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = scale;
            wall.transform.position = position;

            // Add the position to the HashSet to track spawned walls
            spawnedWallPositions.Add(position);
        }
        else
        {
            Debug.Log($"Wall already spawned at position: {position}");
        }
    }
}
