using UnityEngine;

public abstract class Animal : MonoBehaviour
{
    public float Hunger;
    public float MaxHunger;
    public float MoveSpeed;
    public float Eyesight;

    protected Vector3 TargetPos; // Only derived classes can access this

    public virtual void Move(Vector3 targetPosition) // Virtual doesnt need docstrings iirc
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    }

    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    public abstract void Wander();
    public abstract void Eat();
    public abstract void Reproduce();
}