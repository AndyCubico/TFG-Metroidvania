using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPath(new int2(0,0), new int2(3,1));
    }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private void FindPath(int2 startPosition, int2 endPosition)
    {
        // Testing with custom grid, later on do with the grid done in Grid.cs
        int2 gridSize = new int2(4, 4);
        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                PathNode pathNode = new PathNode();
                pathNode.x = x;
                pathNode.y = y;

                pathNode.index = CalculateIndex(x, y, gridSize.x);

                pathNode.gCost = int.MaxValue;
                pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                pathNode.CalculateFCost();

                pathNode.isWalkable = true;
                pathNode.previousNodeIndex = -1;

                pathNodeArray[pathNode.index] = pathNode;
            }
        }

        // A*

        // Neighbour offsets
        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(new int2[]
        {
            new int2(-1, 0), // Left
            new int2(1, 0), // Right
            new int2(0, 1), // Up
            new int2(0, -1), // Down
            new int2(-1, -1), // Left Down
            new int2(-1, 1), // Left Up
            new int2(1, -1), // Right Down
            new int2(1, 1), // RIght Up
        }, Allocator.Temp);

        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

        PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
        startNode.gCost = 0;
        startNode.CalculateFCost();
        pathNodeArray[startNode.index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> visitedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.index);

        while (openList.Length > 0)
        {
            int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
            PathNode currentNode = pathNodeArray[currentNodeIndex];

            if (currentNodeIndex == endNodeIndex)
            {
                // Reached goal
                break;
            }

            // Remove current node from the open list
            for (int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == currentNodeIndex)
                {
                    openList.RemoveAt(i);
                    break;
                }
            }

            visitedList.Add(currentNodeIndex);

            for (int i = 0; i < neighbourOffsetArray.Length; i++)
            {
                int2 neighbourOffset = neighbourOffsetArray[i];
                int2 neighbourPoistion = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                if (!IsPositionInGrid(neighbourPoistion, gridSize))
                {
                    // Not a valid neighbour
                    continue;
                }

                int neighbourNodeIndex = CalculateIndex(neighbourPoistion.x, neighbourPoistion.y, gridSize.x);

                if (visitedList.Contains(neighbourNodeIndex))
                {
                    // Altready visited this node
                    continue;
                }

                PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];

                if (!neighbourNode.isWalkable)
                {
                    // Invalid node
                    continue;
                }

                // Compare costs
                int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPoistion);

                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.previousNodeIndex = currentNodeIndex;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.CalculateFCost();
                    pathNodeArray[neighbourNodeIndex] = neighbourNode;

                    if (!openList.Contains(neighbourNode.index))
                    {
                        openList.Add(neighbourNode.index);
                    }
                }
            }
        }

        PathNode endNode = pathNodeArray[endNodeIndex];
        if (endNode.previousNodeIndex == -1)
        {
            // No path was found
            Debug.Log("No path was found");
        }
        else
        {
            // Found a path
            NativeList<int2> path = CalculatePath(pathNodeArray, endNode);

            foreach (int2 pathPosition in path)
            {
                Debug.Log(pathPosition);
            }

            path.Dispose();
        }

        pathNodeArray.Dispose();
        neighbourOffsetArray.Dispose();
        openList.Dispose();
        visitedList.Dispose();
    }

    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
    {
        if (endNode.previousNodeIndex == -1)
        {
            // No path is found
            return new NativeList<int2>(Allocator.Temp);
        }
        else
        {
            // Found a path
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x, endNode.y));

            PathNode currentNode = endNode;
            while (currentNode.previousNodeIndex != -1)
            {
                PathNode previousNode = pathNodeArray[currentNode.previousNodeIndex];
                path.Add(new int2(previousNode.x, previousNode.y));
                currentNode = previousNode;
            }

            return path;
        }
    }

    private bool IsPositionInGrid(int2 gridPosition, int2 gridSize)
    {
        return
            gridPosition.x >= 0 &&
            gridPosition.y >= 0 &&
            gridPosition.x < gridSize.x &&
            gridPosition.y < gridSize.y;
    }

    private int CalculateIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
    {
        int xDistance = math.abs(aPosition.x - bPosition.x);
        int yDistance = math.abs(bPosition.y - aPosition.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];

        for (int i = 1; i < openList.Length; i++)
        {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if (testPathNode.fCost < lowestCostPathNode.fCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }

        return lowestCostPathNode.index;
    }

    private struct PathNode
    {
        public int x;
        public int y;

        public int index;
        public int previousNodeIndex;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool isWalkable;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}
