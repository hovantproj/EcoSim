using UnityEngine;

public class wolfScript : Animal
{
    public enum states
    {
        IDLE = 0,
        HUNGRY,
        FREAKY
    }

    public states CurrentState;
    private float Radius = 5f;
    private bool StandingStill = false;
    private float StandingStillTimer = 0f;
    private float StandingStillDuration = 1f;


    // DO not change these these are timers (And freak stuff)
    private float WolfCooldown;
    private float WolfTimer;
    private float FreakTime;

    public override void Wander()
    {
        if (!StandingStill)
        {
            if (Vector3.Distance(transform.position, TargetPos) <= 1f)
            {
                if (Random.value < 0.3f) // Chance to stand still
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Eyesight);
        Collider nearestDeer = null;
        float minDist = Mathf.Infinity;

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
        }

        if (nearestDeer != null)
        {
            TargetPos = nearestDeer.transform.position;

            if (Vector3.Distance(nearestDeer.transform.position, transform.position) <= 1f)
            {
                var deer = nearestDeer.GetComponent<deerScript>();
                if (deer != null)
                {
                    deer.Damage(10f);

                    if (deer.Health <= 0)
                    {
                        Debug.Log("Deer killed");
                        Hunger = Mathf.Min(Hunger + 20, MaxHunger);
                        Destroy(nearestDeer.gameObject);
                    }
                }
            }
        } else
        {
            Wander();
        }
    }

    public override void Reproduce() // Tge freaky state
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Eyesight);
        Collider nearestWolf = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Wolf") && hit.gameObject != this.gameObject)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestWolf = hit;
                }
            }
        }

        if (nearestWolf != null)
        {
            TargetPos = nearestWolf.transform.position;

            if (Vector3.Distance(nearestWolf.transform.position, transform.position) <= 1f)
            {
                var Ecosystem = GameObject.Find("EcosystemManager").GetComponent<ecosystemScript>();
                Ecosystem.Spawn("Wolf", 1, transform.position);
                Debug.Log("Wolf reproduced.");
                FreakTime = WolfTimer;
            }
        }
    }

    public void Setup(float MaxHunger, float MoveSpeed, float Eyesight, float Health)
    {
        this.MaxHunger = MaxHunger;
        this.MoveSpeed = MoveSpeed;
        this.Eyesight = Eyesight;
        this.Health = Health;
        this.Hunger = MaxHunger; // Start with full hunger
    }

    private states Get_State()
    {
        if (Hunger <= 30)
        {
            return states.HUNGRY;
        }

        if (WolfTimer - FreakTime >= WolfCooldown)
        {
            return states.FREAKY;
        }

        return states.IDLE;
    }

    void Start()
    {
        TargetPos = movementModule.Get_Random_Pos(transform.position, Radius);
        MoveSpeed = Random.Range(0.5f, 3f);
        MaxHunger = Random.Range(35f, 50f);
        Eyesight = Random.Range(3f, 7f);

        WolfTimer = 0;
        FreakTime = 0;
        WolfCooldown = Random.Range(20f, 40f);
        Hunger = MaxHunger;
    }

    void Update()
    {
        CurrentState = Get_State();
        WolfTimer += Time.deltaTime;
        Hunger -= Time.deltaTime;

        if (Hunger <= 0)
        {
            Die();
            Debug.Log("Wolf died of hunger.");
        }

        Move(TargetPos);

        switch (CurrentState)
        {
            case states.IDLE:
                Wander();
                break;
            case states.HUNGRY:
                Eat();
                break;
            case states.FREAKY:
                Reproduce();
                break;
        }
    }
}