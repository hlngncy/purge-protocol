using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AgentStats", order = 1)]
public class AgentStatsSO : ScriptableObject
{
    
    [SerializeField] private ushort damage;
    [SerializeField] private ushort fireRate;
    [SerializeField] private short health;
    [SerializeField] private int maxMovementSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float jumpImpulse;

    public ushort Damage
    {
        get => damage;
        set => damage = value;
    }
    public ushort FireRate
    {
        get => fireRate;
        set => fireRate = value;
    }
    public short Health
    {
        get => health;
        set => health = value;
    }
    public int MaxMovementSpeed
    {
        get => maxMovementSpeed;
        set => maxMovementSpeed = value;
    }
    public float Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }
    public float JumpImpulse
    {
        get => jumpImpulse;
        set => jumpImpulse = value;
    }
    

}
