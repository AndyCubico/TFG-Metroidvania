using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ArenaCombat : MonoBehaviour
{
    [System.Serializable]
    public class ComplexEnemyParameters 
    {
        public bool isRigth;
        public Vector4 visionSensor;

        public BoxCollider2D SetVisionCollider(Vector4? visionSensor) 
        {
            BoxCollider2D boxCollider2D = new BoxCollider2D();
            //boxCollider2D.bounds.max= visionSensor.max;
            return boxCollider2D;
        }
    }

    [System.Serializable]
    public class EnemyToSpawn 
    {
        public GameObject enemyPrefab;
        public Vector3 positition;
        public bool isExtraSpawn;

        public bool hasComplexParameters;
        [ShowIf("hasComplexParameters", true)] public ComplexEnemyParameters? extraParameters;

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
                    if (enemies[m_activeEnemies.Count + m_deadEnemies].isExtraSpawn) // Extra enemies don't need to be killed to progress the wave
                    {
                        /*GameObject enemy = */enemies[m_activeEnemies.Count + m_deadEnemies].SpawnEnemy();
                        

                        if (enemies[m_activeEnemies.Count + m_deadEnemies].hasComplexParameters) 
                        {
                            // Set rigth or left


                            // Set vision collider
                        }
                        
                        m_deadEnemies++;
                    }
                    yield return new WaitForSeconds(spawnDelay);

                    GameObject enemy2 = enemies[m_activeEnemies.Count + m_deadEnemies].SpawnEnemy();
                    m_activeEnemies.Add(enemy2);
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
            DestroyEnviroment();// Destoy floors, door or any obstacle needed to be destroyed when wave ends
            m_activeEnemies.Clear();
            m_activeEnemies = null;
            waveFinished = true;
        }

        public void DestroyEnviroment() 
        {
            foreach (GameObject go in objectsToDestroy) { Destroy(go); } 
        }
    }

    public List<CombatWave> listWaves = new List<CombatWave>();

    // Wave Start Paramentetrs
    [SerializeField] bool m_IsStartOnCollision;
    [TagDropdown] public string[] collisionTagList = new string[] { };

    // Wave Manegmnent parameters
    int m_CurrentWaveIndex = 0;
    Coroutine m_CurrentWave;

    // Saving
    public class Arena_SL : object_SL
    {
        public int lastWave;
    }
    Arena_SL m_Save;

    [SerializeField] bool m_IsSave;
    bool m_hasSaved = false;

    private void OnEnable()
    {
        if (m_IsSave)
        {
            m_hasSaved = false;
            World_Save_Load saveLoad = GameObject.Find("GameManager")?.GetComponent<World_Save_Load>();

            object_SL nameObj = new object_SL
            {
                // Generic objects attributes
                objectName = this.gameObject.name,
                objectID = this.gameObject.transform.GetSiblingIndex(),
                objectType = object_SL.ObjectType.COMBAT_ARENA,
            };

            m_Save = (Arena_SL)saveLoad.LoadObject(nameObj);

            if (m_Save != null)
            {
                m_CurrentWaveIndex = m_Save.lastWave;
                for (int i = 0; i < m_CurrentWaveIndex; i++)
                {
                    listWaves[i].DestroyEnviroment();
                }
                m_hasSaved = true;

            }
        }
    }

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
        if(m_CurrentWaveIndex >= listWaves.Count && m_IsSave && !m_hasSaved) 
        {
            World_Save_Load saveLoad = GameObject.Find("GameManager")?.GetComponent<World_Save_Load>();

            m_Save = new Arena_SL
            {
                // Generic objects attributes
                objectName = this.gameObject.name,
                objectID = this.gameObject.transform.GetSiblingIndex(),
                objectType = object_SL.ObjectType.COMBAT_ARENA,

                // Specific object atributes
                lastWave = m_CurrentWaveIndex,

            };
            saveLoad.SaveObject(m_Save);

            m_hasSaved = true;
        }
    }

    void StartCombat()
    {
        if (m_CurrentWaveIndex < listWaves.Count)
        {
            m_CurrentWave = StartCoroutine(listWaves[m_CurrentWaveIndex].ManageWave());
        }
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
