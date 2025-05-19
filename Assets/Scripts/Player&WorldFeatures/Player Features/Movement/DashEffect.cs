using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerController;

public class DashEffect : MonoBehaviour
{
    public CharacterPlayerController player;
    List<GameObject> clones;
    public float separationTime;
    public float timeFading;
    [SerializeField] public GameObject cloneSpirte;
    private float timeClones;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterPlayerController>();
        clones = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDashing || player.isImpactHitting)
        {
            timeClones += Time.deltaTime;

            if(timeClones > separationTime)
            {
                GameObject tempGame = Instantiate(cloneSpirte, transform.position, transform.rotation) as GameObject;
                //tempGame.GetComponent<SpriteRenderer>().color = Color.yellow;

                if (player.flipAnimation)
                {
                    tempGame.GetComponent<SpriteRenderer>().flipX = true;
                }

                clones.Add(tempGame);
                timeClones = 0;
            }
        }
        else
        {
            if (clones.Count() > 0)
            {
                if (clones[0] != null)
                {
                    Color temp = clones[0].GetComponent<SpriteRenderer>().color;

                    if (temp.a <= 0f)
                    {
                        Destroy(clones[0]);
                        clones[0] = null;
                        clones.RemoveAt(0);
                    }
                }
            }

            //for(int i = clones.Count() - 1; i > -1; i--)
            //{
            //    if (clones[i] != null)
            //    {
            //        Color temp = clones[i].GetComponent<SpriteRenderer>().color;

            //        if (clones[i] != null && temp.a <= 0f)
            //        {
            //            Destroy(clones[i]);
            //            clones[i] = null;
            //            clones.RemoveAt(i);
            //        }
            //    }
            //}
        }

        if (clones.Count() > 0)
        {
            for (int i = 0; i < clones.Count(); i++)
            {
                if(clones[i] != null)
                {
                    Color temp = clones[i].GetComponent<SpriteRenderer>().color;

                    if (temp.a > 0f)
                    {
                        temp.a -= timeFading * Time.deltaTime;
                        clones[i].GetComponent<SpriteRenderer>().color = temp;
                    }
                }
            }
        }
    }
}
