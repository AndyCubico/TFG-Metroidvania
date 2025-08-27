using System.Collections;
using UnityEngine;
using static BreakableDoor;

public class SnowBreakableDoor : MonoBehaviour, IHittableObject
{
    public AttackFlagType flagMask;
    public AttackFlagType snowMask;
    public bool isFrail; // When hit by snow spell turn into 
    [SerializeField] bool m_IsSave;
    [SerializeField] Color[] colors = new Color[2];

    public Breakable_SL m_Save;

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

    private IEnumerator TurnBrittle()
    {
        isFrail = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = colors[1];
        yield return new WaitForSeconds(4); // Coldown time of snow spell more or less (all elements that interact with the snow abilities last 4 seconds)
        this.gameObject.GetComponent<SpriteRenderer>().color = colors[0];
        isFrail = true;
    }

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flag & snowMask) != 0)
        {
            StartCoroutine(TurnBrittle());
        }

        if ((flag & flagMask) != 0)
        {
            if (isFrail) 
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
            else 
            {
                GameManagerEvents.eSpawnDamageText(new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + this.gameObject.transform.localScale.y / 2, transform.position.z), 0);
            }
            
        }
    }

    
}
