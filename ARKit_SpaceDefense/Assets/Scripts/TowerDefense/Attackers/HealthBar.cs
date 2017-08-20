using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Transform greenIndicator;

    private Transform cameraTransform;

    private Transform m_Transform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        m_Transform = transform;
    }

    public void UpdateBar(float val)
    {
        Vector3 newScale = greenIndicator.localScale;
        newScale.x = val;

        greenIndicator.localScale = newScale;
    }

    private void Update()
    {
        m_Transform.forward = -cameraTransform.forward;
    }
}
