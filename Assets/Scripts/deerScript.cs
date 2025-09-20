using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

enum states
{
    idle = 0,
    fleeing,
    hungry,
    freaky
}

public class deerScript : MonoBehaviour, IIsland
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

    public void Init(GameObject IslandReference)
    {
        Island = IslandReference;
    }

    public Vector3 Get_Valid_Loc()
    {
        return TargetPos;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TargetPos = movementModule.Get_Random_Pos(transform.position, Radius); // Gets where it wants to move
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;    

        movementModule.Move_To(transform, TargetPos, MoveSpeed);

        if (Vector3.Distance(TargetPos, transform.position) <= 1f || Timer > WanderTime)
        {
            TargetPos = movementModule.Get_Random_Pos(transform.position, Radius);
            Timer = 0f;

            movementModule.Move_To(transform, TargetPos, MoveSpeed);
        }
    }
}
