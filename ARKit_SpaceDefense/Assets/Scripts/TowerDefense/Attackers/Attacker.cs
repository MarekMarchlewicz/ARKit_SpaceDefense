using UnityEngine;

public class Attacker : MonoBehaviour
{
    public event System.Action<Attacker> OnDead;
    
    public bool IsAlive { get { return Health > 0; } }

    private int health = 100;
    public int Health
    {
        get
        {
            return health;
        }
        private set
        {
            health = value;

            if (health <= 0)
            {
                health = 0;
                Die();
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (!IsAlive)
            return;

        Health -= damage;
    }

    protected virtual void Die()
    {
        if(OnDead != null)
        {
            OnDead(this);
        }
    }
}
