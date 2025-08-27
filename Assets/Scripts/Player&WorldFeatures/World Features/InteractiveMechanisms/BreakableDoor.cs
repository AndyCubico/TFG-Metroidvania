using UnityEngine;

public class BreakableDoor : MonoBehaviour, IHittableObject
{

    public AttackFlagType flagMask;
    [SerializeField] bool m_IsSave;

    public class Breakable_SL : object_SL
    {
        public bool isBroken;
    }
    Breakable_SL m_Save;

    private void Start()
    {
        if (m_IsSave)
        {
            World_Save_Load saveLoad = GameObject.Find("GameManager")?.GetComponent<World_Save_Load>();

            object_SL nameObj = new object_SL
            {
                // Generic objects attributes
                objectName = this.gameObject.name,
                objectID = this.gameObject.transform.GetSiblingIndex(),
                objectType = object_SL.ObjectType.BREAKABLE_OBJECT,
            };

            m_Save = (Breakable_SL)saveLoad.LoadObject(nameObj);

            if (m_Save != null)
            {
                if (m_Save.isBroken)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flag & flagMask) != 0)
        {
            GameManagerEvents.eSpawnDamageText(new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + this.gameObject.transform.localScale.y / 2, transform.position.z), damage);

            if (m_IsSave)
            {
                World_Save_Load saveLoad = GameObject.Find("GameManager")?.GetComponent<World_Save_Load>();

                m_Save = new Breakable_SL
                {
                    // Generic objects attributes
                    objectName = this.gameObject.name,
                    objectID = this.gameObject.transform.GetSiblingIndex(),
                    objectType = object_SL.ObjectType.BREAKABLE_OBJECT,

                    // Specific object atributes
                    isBroken = true

                };
                saveLoad.SaveObject(m_Save);
            }

            Destroy(gameObject);
        }
    }
}