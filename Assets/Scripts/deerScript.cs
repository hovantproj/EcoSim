using UnityEngine;

public class deerScript : Animal, IIsland
{
    public enum states
    {
        IDLE = 0,
        SCARED,
        HUNGRY,
        FREAKY
    }

    public states CurrentState; // Sets to idle
    private float Radius = 5f;
    private bool StandingStill = false;
    private float StandingStillTimer = 0f;
    private float StandingStillDuration = 2f; // In seconds

    // DO not change these these are timers (And freak stuff)
    private float DeerCooldown; // Freak cooldown in seconds
    private float DeerTimer; // Just a running timer I will be using for freak logic
    private float FreakTime; // Time of last freak

    public override void Wander()
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

    public override void Eat()
    {
        // Gets all the grass
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Eyesight);
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
                return;
            }
        }
    }

    public void Flee()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Eyesight);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Wolf"))
            {
                Vector3 DangerPos = hit.transform.position;
                Vector3 FleeDir = (transform.position - DangerPos).normalized;
                TargetPos = movementModule.Validate_Pos(transform.position + FleeDir * Eyesight, transform.position);
            }
        }
    }

    public override void Reproduce()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Eyesight);
        Collider nearestDeer = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Deer") && hit.gameObject != this.gameObject)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestDeer = hit;
                }
            }
        }

        if (nearestDeer != null)
        {
            TargetPos = nearestDeer.transform.position;

            if (Vector3.Distance(nearestDeer.transform.position, transform.position) <= 1f)
            {
                var Ecosystem = GameObject.Find("EcosystemManager").GetComponent<ecosystemScript>();
                Ecosystem.Spawn("Deer", Random.Range(1, 3), transform.position); // Up to 2 deer
                FreakTime = DeerTimer; // Updates last freak time
                return;
            }
        }
        else
        {
            Wander();
        }
    }

    public void Damage(float dmg)
    {
        Health -= dmg;
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

        if (Hunger <= MaxHunger / 2)
        {
            return states.HUNGRY;
        }

        if (DeerTimer - FreakTime >= DeerCooldown) // Cooldown for freaking out
        {
            return states.FREAKY;
        }

        return states.IDLE;
    }

    void Start()
    {
        DeerTimer = 0;
        FreakTime = 0;
        DeerCooldown = Random.Range(20f, 40f); // Cooldown between 20 and 40 seconds

        Eyesight = Random.Range(3f, 7f);
        Health = Random.Range(10f, 50f);
        MoveSpeed = Random.Range(0.5f, 2f);
        MaxHunger = Random.Range(50f, 100f);
        Hunger = MaxHunger;
        TargetPos = movementModule.Get_Random_Pos(transform.position, Radius);
    }

    void Update()
    {
        CurrentState = Get_State(); // Updates state
        DeerTimer += Time.deltaTime;
        Move(TargetPos); // Use the base class Move method

        // Hunger stuff
        if (Hunger >= 0)
        {
            Hunger -= Time.deltaTime; // hungry
        }

        if (Hunger <= 0)
        {
            Die(); // Use the base class Die method
        }

        switch (CurrentState) // Enums hooray
        {
            case states.IDLE:
                Wander();
                break;

            case states.HUNGRY:
                Eat(); // Use the overridden Eat method
                break;

            case states.SCARED:
                Flee();
                break;

            case states.FREAKY:
                Reproduce(); // Use the overridden Reproduce method
                break;
        }
    }
}