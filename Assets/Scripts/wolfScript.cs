using UnityEngine;

public class wolfScript : MonoBehaviour
{
    public enum states
    {
        IDLE = 0,
        HUNGRY,
        FREAKY
    }

    [Header("Modifiables")]
    public float MoveSpeed;
    public float MaxHunger;
    public float Radius = 3f;
    public states CurrentState; // Sets to idle

    private bool StandingStill = false;
    private float StandingStillTimer = 0f;
    private float StandingStillDuration = 1f; // seconds
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
        }

        if (nearestDeer != null)
        {
            TargetPos = nearestDeer.transform.position;

            if (Vector3.Distance(nearestDeer.transform.position, transform.position) <= 1f)
            {
                Destroy(nearestDeer.gameObject); // Eats the deer
                Hunger = MaxHunger; // Full hunger maybe change later
            }
        }
    }

    public states Get_State()
    {
        if (Hunger <= 30)
        {
            return states.HUNGRY;
        }

        return states.IDLE;
    }

    void Start()
    {
        MoveSpeed = Random.Range(0.1f, 1.5f);
        MaxHunger = Random.Range(20f, 50f);
        Hunger = MaxHunger;
        TargetPos = movementModule.Get_Random_Pos(transform.position, Radius);
    }

    void Update()
    {
        movementModule.Step_Toward(transform, TargetPos, MoveSpeed); // Moves toward target

        // Hunger stuff
        if (Hunger >= 0)
        {
            Hunger -= Time.deltaTime * 2; // Hungry faster when hunting
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

            case states.FREAKY:
                // Implement freaky behavior
                break;
        }
    }
}
