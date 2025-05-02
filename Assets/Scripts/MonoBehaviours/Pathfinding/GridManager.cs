using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Test Grid class
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
                Collider2D hit = Physics2D.OverlapBox(worldPos, Vector2.one * cellSize * 0.9f, 0, m_NotWalkable);// * 0.9f to avoid false positives.
                blockedNodes[x, y] = hit != null;
            }
        }

        // Check which cells have ground just beneath, they are walkable.
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 1; y < grid.GetHeight(); y++) // Start from y = 1 to allow y - 1.
            {
                // Walkable above ground (cell below is blocked, current cell is empty).
                if (blockedNodes[x, y - 1] && !blockedNodes[x, y])
                {
                    grid.GetValue(x, y)?.SetIsWalkable(true);
                }
            }
        }

        // In this last step, check if there are any ledges, and if there are, make all nodes below it walkable until reaching ground.
        // This will create walkable zones for the agents to jump.
        HashSet<int2> jumpOrLedgeNodes = new HashSet<int2>(new Int2Comparer()); // Store grid nodes that are ledges or jump nodes.

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                // Not walkable but not blocked by ground.
                if (!blockedNodes[x, y] && grid.GetValue(x, y)?.IsWalkable() == false)
                {
                    // Check if right or left nodes are walkable and have not been previously checked for being either a ledge or jump node.
                    bool leftIsWalkable = x > 0 && grid.GetValue(x - 1, y)?.IsWalkable() == true && !jumpOrLedgeNodes.Contains(new int2(x - 1, y));
                    bool rightIsWalkable = x < grid.GetWidth() - 1 && grid.GetValue(x + 1, y)?.IsWalkable() == true && !jumpOrLedgeNodes.Contains(new int2(x + 1, y));

                    // If true, the current node being checked is considered a ledge.
                    if (leftIsWalkable || rightIsWalkable)
                    {
                        // Mark this ledge as walkable
                        grid.GetValue(x, y)?.SetIsWalkable(true);
                        jumpOrLedgeNodes.Add(new int2(x, y));

                        // Mark all cells below as walkable until a ground node to create the jump nodes.
                        for (int z = y - 1; z >= 0; z--)
                        {
                            if (blockedNodes[x, z]) break;
                            grid.GetValue(x, z)?.SetIsWalkable(true);
                            jumpOrLedgeNodes.Add(new int2(x, z));
                        }
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

    class Int2Comparer : IEqualityComparer<int2>
    {
        public bool Equals(int2 a, int2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public int GetHashCode(int2 obj)
        {
            return obj.x.GetHashCode() ^ obj.y.GetHashCode();
        }
    }
}

