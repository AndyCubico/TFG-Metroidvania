using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeleportTP : MonoBehaviour
{
    public List<GameObject> tps;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tps.Any())
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                player.transform.position = tps[0].transform.position;
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                player.transform.position = tps[1].transform.position;
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                player.transform.position = tps[2].transform.position;
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                player.transform.position = tps[3].transform.position;
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                player.transform.position = tps[4].transform.position;
            }
            else if (Input.GetKeyDown(KeyCode.F6))
            {
                player.transform.position = tps[5].transform.position;
            }
        }
    }
}
