using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ArenaCombat : MonoBehaviour
{
    [System.Serializable]
    public class EnemyToSpawn 
    {
        public GameObject enemyPrefab;
        public Vector3 positition;
        public GameObject SpawnEnemy()
        {
            return Instantiate(enemyPrefab,positition,Quaternion.identity);
        }
    };

    [System.Serializable]
    public class CombatWave 
    {
        public List<EnemyToSpawn> enemies = new List<EnemyToSpawn>();
        public int maxEnemiesOnce = 2;
        public float spawnDelay = 0;
        public float waveChangeDelay;
        List<GameObject> m_activeEnemies = new List<GameObject>();
        int m_deadEnemies = 0;
        public List<GameObject> objectsToDestroy;
        public bool waveFinished = false;

        public IEnumerator ManageWave()
        {
            while (m_deadEnemies < enemies.Count)
            {
                // Spawn enemies if the cap isn't reached
                if (m_activeEnemies.Count < maxEnemiesOnce && m_deadEnemies + m_activeEnemies.Count < enemies.Count)
                {
                    yield return new WaitForSeconds(spawnDelay);
                    m_activeEnemies.Add(enemies[m_activeEnemies.Count + m_deadEnemies].SpawnEnemy());
                }

                // Check active enemies
                for (int i = m_activeEnemies.Count - 1; i >= 0; i--) // The iteration is backwards for saefty as list eliminate from left to rigth (or something like that, I'm not really sure)
                {
                    if (m_activeEnemies[i] == null || m_activeEnemies[i].IsDestroyed()) // IsDestroyed() isn't very reliable, so I use both just in case
                    {
                        m_deadEnemies++;
                        m_activeEnemies.RemoveAt(i);
                    }
                }

                yield return null; // Prevent tight loop (Makes the corutine stop until next frame, without this the while freezes the game).
            }

            // After wave ends
            yield return new WaitForSeconds(waveChangeDelay);

            foreach (GameObject go in objectsToDestroy) {Destroy(go); } // Destoy floors, door or any obstacle needed to be destroyed when wave ends
                

            waveFinished = true;
        }
    }

    public List<CombatWave> listWaves = new List<CombatWave>();

    // Wave Start Paramentetrs
    [SerializeField] bool m_IsStartOnCollision;
    [TagDropdown] public string[] collisionTagList = new string[] { };

    // Wave Manegmnent parameters
    int m_CurrentWaveIndex = 0;
    Coroutine m_CurrentWave;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!m_IsStartOnCollision) // If not start on collision it starts when the player enters 
        {
            StartCombat();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentWave != null && m_CurrentWaveIndex < listWaves.Count) 
        {
            if (listWaves[m_CurrentWaveIndex].waveFinished) 
            {
                //StopCoroutine(m_CurrentWave);
                m_CurrentWaveIndex++;
                if (listWaves.Count > m_CurrentWaveIndex) {StartCombat(); }
                

            }
        }
    }

    void StartCombat()
    {
        m_CurrentWave = StartCoroutine(listWaves[m_CurrentWaveIndex].ManageWave());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionTagList.Contains(collision.gameObject.tag) && m_IsStartOnCollision) 
        {
            m_IsStartOnCollision = false; // Make sure is not triggered again on collision
            StartCombat();
        }
    }
}
