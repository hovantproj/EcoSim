using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Timeline;

public class deerScript : MonoBehaviour
{
    public enum states
    {
        IDLE = 0,
        SCARED,
        HUNGRY,
        FREAKY
    }

    public ecosystemScript ecosystem;

    [Header("Modifiables")]
    public float MoveSpeed;
    public float MaxHunger;
    public float Health;
    public float Radius = 5f;
    public states CurrentState; // Sets to idle

    private bool StandingStill = false;
    private float StandingStillTimer = 0f;
    private float StandingStillDuration = 2f; // seconds
    private Vector3 TargetPos;
    public float Hunger;

    public void Wander()
    {
        if (!StandingStill)
        {
            // Only decide to stand still when reaching the target
            if (Vector3.Distance(transform.position, TargetPos) <= 1f)
            {
                if (Random.value < 0.5f) // Chance to stand still
                {
                    TargetPos = transform.position;
                    StandingStill = true;
                    StandingStillTimer = 0f;
                }
                else
                {
                    TargetPos = movementModule.Get_Random_Pos(transform.position, Radius);
                }
            }
        }
        else
        {
            StandingStillTimer += Time.deltaTime;
            if (StandingStillTimer >= StandingStillDuration)
            {
                TargetPos = movementModule.Get_Random_Pos(transform.position, Radius);
                StandingStill = false;
            }
        }
    }

    public void Find_Food()
    {
        // Gets all the grass
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        Collider nearestGrass = null;
        float minDist = Mathf.Infinity; // Arbitrary big value

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Grass"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestGrass = hit;
                }
            }
            else
            {
                Wander();
            }
        }

        if (nearestGrass != null)
        {
            TargetPos = nearestGrass.transform.position;
            
            if (Vector3.Distance(nearestGrass.transform.position, transform.position) <= 1f)
            {
                Destroy(nearestGrass.gameObject); // Eats the grass
                Hunger = MaxHunger; // Full hunger maybe change later
            }
        }
    }

    public void Flee()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Wolf"))
            {
                Vector3 DangerPos = hit.transform.position;
                Vector3 FleeDir = (transform.position - DangerPos).normalized;
                TargetPos = movementModule.Validate_Pos(transform.position + FleeDir * Radius, transform.position);
            }
        }
    }

    public void Freak()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        Collider nearestDeer = null;
        float minDist = Mathf.Infinity; // Arbitrary big value

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Deer"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestDeer = hit;
                }
            }
            else
            {
                Wander();
            }

            if (nearestDeer != null)
            {
                TargetPos = nearestDeer.transform.position;

                if (Vector3.Distance(nearestDeer.transform.position, transform.position) <= 1f)
                {
                    ecosystem.Spawn(ecosystem.DeerPrefab, 1, transform.position + new Vector3(1,0,1));
                }
            }
        }
    }

    public states Get_State()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Wolf"))
            {
                return states.SCARED;
            }
        }

        if (Hunger <= 30)
        {
            return states.HUNGRY;
        }

        return states.IDLE;
    }

    void Start()
    {
        MoveSpeed = Random.Range(0.5f, 2f);
        MaxHunger = Random.Range(50f, 100f);
        Hunger = MaxHunger;
        TargetPos = movementModule.Get_Random_Pos(transform.position, Radius);
    }

    void Update()
    {
        movementModule.Step_Toward(transform, TargetPos, MoveSpeed); // Moves toward target

        // Hunger stuff
        if (Hunger >= 0)
        {
            Hunger -= Time.deltaTime; // hungry
        }

        CurrentState = Get_State(); // Updates state

        switch (CurrentState) // Enums hooray
        {
            case states.IDLE:
                Wander();
                break;

            case states.HUNGRY:
                Find_Food();
                break;

            case states.SCARED:
                Flee();
                break;

            case states.FREAKY:
                // Implement freaky behavior
                break;
        }
    }
}
