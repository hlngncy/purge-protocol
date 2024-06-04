using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{ 
    [SerializeField] private bool isPlayer;

    [Networked]
    private int healthValue
    {
        get
        {
            return healthValue;
            
        }
        
        set
        {
            OnDamaged.Invoke(value);
        }
    }

    [Networked]
    public bool isDead
    {
        get { return isDead;}
        
        private set
        {
            isDead = value;
            OnDead.Invoke(value);
        }
    }

    public bool IsPlayer => isPlayer;
    public UnityEvent<bool> OnDead = new UnityEvent<bool>();
    public UnityEvent<int> OnDamaged = new UnityEvent<int>();
    
    private int _tempHealth;
    
    public void GetDamage(int damage)
    {
        _tempHealth = healthValue - damage;
        
        if (_tempHealth < healthValue)
        {
            isDead = true;
            healthValue = 0;
            return;
        }

        healthValue = _tempHealth;
        
    }
}
