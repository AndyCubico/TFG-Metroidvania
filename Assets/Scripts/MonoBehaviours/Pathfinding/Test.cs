using UnityEngine;

public class Test : MonoBehaviour
{
    // Test Grid class
    private Grid grid;
    [SerializeField] private int width, height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector2 origin;

    void Start()
    {
        grid = new Grid(width, height, cellSize, origin);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            grid.SetValue(mouseWorldPos, 1);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Debug.Log(grid.GetValue(mouseWorldPos));
        }
    }
}
