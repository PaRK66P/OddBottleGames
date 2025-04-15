using System.Collections.Generic;
using UnityEngine;

public class PathfindingComponent : MonoBehaviour
{
    private PathfindingManager _manager;
    private List<Node> _path;

    // Sets up the component
    public void Initialise(ref PathfindingManager manager)
    {
        _manager = manager;
    }

    // Returns the path from the start node to the target node
    /*
     * Uses A* pathfinding algorithm
     */
    public List<Node> GetPath(Node startNode, Node targetNode)
    {
        _path = new List<Node>();

        List<Node> openSet = new List<Node>(); // Set of nodes to visit
        HashSet<Node> closedSet = new HashSet<Node>(); // Set of visited nodes
        openSet.Add(startNode);

        // While there are nodes to explore
        while (openSet.Count > 0) // I AM BEING VERY CAREFUL WITH THIS
        {
            // Find node with lowest cost
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].GetFCost <= currentNode.GetFCost)
                {
                    if (openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }
            }

            // Update sets
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) // Found path
            {
                RetracePath(startNode, targetNode);
                return _path;
            }

            // Add valid neighbours to the open set
            foreach (Node neighbour in _manager.GetNeighbourNodes(currentNode))
            {
                if (neighbour.isBlocked || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(neighbour, targetNode);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Updates the grid nodes with these values
                    /*
                     * Only one path is calculated at a time so this should be fine and shouldn't disrupt other paths
                     */
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parentNode = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        // All nodes exhausted, no path was found
        return null;
    }

    // Updates the _path to trace from the start node to the end node
    /*
     * The nodes themselves contain their parent node for the current path
     * Using this the path is retraced and reversed to be from the start to the end
     */
    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();

        _path = path;
    }

    // Returns the distance between the two nodes (unsigned)
    private int GetDistance(Node startNode, Node endNode)
    {
        int distanceX = Mathf.Abs(startNode.gridX - endNode.gridX);
        int distanceY = Mathf.Abs(startNode.gridY - endNode.gridY);

        // Calculate diagonal distance (14) then add leftover distance (10)

        if(distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
