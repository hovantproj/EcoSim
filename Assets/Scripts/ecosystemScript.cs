using UnityEngine;
using Unity.VisualScripting;
using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.XR;

public class ecosystemScript : MonoBehaviour, IIsland
{
    // Variables and allat
    [Header("Prefabs")] // My models/prefabs
    public GameObject GrassPrefab;
    public GameObject DeerPrefab;
    public GameObject WolfPrefab;
    public GameObject Island;
    public GameObject IslandSpawns;

    [Header("Spawn_Settings")] // Imma set how many items spawned here
    public int GrassAmt = 30; // 30 grass
    public int DeerAmt = 10; // 10 deer
    public int WolfAmt = 5; // 5 wolves
    public Vector3 SpawnArea = new Vector3(3, 0, 3);

    public void Damage(float dmg) { } // Does nthing, just to satisfy the interface
    public void Setup(float MaxHunger, float MoveSpeed, float Eyesight) { } // Same here
    public void Spawn(string Name, int Count, Vector3 Pos = default, float[] Inherit = default)
    {
        /**
        Spawns the object, using the name to determine what to spawn

        @params: Name (what to spawn), Count (how many), Pos (where to spawn, default is random), Inherit (stats to inherit, default is none)

        @returns: none
        **/

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

            if (Inherit != null && Inherit.Length == 4)
            {
                float MaxHunger = Inherit[0];
                float MoveSpeed = Inherit[1];
                float Eyesight = Inherit[2];

                var islandComponent = Island.GetComponent<IIsland>();
                var animalComponent = Object.GetComponent<Animal>() as IIsland;

                if (islandComponent != null && animalComponent != null)
                {
                    animalComponent.Setup(MaxHunger, MoveSpeed, Eyesight);
                }
            }
        }
    }
    
    Vector3 GetPosition()
    {
        /**
        Gets a position within the spawn area

        @params: none

        @returns: chosenPos (the position to spawn at)
        **/

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
        StartCoroutine(SpawnGrass()); // Just keeps spawning grass
    }

    private System.Collections.IEnumerator SpawnGrass()
    {
        /**
        Keeps spawning grass at random interval bvetween 1 and 5 seconds

        @params: none

        @returns: none
        **/

        while (true)
        {
            float delay = Random.Range(1f, 5f);
            yield return new WaitForSeconds(delay);

            Spawn("Grass", 1);
        }
    }
}
