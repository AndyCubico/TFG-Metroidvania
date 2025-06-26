using UnityEngine;

public class MoveWithLerp : MonoBehaviour
{
    public ILerpValueReturn lerpReturner; // Inherithing from an interface allows to use any 


    // Update is called once per frame
    void Update()
    {
        lerpReturner.GetCurrentValue();
    }
}
