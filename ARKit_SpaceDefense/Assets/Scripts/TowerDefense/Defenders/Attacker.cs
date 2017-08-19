using UnityEngine;

public class Attacker : MonoBehaviour
{
    public event System.Action<Attacker> OnDead;

    private void Die()
    {
        if(OnDead != null)
        {
            OnDead(this);
        }
    }
}
