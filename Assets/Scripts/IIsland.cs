using UnityEngine;

public interface IIsland // The I stands for interface btw
{
    void Damage(float dmg);
    void Setup(float MaxHunger, float MoveSpeed, float Eyesight, float Health);
}


