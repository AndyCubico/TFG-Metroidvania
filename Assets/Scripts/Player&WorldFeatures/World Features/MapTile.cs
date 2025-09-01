using UnityEngine;

public class MapTile : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(1,1);

    [SerializeField] SceneField asociatedScene;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x * 64, size.y * 36);
    }
}
