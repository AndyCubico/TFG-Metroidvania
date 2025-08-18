using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHittableObject
{
    [Header("Life Variables")]
    public float life;

    public AttackFlagType flagMask;
    public bool cantBePushed = false;

    bool pushTransition = false;

    Vector3 nextPosition;

    private float displacement = 3f;
    private float speedDisplacement = 2f;

    private float timeToDisplace = 1.5f;
    private float displaceCounter = 0f;

    private float wallCollisionDamage = 30f;


    void Update()
    {
        if (pushTransition && this.gameObject != null)
        {
            displaceCounter += Time.deltaTime;

            if (displaceCounter >= timeToDisplace)
            {
                this.gameObject.layer = 11; // Return layer to enemy
                pushTransition = false;
                displaceCounter = 0f;
            }

            if (this.transform.position != nextPosition)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, nextPosition, speedDisplacement * Time.deltaTime);
            }
            else
            {
                this.gameObject.layer = 11; // Return layer to enemy
                pushTransition = false;
            }
        }
    }

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flag & flagMask) != 0)
        {
            life -= damage;
            GameManagerEvents.eSpawnDamageText(new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + this.gameObject.transform.localScale.y / 2, transform.position.z), damage);

            if (life <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void PushEnemy(GameObject player)
    {
        if (!cantBePushed)
        {
            pushTransition = true;
            this.gameObject.layer = 0; //Quit layer enemy temporally

            bool left = false;

            if (player != null)
            {
                if (player.transform.position.x >= this.transform.position.x)
                {
                    left = true;
                }
                else
                {
                    left = false;
                }
            }

            nextPosition = new Vector3(left ? (this.transform.position.x - displacement) : (this.transform.position.x + displacement), this.transform.position.y, this.transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pushTransition)
        {
            if (collision.gameObject.layer == 0 || collision.gameObject.layer == 9 || collision.gameObject.layer == 11)
            {
                this.gameObject.layer = 11; // Return layer to enemy
                pushTransition = false;
                displaceCounter = 0f;

                ReceiveDamage(wallCollisionDamage, AttackFlagType.Wall);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (pushTransition)
        {
            if (collision.gameObject.layer == 0 || collision.gameObject.layer == 9 || collision.gameObject.layer == 11)
            {
                this.gameObject.layer = 11; // Return layer to enemy
                pushTransition = false;
                displaceCounter = 0f;
            }
        }
    }
}
