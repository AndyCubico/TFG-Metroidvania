using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Test Grid class
    public Grid<GridNode> grid;
    [SerializeField] private int width, height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector2 origin;

    public static GridManager Instance { private set; get; }

    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        grid = new Grid<GridNode>(width, height, cellSize, origin, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));
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

