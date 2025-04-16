using System;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalData
{
    /// <summary>
    /// Grid which contains all the transforms of the blocks
    /// </summary>
    public static List<Transform>[,,] TransformsListGrid3D { get; set; }

    /// <summary>
    /// Grid which contains all the prefabricated positions
    /// </summary>
    public static Vector3[,,] FixedPositionGrid { get; private set; }

    /// <summary>
    /// Size of the grid
    /// </summary>
    public static int GridSize { get; private set; } = 0;

    /// <summary>
    /// Cubic size of the grid
    /// </summary>
    public static int GridSizeCubic { get; private set; } = 0;

    /// <summary>
    /// Generate a transform grid and a position grid by fixed size
    /// </summary>
    public static void SetGrid(int gridSize)
    {
        //Initialize grid
        TransformsListGrid3D = new List<Transform>[gridSize, gridSize, gridSize];
        FixedPositionGrid = new Vector3[gridSize, gridSize, gridSize];
        GridSize = gridSize;
        GridSizeCubic = gridSize * gridSize * gridSize;

        //Generate transform grid's list
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    TransformsListGrid3D[x, y, z] = new List<Transform>();
                }
            }
        }

        //Generate position grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    FixedPositionGrid[x, y, z] = new Vector3(x * BlockDistance, y * BlockDistance, z * BlockDistance);
                }
            }
        }
    }

    /// <summary>
    /// Distance between blocks
    /// </summary>
    public static float BlockDistance { get; private set; } = 2f;
}
