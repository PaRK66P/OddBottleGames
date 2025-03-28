using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    private List<Node> _neighbourNodes;
    private Node _targetNode;

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

    public Vector2 GetPathDirection(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = NodeFromWorldPosition(startPosition); // Node should never be blocked
        Node targetNode = GetNearestNodeInDirection(NodeFromWorldPosition(targetPosition).worldPosition, startPosition - targetPosition);
        _targetNode = targetNode;

        List<Node> path = _pathfindingScript.GetPath(startNode, targetNode);
        if (path.Count > 1)
        {
            Vector2 pathDistance = new Vector2(path[1].worldPosition.x - startPosition.x, path[1].worldPosition.y - startPosition.y);

            _debugPath = path;

            return pathDistance.normalized;
        }

        _debugPath = path;

        if (NodeFromWorldPosition(targetPosition).isBlocked)
        {
            return targetNode.worldPosition - startPosition;
        }

        return targetPosition - startPosition;
    }

    public Node GetNearestNodeInDirection(Vector3 target, Vector3 direction)
    {
        direction.Normalize();
        Node returnNode = NodeFromWorldPosition(target);
        if(!returnNode.isBlocked) { return returnNode; }
        List<Node> neighbourNodes;

        _neighbourNodes = new List<Node>();

        bool isPositiveXDirection = direction.x >= 0.0f ? true : false;
        bool isPositiveYDirection = direction.y >= 0.0f ? true : false;

        Vector3 neighbourDirection;
        bool isSameXDirection;
        bool isSameYDirection;

        // AGAIN I AM BEING VERY CAREFUL
        while (returnNode.isBlocked)
        {
            neighbourNodes = GetNeighbourNodes(returnNode);
            if(neighbourNodes.Count > 0)
            {
                foreach (Node neighbour in neighbourNodes)
                {
                    neighbourDirection = neighbour.worldPosition - returnNode.worldPosition;
                    isSameXDirection = isPositiveXDirection == (neighbourDirection.x >= 0.0f ? true : false);
                    isSameYDirection = isPositiveYDirection == (neighbourDirection.y >= 0.0f ? true : false);

                    if ((isSameXDirection && neighbourDirection.y == 0.0f) // Check for only X direction
                        || (isSameYDirection && neighbourDirection.x == 0.0f) // Check for only Y direction
                        || (isSameXDirection && isSameYDirection)) // Check for matching both
                    {
                        if(!neighbour.isBlocked) 
                        {
                            Debug.Log("Found neighbour");
                            return neighbour; 
                        }
                        _neighbourNodes.Add(neighbour);
                    }
                }
            }

            returnNode = NodeFromWorldPosition(returnNode.worldPosition + direction * nodeSize);
            _neighbourNodes.Add(returnNode);
        }

        Debug.Log("Returned position");
        return returnNode;
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

        if (_neighbourNodes != null)
        {
            foreach (Node node in _neighbourNodes)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSize - 0.1f));
            }
        }

        if (_targetNode != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_targetNode.worldPosition, Vector3.one * (nodeSize - 0.1f));
        }
    }
}
