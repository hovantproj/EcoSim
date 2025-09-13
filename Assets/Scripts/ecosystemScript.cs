using UnityEngine;
using Unity.Mathematics;

public class ecosystemScript : MonoBehaviour
{
    // Variables and allat
    [Header("Prefabs")] // My models/prefabs
    public GameObject GrassPrefab;
    public GameObject DeerPrefab;
    public GameObject WolfPrefab;

    [Header("Settings")] // Imma set how many items spawned here
    public int GrassAmt = 30; // 50 grass
    public int DeerAmt = 10; // 10 deer
    public int WolfAmt = 3; // 3 wolves
    public Vector3 SpawnArea = new Vector3(3, 0, 3);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() // Basically my main/entry point
    {
        Spawn(GrassPrefab, GrassAmt);
        Spawn(DeerPrefab, DeerAmt);
        Spawn(WolfPrefab, WolfAmt);
    }

    Vector3 GetPosition()
    {
        // Getting the random positions of the spawn in allowable area
        // Needs unityengine.random rather than just random because
        float RandomX = UnityEngine.Random.Range(-SpawnArea.x / 2, SpawnArea.x / 2);
        float RandomZ = UnityEngine.Random.Range(-SpawnArea.z / 2, SpawnArea.z / 2);

        Vector3 ChosenPos = new Vector3(RandomX, 0, RandomZ);
        print(ChosenPos);

        return ChosenPos;
    }

    public void Spawn(GameObject Object, int Count)
    {
        for (int i = 0; i < Count; i++)
        {
            Vector3 SpawnPos = GetPosition();
            GameObject NewObj = Instantiate(Object, SpawnPos, quaternion.identity);
        }
    }
}
