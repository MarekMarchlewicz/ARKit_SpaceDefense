using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Tower : MonoBehaviour
{
    [SerializeField, Range(1,30)]
    private float shotsPerSecond = 1f;

    private List<Attacker> attackers = new List<Attacker>();

    private void OnTriggerEnter(Collider collider)
    {
        Attacker attacker = collider.GetComponent<Attacker>();

        attackers.Add(attacker);

        attacker.OnDead += Attacker_OnDead;
    }

    private void OnTriggerExit(Collider collider)
    {
        Attacker attacker = collider.GetComponent<Attacker>();

        attackers.Remove(attacker);
    }
    
    private void Attacker_OnDead(Attacker attacker)
    {
        attackers.Remove(attacker);
    }

    private void Update()
    {
        if(attackers.Count > 0)
        {

        }
    }
}
