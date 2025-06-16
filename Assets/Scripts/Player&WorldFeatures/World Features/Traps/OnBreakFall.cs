using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class OnBreakFall : MonoBehaviour
{
    [SerializeField] GameObject m_ObjectChecked;

    private void Update()     
    {
        if (m_ObjectChecked.IsDestroyed())
        {
            this.gameObject.GetComponent<Rigidbody2D>().WakeUp();
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        }
    }
}
