using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class simManager : MonoBehaviour
{
    public GameObject IslandSpawns;

    [System.Serializable] // Needed for json according to documentation
    public struct Stats
    {
        public float MaxHunger;
        public float MoveSpeed;
        public float Eyesight;
    }

    [System.Serializable]
    public struct SimulationObjectData
    {
        public string Type;
        public Vector3 Position;
        public Stats ObjectStats;
    }

    [System.Serializable]
    public struct SimulationSaveData
    {
        public List<SimulationObjectData> Objects;
    }

    public List<SimulationObjectData> SimulationObjects = new List<SimulationObjectData>();

    void OnApplicationQuit()
    {
        SimulationObjects.Clear(); // Makes sure the list is empty

        if (IslandSpawns == null) // I think pros call this a sanity check or smthn
        {
            Debug.LogWarning("COULDNT FIND ISLAND SPAWN FIX THIS");
            return;
        }

        for (int i = 0; i < IslandSpawns.transform.childCount; i++) // For loop with length of no of children in ,y islandspawns 
        {
            Transform Child = IslandSpawns.transform.GetChild(i);

            var Animal = Child.GetComponent<IAnimal>(); // Gets the IAnimal inteface with all the stats stuff in it

            Stats Stat = new Stats();

            if (Animal != null)
            {
                Stat.MaxHunger = Animal.MaxHunger;
                Stat.MoveSpeed = Animal.MoveSpeed;
                Stat.Eyesight = Animal.Eyesight;
            }

            SimulationObjectData Data = new SimulationObjectData
            {
                Type = Child.name, // This will be either wolf deer or grass
                Position = Child.position,
                ObjectStats = Stat
            };

            SimulationObjects.Add(Data);
        }

        SimulationSaveData saveData = new SimulationSaveData { Objects = SimulationObjects };
        string json = JsonUtility.ToJson(saveData, true); // Uses Jsonutility
        string path = Path.Combine(Application.persistentDataPath, "ecosimData.json");
        File.WriteAllText(path, json); // Writes it to a json file

        Debug.Log($"{SimulationObjects.Count} have been saved to file at {path}");
    }
}
