using UnityEngine;

// Only one per Scene
public class SetActiveInstanceSingleton : MonoBehaviour
{
    public static SetActiveInstanceSingleton Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    [Tooltip("Must add GameObjects from inspector")]
    public NestedList<GameObject> gameObjectsList;
}