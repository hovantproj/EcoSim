using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class deerScript : Animal, IIsland, IAnimal
{
    public enum states
    {
        IDLE = 0,
        SCARED,
        HUNGRY,
        FREAKY
    }

    float IAnimal.MaxHunger => MaxHunger; // The cool arrow thing means its read only
    float IAnimal.MoveSpeed => MoveSpeed;
    float IAnimal.Eyesight => Eyesight;

    public states CurrentState; // Sets to idle
    private float Radius = 5f;
    private bool StandingStill = false;
    private float StandingStillTimer = 0f;
    private float StandingStillDuration = 2f; // In seconds
    private bool Inherit = false; // If true, the deer will inherit the stats of its parents

    // DO not change these these are timers (And freak stuff)
    private float DeerCooldown; // Freak cooldown in seconds
    private float DeerTimer; // Just a running timer I will be using for freak logic
    private float FreakTime; // Time of last freak

    public override void Wander()
    {
        /**
        Moves the deer over across to the goal position at their Speed, also has opportunity to stand still

        @params: none

        @returns: none (procedure)
        **/

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
        /**
        Makes deer approach the grass (goal pos) to eat it

        @params: none

        @returns: none (procedure)
        **/

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

        else
        {
            Wander();
        }
    }

    public void Flee()
    {
        /**
        Makes the deer run away from nearby wolves

        @params: none

        @returns: none (procedure)
        **/

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Eyesight);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Wolf"))
            {
                Vector3 DangerPos = hit.transform.position;
                Vector3 FleeDir;

                FleeDir = (transform.position - DangerPos).normalized; // Runs directly away

                if (!movementModule.Validate_Pos(transform.position + FleeDir * Eyesight))
                {
                    Vector3 leftDir = Quaternion.Euler(0, -90, 0) * FleeDir; // Try side directions if it cant go directly away
                    Vector3 rightDir = Quaternion.Euler(0, 90, 0) * FleeDir;

                    if (movementModule.Validate_Pos(transform.position + leftDir * Eyesight))
                    {
                        TargetPos = transform.position + leftDir * Eyesight;
                    }
                    else if (movementModule.Validate_Pos(transform.position + rightDir * Eyesight))
                    {
                        TargetPos = transform.position + rightDir * Eyesight;
                    }
                }
                else
                {
                    TargetPos = transform.position + FleeDir * Eyesight;
                }

                break;
            }
        }
    }

    public override void Reproduce()
    {

        /**
        Makes the deer look for another deer to reproduce with, and when it does it sends across the class stats to the ecosystem spawner for similar traits to parents

        @params: none

        @returns: none (procedure)
        **/

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
                // Momentarily stand still
                TargetPos = transform.position;

                var Ecosystem = GameObject.Find("EcosystemManager").GetComponent<ecosystemScript>();
                float[] inheritStats = new float[] { (this.MaxHunger + nearestDeer.GetComponent<deerScript>().MaxHunger + Random.Range(-5, 5)) / 2,
                                                    (this.MoveSpeed + nearestDeer.GetComponent<deerScript>().MoveSpeed + Random.Range(-0.1f, 0.1f)) / 2,
                                                    (this.Eyesight + nearestDeer.GetComponent<deerScript>().Eyesight + Random.Range(-0.5f, 0.5f)) / 2};
                Ecosystem.Spawn("Deer", Random.Range(1, 3), transform.position, inheritStats); // Up to 2 deer
                FreakTime = DeerTimer; // Updates last freak time
            }
        }
        else
        {
            Wander();
        }
    }

    public void Setup(float MaxHunger, float MoveSpeed, float Eyesight) // From IIsland (If its an offspring)
    {
        /**
        Sets up the stats of the object (for offspring only)

        @params: MaxHunger, MoveSpeed, Eyesight

        @returns: none
        **/

        print("Deer inherited stats");
        Inherit = true;
        this.MaxHunger = MaxHunger;
        this.MoveSpeed = MoveSpeed;
        this.Eyesight = Eyesight;
        Hunger = MaxHunger; // Start with full hunger
    }

    public states Get_State()
    {
        /**
        Gets the state that the object should be in

        @params: none

        @returns: state (the state the object should be in)
        **/

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

        TargetPos = movementModule.Get_Random_Pos(transform.position, Radius);

        if (!Inherit)
        {
            Eyesight = Random.Range(3f, 7f);
            MoveSpeed = Random.Range(0.5f, 2f);
            MaxHunger = Random.Range(50f, 100f);
            Hunger = MaxHunger;
        }
    }

    void Update()
    {
        Vector3 crossProduct = Vector3.Cross(transform.forward, (TargetPos - transform.position).normalized);
        if (crossProduct.y > 0.1f)
        {
            // Turn right
            transform.Find("deerSprite").localScale = new Vector3(1, 1, 1); // Normal
        }
        else if (crossProduct.y < -0.1f)
        {
            // Turn left
            transform.Find("deerSprite").localScale = new Vector3(-1, 1, 1); // Flipped
        }

        // check if deer is moving
        if (Vector3.Distance(transform.position, TargetPos) > 0.1f)
        {
            float walkAngle = Mathf.Sin(Time.time * 10f * MoveSpeed) * 10f; // Sways between 5 and -5 degrees
            transform.Find("deerSprite").rotation = Quaternion.Lerp(transform.Find("deerSprite").rotation, Quaternion.Euler(45, 0, walkAngle), Time.deltaTime * 5f);
        }
        else
        {
            transform.Find("deerSprite").rotation = Quaternion.Lerp(transform.Find("deerSprite").rotation, Quaternion.Euler(45, 0, 0), 2f); // Reset rotation when not moving
        }

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