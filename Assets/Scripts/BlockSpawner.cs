using UnityEngine;

public class BlockSpawner : MonoBehaviour {

    public Transform[] spawnPoints;

    public GameObject blockPrefab;

    public float timeToSpawn = 2f;

    public float timeBetweenWaves = 1f;

    public int numbersOfWaves = 0;

    private void Start()
    {
        timeToSpawn = 2f;
        timeBetweenWaves = 1f;
    }

    // Use this for initialization
    void Update ()
    {
        if (Time.timeSinceLevelLoad >= timeToSpawn)
        {
            SpawnBlocks();
            numbersOfWaves++;
            timeToSpawn += timeBetweenWaves;
        }
        
	}

    void SpawnBlocks()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (randomIndex != i)
            {
                Instantiate(blockPrefab, spawnPoints[i]);
            }
        }
    }

    public int GetNumberOfWaves()
    {
        return numbersOfWaves;
    }

}
