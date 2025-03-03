using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Node position is at the center
public struct Node
{
    public bool isBlocked;
    public Vector3 worldPosition;
}

public class PathfindingManager : MonoBehaviour
{
    [SerializeField]
    public LayerMask blockingLayer;
    public Transform gridBottomLeft;
    public Transform gridTopRight;
    public float nodeSize;

    private Node[,] _grid;
    private Vector2 _gridWorldSize;
    private int _gridSizeX;
    private int _gridSizeY;

    // Start is called before the first frame update
    void Start()
    {
        _gridWorldSize = new Vector2(gridTopRight.position.x - gridBottomLeft.position.x, gridTopRight.position.y - gridBottomLeft.position.y);

        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / nodeSize);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / nodeSize);
        InitialiseGrid();
        CalculateGrid();
    }

    private void InitialiseGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                _grid[x, y] = new Node();
            }
        }
    }

    private void CalculateGrid()
    {
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPosition = gridBottomLeft.position + Vector3.right * (x * nodeSize + (nodeSize / 2.0f)) + Vector3.up * (y * nodeSize + (nodeSize / 2.0f));

                bool isBlocked = (Physics2D.BoxCast(worldPosition, Vector2.one * nodeSize, 0, Vector2.up, 0, blockingLayer));
                
                _grid[x, y].isBlocked = isBlocked;
                _grid[x, y].worldPosition = worldPosition;
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        if(worldPosition.x < gridBottomLeft.position.x 
            || worldPosition.x > gridTopRight.position.x 
            || worldPosition.y < gridBottomLeft.position.y
            || worldPosition.y > gridTopRight.position.y) 
        {
            Debug.LogWarning("Position out of grid");
            return _grid[0, 0]; 
        }

        float percentX = (worldPosition.x - gridBottomLeft.position.x) / _gridWorldSize.x;
        float percentY = (worldPosition.y - gridBottomLeft.position.y) / _gridWorldSize.y;

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

        return _grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (_grid != null)
        {
            foreach (Node node in _grid)
            {
                Gizmos.color = node.isBlocked ? Color.red : Color.green;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSize - 0.1f));
            }
        }
    }
}
