using Unity.VisualScripting;
using UnityEngine;

public static class movementModule
{
    private static BoxCollider IslandCollider;

    public static void Init(GameObject Island)
    {
        /**
        Just for sending the island across because the bounds is needed

        @params: Island

        @returns: none
        **/

        IslandCollider = Island.GetComponent<BoxCollider>();
        
        if (IslandCollider == null)
            {
                Debug.LogError("Island GameObject does not have a BoxCollider attached or it's disabled!");
            }
    }

    public static bool Validate_Pos(Vector3 TriedPos)
    {
        /**
        Validates if the position is on the island, if not returns the original position

        @params: TriedPos (where the thing is trying to go)

        @returns: true/false (validty of the pos)
        **/

        if (IslandCollider.bounds.Contains(TriedPos))
            return true;
        else
            return false;
    }
    
    public static Vector3 Get_Random_Pos(Vector3 Origin, float Radius) {
        /**
        Gets a random location within the radius, also must be valid

        @params: Origin (where the thing is), Radius (how far it can go)

        @returns: RandomPos (The valid position)
        **/
        Vector3 RandomPos;
        
        do
        {
            Vector2 RandCircle = Random.insideUnitCircle * Radius; // Random inside radius
            RandomPos = new Vector3(Origin.x + RandCircle.x, Origin.y, Origin.z + RandCircle.y);
        } while (!IslandCollider.bounds.Contains(RandomPos)); // Keeps going until valid

        return RandomPos;
    }
}
