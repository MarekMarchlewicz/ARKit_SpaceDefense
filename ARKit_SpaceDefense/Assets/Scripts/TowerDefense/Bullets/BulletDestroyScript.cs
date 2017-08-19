using UnityEngine;

public class BulletDestroyScript : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("Destroy", 2f);
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
