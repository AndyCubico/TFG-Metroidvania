using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Grid<GridNode> grid;
    [SerializeField] private int width, height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector2 origin;

    [SerializeField] private LayerMask m_NotWalkable; // TODO: It only works with ground, manage other types of platforms.

    public static GridManager Instance { private set; get; }

    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        grid = new Grid<GridNode>(width, height, cellSize, origin, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));

        // --- Set grid walkability ---

        // First check for colliders that are blocked terrain.
        bool[,] blockedNodes = new bool[grid.GetWidth(), grid.GetHeight()]; // Store grid nodes that have terrain.

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Vector2 worldPos = origin + new Vector2(x + 0.5f, y + 0.5f) * cellSize;
                Collider2D hit = Physics2D.OverlapBox(worldPos, Vector2.one * cellSize * 0.8f, 0, m_NotWalkable);// * 0.9f to avoid false positives.
                blockedNodes[x, y] = hit != null;
            }
        }

        // Check which nodes have ground just beneath, they are walkable.
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 1; y < grid.GetHeight(); y++) // Start from y = 1 to allow y - 1.
            {
                // Nodes that are walkable above ground (node below is blocked, current cell is empty).
                if (blockedNodes[x, y - 1] && !blockedNodes[x, y])
                {
                    grid.GetValue(x, y)?.SetIsWalkable(true);
                }
            }
        }

        // In this last step, check if there are any cliffs, and if there are, make all nodes below it walkable until reaching ground.
        // Cliffs are walkable zones that tell the agent to jump instead of to just go walking.
        HashSet<int2> cliffNodes = new HashSet<int2>(new Helper.Int2Comparer()); // Store grid nodes that are cliffs.

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                // Not walkable but not blocked by ground.
                if (!blockedNodes[x, y] && grid.GetValue(x, y)?.IsWalkable() == false)
                {
                    // Check if right or left nodes are walkable and have not been previously checked for being a cliff.
                    bool leftIsWalkable = x > 0 && grid.GetValue(x - 1, y)?.IsWalkable() == true && !cliffNodes.Contains(new int2(x - 1, y));
                    bool rightIsWalkable = x < grid.GetWidth() - 1 && grid.GetValue(x + 1, y)?.IsWalkable() == true && !cliffNodes.Contains(new int2(x + 1, y));

                    // If true, the current node being checked is considered a cliff.
                    if (leftIsWalkable || rightIsWalkable)
                    {
                        // Mark this node as walkable and as a cliff.
                        grid.GetValue(x, y)?.SetIsWalkable(true);
                        grid.GetValue(x, y)?.SetIsCliff(true);
                        cliffNodes.Add(new int2(x, y));

                        // Mark all cells below as walkable until a ground node.
                        for (int z = y - 1; z >= 0; z--)
                        {
                            if (blockedNodes[x, z]) break;
                            grid.GetValue(x, z)?.SetIsWalkable(true);
                            cliffNodes.Add(new int2(x, z));
                        }
                    }
                }
            }
        }

        // Fill the gap between cliffs
        for (int y = 0; y < grid.GetHeight(); y++)
        {
            int? leftCliff = null;

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                var node = grid.GetValue(x, y);

                if (node != null && node.IsCliff())
                {
                    if (leftCliff == null)
                    {
                        leftCliff = x;
                    }
                    else
                    {
                        int start = leftCliff.Value + 1;
                        int end = x - 1;

                        bool validGap = true;

                        for (int gapX = start; gapX <= end; gapX++)
                        {
                            // Must be air above and below
                            if (blockedNodes[gapX, y] ||
                                (y > 0 && (blockedNodes[gapX, y - 1] || grid.GetValue(gapX, y - 1)?.IsWalkable() == true)))
                            {
                                validGap = false;
                                break;
                            }
                        }

                        if (validGap)
                        {
                            for (int gapX = start; gapX <= end; gapX++)
                            {
                                grid.GetValue(gapX, y)?.SetIsWalkable(true);
                                grid.GetValue(gapX, y)?.SetIsCliff(true);
                                cliffNodes.Add(new int2(gapX, y));

                                // Fill down until hitting ground
                                for (int z = y - 1; z >= 0; z--)
                                {
                                    if (blockedNodes[gapX, z]) break;
                                    grid.GetValue(gapX, z)?.SetIsWalkable(true);
                                    cliffNodes.Add(new int2(gapX, z));
                                }
                            }
                        }

                        leftCliff = x;
                    }
                }
            }
        }


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            grid.GetXYPosition(mouseWorldPos, out int x, out int y);
            GridNode node = grid.GetValue(x, y);
            if (node != null)
            {
                node.SetIsWalkable(!node.IsWalkable()); // Toggle walkability
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            grid.GetXYPosition(mouseWorldPos, out int x, out int y);
            GridNode node = grid.GetValue(x, y);
            if (node != null)
            {
                Debug.Log($"Node at ({x}, {y}) is {(node.IsWalkable() ? "Walkable" : "Blocked")}");
            }
        }
    }
}

