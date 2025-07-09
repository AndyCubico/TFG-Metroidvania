using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    struct enemy
    {
        public GameObject obj;
        public float timer;
        public Rigidbody2D rb;
    };

    // Point frome where the attacks will be shot
    Transform spawner;

    [Header("Enemy Attack Prefab")]
    public GameObject enemyPrefab;

    // List of the enemies
    List<enemy> enemies;

    [Header("Different settings")]
    public bool flipDirection;
    public bool enemiesGravity;


    [Header("Life time of attacks")]
    public float enemiesLifeTime;

    [Header("Speed of attacks")]
    public float enemiesSpeed;

    [Header("Spawn Rate of attacks")]
    public float spawnRate;

    // Timer for spawn
    float spawnTimer;

    void Start()
    {
        spawner = this.gameObject.transform;
        enemies = new List<enemy>();
    }

    void Update()
    {
        // Spawn timer
        spawnTimer += Time.deltaTime;

        if(spawnTimer >= spawnRate) // If the time of new spawn arrives, set up a new enemy and spawns it with a moving force
        {
            int dir = (flipDirection ? -1 : 1); // Select the shooting direction, left or right

            enemy e = new enemy();

            e.obj = Instantiate(enemyPrefab, spawner.transform.position, spawner.transform.rotation);
            e.timer = enemiesLifeTime;
            e.rb = e.obj.GetComponent<Rigidbody2D>();

            e.obj.GetComponent<EnemyHit>().canBeParried = Random.value < 0.5f; // Generates a random true or false to indicate if it can be parried

            if (!e.obj.GetComponent<EnemyHit>().canBeParried)
            {
                e.obj.GetComponent<SpriteRenderer>().color = Color.yellow;
            }

            if (!enemiesGravity) // Quit gravity in case of want it
            {
                e.rb.gravityScale = 0;
            }

            e.rb.linearVelocity = new Vector2(enemiesSpeed * dir, 0);

            enemies.Add(e);

            spawnTimer = 0;
        }

        for(int i = 0; i < enemies.Count; i++) // Here the enemies timer will be checked to delete them in case of passing the time of life
        {
            enemy e = enemies[i];
            GameObject objToDelete;

            if(enemies[i].obj == null)
            {
                Destroy(enemies[i].obj);
                enemies.RemoveAt(i);
            }

            if(e.timer > 0f) // Reduce the time of life
            {
                e.timer -= Time.deltaTime;

                enemies[i] = e;
            }
            else // Delete the object of the enemy and remove it from the list
            {
                objToDelete = e.obj;

                if(objToDelete != null)
                {
                    Destroy(objToDelete);
                }

                enemies.RemoveAt(i);
            }
        }
    }
}
