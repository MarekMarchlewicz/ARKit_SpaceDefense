using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Tower : MonoBehaviour
{
    [SerializeField, Range(1,30)]
    private float shotsPerSecond = 1f;

    private List<Attacker> attackers = new List<Attacker>();

    

    private void Update()
    {
        if(attackers.Count > 0)
        {

        }
    }
}
