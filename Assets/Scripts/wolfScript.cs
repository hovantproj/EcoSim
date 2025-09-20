using UnityEngine;

public class wolfScript : MonoBehaviour, IIsland
{
    [Header("Modifiables")]
    public float MoveSpeed = 1f;
    public float Hunger;
    public float Metabolism;
    // Movement stuff
    public float Radius = 10f; // 20 units will be tge Radius
    public float WanderTime = 4f; // How long before it starts exploreing again
    private Vector3 TargetPos;
    private float Timer;
    private GameObject Island;

    public void Init(GameObject IslandRef)
    {
        Island = IslandRef;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
