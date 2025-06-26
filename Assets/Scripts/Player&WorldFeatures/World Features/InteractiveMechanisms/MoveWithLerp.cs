using UnityEngine;

public class MoveWithLerp : MonoBehaviour
{
    public Transform initialPosition;
    public Transform finalPosition;
    public MonoBehaviour lerpSource;

    private ILerpValueReturn lerpReturner;

    void Awake()
    {
        lerpReturner = lerpSource as ILerpValueReturn;
        if (lerpReturner == null)
            Debug.LogError("Assigned object does not implement ILerpValueReturn");
    }
    // Update is called once per frame
    void Update()
    {
        float t = lerpReturner.GetCurrentValue();
        transform.position = Vector3.Lerp(initialPosition.position, finalPosition.position, t);
    }
}
