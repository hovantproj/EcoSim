using Unity.VisualScripting;
using UnityEngine;

public static class movementModule
{
    private static Collider IslandCollider;

    public static void Init(GameObject Island)
    {
        IslandCollider = Island.GetComponent<Collider>();
        if (IslandCollider == null)
            Debug.LogError("Island error"); // Gives an error
    }

    public static void Move_To(Transform Object, Vector3 Target, float Speed)
    {
        /**
        Moves a thing over across to the goal position at their Speed

        @params: Obj (The thing), Target (where I want it to go), Speed (Speed obv)

        @returns: none (procedure)
        **/
        Vector3 Direction = (Target - Object.position).normalized; // Sets the length at 1 (normalize)
        Object.position += Direction * Speed * Time.deltaTime; // Moves it in the Direction of "Direction" at "Speed" every "deltaTiem"
    }

    public static Vector3 Get_Random_Pos(Vector3 Origin, float Radius) {
        /**
        Gets a random location within the radius, also must be valid

        @params: Origin (where the thing is), Radius (how far it can go)

        @returns: RandomPos (The valid position)
        **/
        Vector2 RandCircle = Random.insideUnitCircle * Radius; // Random inside radius
        Vector3 RandomPos;

        do
        {
            RandomPos = new Vector3(Origin.x + RandCircle.x, Origin.y, Origin.z + RandCircle.y);
        } while (!IslandCollider.bounds.Contains(RandomPos)); // Keeps going until valid


        return RandomPos;
    }
}
