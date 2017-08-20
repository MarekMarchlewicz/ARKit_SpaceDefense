using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private float destroyTime = 2f;

    private void OnEnable()
    {
        Invoke("Destroy", destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Attacker attacker = collision.collider.GetComponent<Attacker>();

        if(attacker != null)
        {
            attacker.TakeDamage(damage);
        }
    }

    private void Destroy()
    {
        GetComponent<TrailRenderer>().Clear();

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
