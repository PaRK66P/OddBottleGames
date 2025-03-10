using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Node position is at the center
public class Node
{
    public bool isBlocked;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    public int gCost; // Travel distance
    public int hCost; // Distance from goal
    public Node parentNode;

    public void InitialiseNode(bool isBlockedValue, Vector3 worldPositionValue, int gridXValue, int gridYValue)
    {
        this.isBlocked = isBlockedValue;
        this.worldPosition = worldPositionValue;
        this.gridX = gridXValue;
        this.gridY = gridYValue;
    }

    public int GetFCost
    {
        get { return gCost + hCost; }
    }
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

    private List<Node> _debugPath;

    private PathfindingComponent _pathfindingScript;

    // Start is called before the first frame update
    void Start()
    {
        _gridWorldSize = new Vector2(gridTopRight.position.x - gridBottomLeft.position.x, gridTopRight.position.y - gridBottomLeft.position.y);

        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / nodeSize);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / nodeSize);

        PathfindingManager manager = this;
        _pathfindingScript =  gameObject.AddComponent<PathfindingComponent>();
        _pathfindingScript.Initialise(ref manager);

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

                _grid[x, y].InitialiseNode(isBlocked, worldPosition, x, y);
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        if (worldPosition.x < gridBottomLeft.position.x
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

    public List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int checkX;
        int checkY;

        // Check surrounding nodes
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Ignore the same node
                if(x == 0 && y == 0)
                {
                    continue;
                }

                checkX = node.gridX + x;
                checkY = node.gridY + y;

                if(checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Vector3 GetPathDirection(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = NodeFromWorldPosition(startPosition);
        if (startNode.isBlocked)
        {
            List<Node> startNeighbours = GetNeighbourNodes(startNode);
            if(startNeighbours.Count <= 0)
            {
                Debug.LogError("Target is unreachable and stuck in a wall");
                return Vector3.zero;
            }

            startNode = startNeighbours[0];
        }

        Node targetNode = NodeFromWorldPosition(targetPosition);
        if (targetNode.isBlocked)
        {
            List<Node> targetNeighbours = GetNeighbourNodes(targetNode);
            if (targetNeighbours.Count <= 0)
            {
                Debug.LogError("Target is unreachable and stuck in a wall");
                return Vector3.zero;
            }

            targetNode = targetNeighbours[0];
        }

        List<Node> path = _pathfindingScript.GetPath(startNode, targetNode);
        _debugPath = path;
        if (path != null)
        {
            Debug.Log((path[0].worldPosition - startPosition).normalized);
            return (path[0].worldPosition - startPosition).normalized;
        }
        Debug.Log(Vector3.zero);
        return Vector3.zero;
    }

    public Vector3 GetNearestNodeInDirection(Vector3 target, Vector3 direction)
    {
        Node returnNode = NodeFromWorldPosition(target);
        List<Node> neighbourNodes;

        // AGAIN I AM BEING VERY CAREFUL
        while (returnNode.isBlocked)
        {
            neighbourNodes = GetNeighbourNodes(returnNode);
            if(neighbourNodes.Count <= 0)
            {
                returnNode = NodeFromWorldPosition(returnNode.worldPosition + direction * nodeSize);
                continue;
            }
            returnNode = neighbourNodes[0];
        }

        return returnNode.worldPosition;
    } 

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (_grid != null)
        {
            foreach (Node node in _grid)
            {
                Gizmos.color = Color.red;
                if (node.isBlocked)
                {
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSize - 0.1f));
                }
            }
        }

        if (_debugPath != null)
        {
            foreach (Node node in _debugPath)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSize - 0.1f));
            }
        }
    }
}
