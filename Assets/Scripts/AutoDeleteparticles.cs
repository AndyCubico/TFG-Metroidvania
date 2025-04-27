using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeleteparticles : MonoBehaviour
{
    float timing = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timing += Time.deltaTime;

        if (timing > 3f) 
        {
            Destroy(this.gameObject);
        }
    }
}
