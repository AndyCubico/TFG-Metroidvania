//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MoveWithLerp : MonoBehaviour
{
    public Transform initialPosition;
    public Transform finalPosition;
    public MonoBehaviour lerpSource; // We cannot use an Interface as a parameter, so we must use a Mono

    private ILerpValueReturn m_LerpReturner;

    // Control of smooth lerp
    [SerializeField] float m_TargetValue;

    void Awake()
    {
        m_LerpReturner = lerpSource as ILerpValueReturn;
        if (m_LerpReturner == null) {Debug.LogError("Assigned object does not implement ILerpValueReturn"); }
            
    }
    // Update is called once per frame
    void Update()
    {
        float t = m_LerpReturner.GetCurrentValue();
        transform.position = Vector3.Lerp(initialPosition.position, finalPosition.position, t);
    }
}
