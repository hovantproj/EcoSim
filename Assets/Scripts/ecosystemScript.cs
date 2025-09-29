using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using NUnit.Framework;

public class ecosystemScript : MonoBehaviour
{
    // Variables and allat
    [Header("Prefabs")] // My models/prefabs
    public GameObject GrassPrefab;
    public GameObject DeerPrefab;
    public GameObject WolfPrefab;
    public GameObject Island;
    public GameObject IslandSpawns;

    [Header("Spawn_Settings")] // Imma set how many items spawned here
    public int GrassAmt = 30; // 50 grass
    public int DeerAmt = 10; // 10 deer
    public int WolfAmt = 3; // 3 wolves
    public Vector3 SpawnArea = new Vector3(3, 0, 3);

    public void Spawn(string Name, int Count, Vector3 Pos = default)
    {
        GameObject Object;
        Vector3 SpawnPos;

        if (Name == "Deer")
        {
            Object = DeerPrefab;
        }
        else if (Name == "Wolf")
        {
            Object = WolfPrefab;
        }
        else if (Name == "Grass")
        {
            Object = GrassPrefab;
        }
        else
        {
            Debug.LogError("Invalid object name for spawning: " + Name);
            return;
        }

        for (int i = 0; i < Count; i++)
        {
            if (Pos == default)
            {
                SpawnPos = GetPosition();
            }

            else
            {
                SpawnPos = Pos;
            }

            if (Object == null)
            {
                Debug.LogError("Prefab for " + Name + " is not assigned.");
                return;
            }

           Instantiate(Object, SpawnPos, Quaternion.identity, IslandSpawns.transform);
        }
    }

    Vector3 GetPosition()
    {
        // Getting the random positions of the spawn in allowable area
        // Needs unityengine.random rather than just random because
        float RandomX = UnityEngine.Random.Range(-SpawnArea.x / 2, SpawnArea.x / 2);
        float RandomZ = UnityEngine.Random.Range(-SpawnArea.z / 2, SpawnArea.z / 2);

        Vector3 ChosenPos = new Vector3(RandomX, 0, RandomZ);

        return ChosenPos;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() // Basically my main/entry point
    {
        movementModule.Init(Island);
        Spawn("Grass", GrassAmt);
        Spawn("Deer", DeerAmt);
        Spawn("Wolf", WolfAmt);
    }
}
