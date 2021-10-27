using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteDungeon : MonoBehaviour
{
    [SerializeField] List<GameObject> obstaclePrefabs;
    [SerializeField] float obstacleGap;
    [SerializeField] float healthPickupChance;

    private LinkedList<GameObject> dungeonChunks;
    private GameObject player;

    private float dungeonChunkLength;


    void Start()
    {
        // Find Dungeon Chunks
        dungeonChunks = new LinkedList<GameObject>();
        for (var i = 0; i < transform.childCount; ++i)
            dungeonChunks.AddLast(transform.GetChild(i).gameObject);

        // Get the length of one dungeonChunk, especially the z length, to perform calculation later
        // This is found via
        //      dungeonChunks[0].transform > Structure.transform > Floor.transform > ViewSize.size
        dungeonChunkLength = dungeonChunks.First.Value.transform.Find("Structure").Find("Floor").GetComponent<ViewSize>().size.z;


        // Initially, every chunk except the first one will have obstacles generated.
        bool isFirst = true;
        foreach (var dungeonChunk in dungeonChunks)
        {
            if (isFirst) isFirst = false;
            else generateNewObstacles(dungeonChunk);
        }
    }


    public void generateNewChunk()
    {
        GameObject passedDungeonChunk = dungeonChunks.First.Value;
        dungeonChunks.RemoveFirst();
        passedDungeonChunk.transform.position = new Vector3(passedDungeonChunk.transform.position.x,
                                                            passedDungeonChunk.transform.position.y,
                                                            dungeonChunks.Last.Value.transform.position.z + dungeonChunkLength);
        dungeonChunks.AddLast(passedDungeonChunk);

        // Generate obstacles
        generateNewObstacles(passedDungeonChunk);
        spawnHealthPickup(passedDungeonChunk);
    }


    void spawnHealthPickup(GameObject dungeonChunk)
    {
        GameObject healthPickup = dungeonChunk.transform.Find("HealthPickups").gameObject;
        healthPickup.transform.GetChild(0).gameObject.SetActive(Random.Range(0f, 1f) < healthPickupChance);
    }



    // Creates a series of new obstacles in given DungeonChunk.
    void generateNewObstacles(GameObject dungeonChunk)
    {
        GameObject obstaclesGameObject = dungeonChunk.transform.Find("Obstacles").gameObject;

        // Destroys every previous obstacle in DungeonChunk/Obstacles
        for (var i = 0; i < obstaclesGameObject.transform.childCount; ++i)
            Destroy(obstaclesGameObject.transform.GetChild(i).gameObject);

        // Create new obstacles
        for (float offset = 0; offset < dungeonChunkLength; offset += obstacleGap)
        {
            int index = Random.Range(0, obstaclePrefabs.Count);

            GameObject obstacle = Instantiate(obstaclePrefabs[index], obstaclesGameObject.transform);
            obstacle.transform.position = new Vector3(obstacle.transform.position.x, 
                                                      obstacle.transform.position.y, 
                                                      obstacle.transform.position.z + offset);
        }

    }
}
