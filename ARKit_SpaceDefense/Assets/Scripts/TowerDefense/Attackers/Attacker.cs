using UnityEngine;

public class Attacker : MonoBehaviour
{
    public event System.Action<Attacker> OnDead;

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
        Health -= damage;
    }

    private void Die()
    {
        if(OnDead != null)
        {
            OnDead(this);
        }
    }
}
