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
    [Header("NECESSARY")]
    [SerializeField] // Collision layers that can't be traversed
    private LayerMask _blockingLayer;

    // Grid dimensions
    [SerializeField]
    private float _nodeSize;
    [SerializeField]
    private Transform _gridBottomLeft;
    [SerializeField]
    private Transform _gridTopRight;

    // Grid values
    private Node[,] _grid;
    private Vector2 _gridWorldSize;
    private int _gridSizeX;
    private int _gridSizeY;

    // Debugging
    private List<Node> _debugPath; 
    private List<Node> _neighbourNodes;
    private Node _targetNode;

    [Header("DEBUG")]
    [SerializeField]
    private bool _showGrid;
    [SerializeField]
    private bool _showBlockedNodes;
    [SerializeField]
    private bool _showRecentPath;
    [SerializeField]
    private bool _showRecentNeighbours;
    [SerializeField]
    private bool _showTargetNode;

    // Components
    private PathfindingComponent _pathfindingScript;

    #region Grid
    // Start is called before the first frame update
    void Start()
    {
        _gridWorldSize = new Vector2(_gridTopRight.position.x - _gridBottomLeft.position.x, _gridTopRight.position.y - _gridBottomLeft.position.y);

        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeSize);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeSize);

        PathfindingManager manager = this;
        _pathfindingScript =  gameObject.AddComponent<PathfindingComponent>();
        _pathfindingScript.Initialise(ref manager);

        InitialiseGrid();
        CalculateGrid();
    }

    // Sets up the grid
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

    // Updates each node in the grid
    public void CalculateGrid()
    {
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPosition = _gridBottomLeft.position + Vector3.right * (x * _nodeSize + (_nodeSize / 2.0f)) + Vector3.up * (y * _nodeSize + (_nodeSize / 2.0f));

                bool isBlocked = (Physics2D.BoxCast(worldPosition, Vector2.one * _nodeSize, 0, Vector2.up, 0, _blockingLayer));

                _grid[x, y].InitialiseNode(isBlocked, worldPosition, x, y);
            }
        }
    }
    #endregion

    #region Methods
    // Returns the node in the passed world position
    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        if (worldPosition.x < _gridBottomLeft.position.x
            || worldPosition.x > _gridTopRight.position.x
            || worldPosition.y < _gridBottomLeft.position.y
            || worldPosition.y > _gridTopRight.position.y)
        {
            Debug.LogWarning("Position out of grid");
            return _grid[0, 0];
        }

        float percentX = (worldPosition.x - _gridBottomLeft.position.x) / _gridWorldSize.x;
        float percentY = (worldPosition.y - _gridBottomLeft.position.y) / _gridWorldSize.y;

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

        return _grid[x, y];
    }

    // Returns a list of all neighbour nodes to the passed node
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

                // Check if within the grid
                if(checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    // Returns the direction from the start position to move along the path to get to the target position
    public Vector2 GetPathDirection(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = NodeFromWorldPosition(startPosition); // Node should never be blocked
        Node targetNode = GetNearestNodeInDirection(NodeFromWorldPosition(targetPosition).worldPosition, startPosition - targetPosition);
        _targetNode = targetNode; // Debug

        List<Node> path = _pathfindingScript.GetPath(startNode, targetNode);

        // Checks if the path is longer than the starting node
        if (path.Count > 1)
        {
            // Returns the direction to move from the start
            Vector2 pathDistance = new Vector2(path[1].worldPosition.x - startPosition.x, path[1].worldPosition.y - startPosition.y);

            _debugPath = path; // Debug

            return pathDistance.normalized;
        }

        // Otherwise the start is on the target node

        _debugPath = path; // Debug

        // If the original target position is blocked use the target node that isn't blocked
        /* This ensures the future start positions will never be on a blocked node */
        if (NodeFromWorldPosition(targetPosition).isBlocked)
        {
            return targetNode.worldPosition - startPosition;
        }

        return targetPosition - startPosition;
    }

    // Returns the nearest unblocked node from the target with consideration of direction from start position
    /* target is the target world position
     * direction is the direction from the TARGET world position to the START world position
     */
    public Node GetNearestNodeInDirection(Vector3 target, Vector3 direction)
    {
        direction.Normalize();
        Node returnNode = NodeFromWorldPosition(target);
        if(!returnNode.isBlocked) { return returnNode; } // Return the target if they aren't blocked
        List<Node> neighbourNodes;

        _neighbourNodes = new List<Node>(); // Debug

        bool isPositiveXDirection = direction.x >= 0.0f ? true : false;
        bool isPositiveYDirection = direction.y >= 0.0f ? true : false;

        Vector3 neighbourDirection;
        bool isSameXDirection;
        bool isSameYDirection;

        // Loop until the node is no longer blocked
        // AGAIN I AM BEING VERY CAREFUL
        while (returnNode.isBlocked)
        {
            neighbourNodes = GetNeighbourNodes(returnNode);
            foreach (Node neighbour in neighbourNodes) // Checks each neighbour node
            {
                neighbourDirection = neighbour.worldPosition - returnNode.worldPosition;
                // Compare direction with neighbour direction
                isSameXDirection = isPositiveXDirection == (neighbourDirection.x >= 0.0f ? true : false);
                isSameYDirection = isPositiveYDirection == (neighbourDirection.y >= 0.0f ? true : false);

                // Only use neighbours that are in the matching direction
                if ((isSameXDirection && neighbourDirection.y == 0.0f) // Check for only X direction
                    || (isSameYDirection && neighbourDirection.x == 0.0f) // Check for only Y direction
                    || (isSameXDirection && isSameYDirection)) // Check for matching both
                {
                    _neighbourNodes.Add(neighbour); // Debug

                    if (!neighbour.isBlocked) // If the neighbour is in the matching direction and not blocked the node has been found
                    {
                        return neighbour;
                    }
                }
            }

            // No neighbour was valid so change the node that we checked by moving it a node closer in the target direction
            returnNode = NodeFromWorldPosition(returnNode.worldPosition + direction * _nodeSize);

            _neighbourNodes.Add(returnNode); // Debug
        }

        // The new node is valid ending the loop
        return returnNode;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (_showGrid)
        {
            if (_grid != null)
            {
                Gizmos.color = Color.cyan;
                foreach (Node node in _grid)
                {
                    if (!node.isBlocked)
                    {
                        Gizmos.DrawCube(node.worldPosition, Vector3.one * (_nodeSize - 0.1f));
                    }
                }
            }
        }

        if (_showBlockedNodes)
        {
            if (_grid != null)
            {
                Gizmos.color = Color.red;
                foreach (Node node in _grid)
                {
                    if (node.isBlocked)
                    {
                        Gizmos.DrawCube(node.worldPosition, Vector3.one * (_nodeSize - 0.1f));
                    }
                }
            }
        }

        if (_showRecentPath)
        {
            if (_debugPath != null)
            {
                Gizmos.color = Color.green;
                foreach (Node node in _debugPath)
                {
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (_nodeSize - 0.1f));
                }
            }
        }

        if (_showRecentNeighbours)
        {
            if (_neighbourNodes != null)
            {
                Gizmos.color = Color.blue;
                foreach (Node node in _neighbourNodes)
                {
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (_nodeSize - 0.1f));
                }
            }
        }

        if (_showTargetNode)
        {
            if (_targetNode != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(_targetNode.worldPosition, Vector3.one * (_nodeSize - 0.1f));
            }
        }
    }
    #endregion
}
