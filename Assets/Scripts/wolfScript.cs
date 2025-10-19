using UnityEngine;

public class wolfScript : Animal, IIsland, IAnimal
{
    public enum states
    {
        IDLE = 0,
        HUNGRY,
        FREAKY
    }

    float IAnimal.MaxHunger => MaxHunger;
    float IAnimal.MoveSpeed => MoveSpeed;
    float IAnimal.Eyesight => Eyesight;

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
                    Debug.Log("Deer killed");
                    Hunger = Mathf.Min(Hunger + 20, MaxHunger);
                    Destroy(nearestDeer.gameObject);
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
                // Momentarily stand still
                TargetPos = transform.position;

                var Ecosystem = GameObject.Find("EcosystemManager").GetComponent<ecosystemScript>();
                float[] inheritStats = new float[] { (this.MaxHunger + nearestWolf.GetComponent<deerScript>().MaxHunger + Random.Range(-5, 5)) / 2,
                                                    (this.MoveSpeed + nearestWolf.GetComponent<deerScript>().MoveSpeed + Random.Range(-0.1f, 0.1f)) / 2,
                                                    (this.Eyesight + nearestWolf.GetComponent<deerScript>().Eyesight + Random.Range(-0.5f, 0.5f)) / 2};
                Ecosystem.Spawn("Wolf", 1, transform.position, inheritStats);
                FreakTime = WolfTimer;
            }
        }
    }

    public void Setup(float MaxHunger, float MoveSpeed, float Eyesight)
    {
        /**
        Sets up the stats of the object (for offspring only)

        @params: MaxHunger, MoveSpeed, Eyesight

        @returns: none
        **/

        this.MaxHunger = MaxHunger;
        this.MoveSpeed = MoveSpeed;
        this.Eyesight = Eyesight;
        this.Hunger = MaxHunger; // Start with full hunger
    }

    private states Get_State()
    {
        /**
        Gets the state that the object should be in

        @params: none

        @returns: state (the state the object should be in)
        **/

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
        Eyesight = Random.Range(3f, 5f);

        WolfTimer = 0;
        FreakTime = 0;
        WolfCooldown = Random.Range(40f, 50f);
        Hunger = MaxHunger;
    }

    void Update()
    {
        Vector3 crossProduct = Vector3.Cross(transform.forward, (TargetPos - transform.position).normalized);
        if (crossProduct.y > 0.1f)
        {
            // Turn right
            transform.Find("wolfSprite").localScale = new Vector3(1, 1, 1); // Normal
        }
        else if (crossProduct.y < -0.1f)
        {
            // Turn left
            transform.Find("wolfSprite").localScale = new Vector3(-1, 1, 1); // Flipped
        }

        // check if wolf is moving
        if (Vector3.Distance(transform.position, TargetPos) > 0.1f)
        {
            float walkAngle = Mathf.Sin(Time.time * 10f * MoveSpeed) * 10f; // Sways between 5 and -5 degrees
            transform.Find("wolfSprite").rotation = Quaternion.Lerp(transform.Find("wolfSprite").rotation, Quaternion.Euler(45, 0, walkAngle), Time.deltaTime * 5f);
        }
        else
        {
            transform.Find("wolfSprite").rotation = Quaternion.Lerp(transform.Find("wolfSprite").rotation, Quaternion.Euler(45, 0, 0), 2f); // Reset rotation when not moving
        }

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