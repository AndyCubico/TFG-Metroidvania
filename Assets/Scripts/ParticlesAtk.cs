using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesAtk : MonoBehaviour
{
    [SerializeField] private ParticleSystem _subEmitter;
    [SerializeField] private GameObject _collider;

    //[SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private int _dmg = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_subEmitter != null)
        {
            if (_subEmitter.particleCount == 1)
            {
                _collider.SetActive(true);
            }
            else
            {
                _collider.SetActive(false);
            }
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Take Damage");
            //_playerHealth.TakeDmg(_dmg);
        }
    }
}