using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class simManager : MonoBehaviour
{
    public bool Save = false;
    public Button SaveButton;
    public Button LoadButton;

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

    private List<SimulationObjectData> SimulationObjects = new List<SimulationObjectData>();
    
    public void ToggleSave()
    {
        /**
        Toggles the save boolean and changes colour of button

        @params: none

        @returns: none
        **/

        Save = !Save; // Awesome little trick which realisitcally saves 0 lines of code anyways

        if (Save == true)
        {
            SaveButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            SaveButton.GetComponent<Image>().color = Color.red;
        }
    }

    private void SaveSim()
    {
        /**
        Saves the simulation to a json

        @params: none

        @returns: none
        **/

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
                Type = Child.tag, // This will be either wolf deer or grass
                Position = Child.position,
                ObjectStats = Stat
            };

            SimulationObjects.Add(Data);
        }

        SimulationSaveData SaveData = new SimulationSaveData {Objects = SimulationObjects};
        string json = JsonUtility.ToJson(SaveData, true); // Uses Jsonutility to turn into json
        string path = Path.Combine(Application.persistentDataPath, "ecosimData.json");
        File.WriteAllText(path, json); // Writes it to a json file

        Debug.Log($"{SimulationObjects.Count} have been saved to file at {path}");
    }

    public void LoadSim()
    {
        /**
        Loads the previous simulation file

        @params: none

        @returns: none
        **/

        string path = Path.Combine(Application.persistentDataPath, "ecosimData.json");
        var Ecosystem = GameObject.Find("EcosystemManager").GetComponent<ecosystemScript>();

        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path); // All the text in the file
        SimulationSaveData LoadData = JsonUtility.FromJson<SimulationSaveData>(json); // Turns it into my struct w list agian

        Debug.Log($"Loaded {LoadData.Objects.Count} from {path}");

        for (int i = 0; i < IslandSpawns.transform.childCount; i++)
        {
            Destroy(IslandSpawns.transform.GetChild(i).gameObject); // Clears the board (deletes the stuff on it already)
        }

        for (int i = 0; i < LoadData.Objects.Count; i++)
        {
            var ObjectData = LoadData.Objects[i];
            
            string Type = ObjectData.Type;
            int Count = 1;
            Vector3 Pos = ObjectData.Position;
            float[] Inherit = new float[] { ObjectData.ObjectStats.MaxHunger,
                                            ObjectData.ObjectStats.MoveSpeed,
                                            ObjectData.ObjectStats.Eyesight};;

            Ecosystem.Spawn(Type, Count, Pos, Inherit);
        }
    }

    void OnApplicationQuit()
    {
        if (Save)
        {
            SaveSim();
        }
    }
}
