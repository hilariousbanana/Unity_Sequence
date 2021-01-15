using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private StageInformation stageInfo;
    private List<Transform> spawnPoints;
    private List<GameObject> enemyTypes;

    private int CurInstance = 0;
    private int MaxInstance;

    private bool bInstantiating = false;
    private bool bInit = false;

    // Start is called before the first frame update
    void Start()
    {
        stageInfo = FindObjectOfType<StageInformation>().GetComponent<StageInformation>();
        MaxInstance = stageInfo.MaxInstance;
        spawnPoints = stageInfo.EnemySpawn;
        enemyTypes = stageInfo.EnemyTypes;

        InitializeField();
    }

    // Update is called once per frame
    void Update()
    {
        if(bInit)
            InstantiateEnemy();
    }

    void InitializeField()
    {
        while(CurInstance < MaxInstance)
        {
            int rand1 = Random.Range(0, enemyTypes.Count);
            int rand2 = Random.Range(0, spawnPoints.Count);

            Instantiate(enemyTypes[rand1].gameObject, spawnPoints[rand2].position, spawnPoints[rand2].rotation);

            CurInstance++;
        }
        bInit = true;
    }

    void InstantiateEnemy()
    {
        if(CurInstance < MaxInstance && !bInstantiating)
        {
            StartCoroutine(InstantiateCoroutine());
        }
    }

    IEnumerator InstantiateCoroutine()
    {
        bInstantiating = true;
        
        yield return new WaitForSeconds(0.8f);

        int rand1 = Random.Range(0, (enemyTypes.Count + 1));
        int rand2 = Random.Range(0, (spawnPoints.Count + 1));

        Instantiate(enemyTypes[rand1], spawnPoints[rand2].position, spawnPoints[rand2].rotation);

        CurInstance++;

        bInstantiating = false;
    }
}
