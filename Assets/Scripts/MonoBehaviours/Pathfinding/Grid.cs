using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private int m_Width;
    private int m_Height;
    private float m_CellSize;
    private int[,] m_GridArray;

    // Debug draw numbers in screen
    private TextMesh[,] m_DebugTextArray;

    public Grid(int width, int height, float cellSize)
    {
        m_Width = width;
        m_Height = height;
        m_CellSize = cellSize;

        m_GridArray = new int[width, height];
        m_DebugTextArray = new TextMesh[width, height];

        for (int x = 0; x < m_GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < m_GridArray.GetLength(1); y++)
            {
                m_DebugTextArray[x, y] = CreateDisplayText(m_GridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 20, Color.white);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * m_CellSize;
    }

    private void GetXYPosition(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / m_CellSize);
        y = Mathf.FloorToInt(worldPosition.y / m_CellSize);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < m_Width && y < m_Height)
        {
            return m_GridArray[x, y];
        }
        else
        {
            return -1;
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXYPosition(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    // Change cell value
    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < m_Width && y < m_Height)
        {
            m_GridArray[x, y] = value;
            m_DebugTextArray[x, y].text = value.ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXYPosition(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    // Debug Grid values
    private static TextMesh CreateDisplayText(string text = "", Transform parent = null, Vector3 localPosition = default, int fontSize = 10, Color color = default,
                                                TextAnchor textAnchor = TextAnchor.MiddleCenter, int sortingOrder = 500, TextAlignment textAlignment = TextAlignment.Center)
    {
        GameObject go = new GameObject("Display Text", typeof(TextMesh));
        Transform transform = go.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;

        transform.localScale = Vector3.one * 0.1f; // This makes the text readable at small sizes

        TextMesh textMesh = go.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.fontSize = fontSize;
        textMesh.text = text;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
