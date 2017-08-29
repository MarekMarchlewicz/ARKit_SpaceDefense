using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
	[SerializeField]
	private Color healthyColor;

	[SerializeField]
	private Color deadColor;

    [SerializeField]
    private float rotationSpeed = 20f;

    [SerializeField]
    private AnimationCurve strengthCurve;

    [SerializeField]
    private AnimationCurve angleCurve;

    [SerializeField]
    private float maxRadius = 60f;

	public int Health { get { return health; } }
	private int health = 100;
    
    private List<Vector4> positions = new List<Vector4>();
    private List<float> positionsStartTime = new List<float>();
    private List<float> positionStrength = new List<float>();
    private List<float> positionAngle = new List<float>();

    private MaterialPropertyBlock materialPropertyBlock;

    private MeshRenderer m_MeshRenderer;

    private void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

	public void Hit(int damage, Vector3 position)
    {
        positions.Add(position);
        positionsStartTime.Add(Time.time);
        positionStrength.Add(0f);
        positionAngle.Add(0f);

		health -= damage;
    }
    
	private void Update ()
    {
        EvaluateStrengths();

        if (positions.Count > 0)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetVectorArray("_Points", positions);
            materialPropertyBlock.SetFloatArray("_PointsStrength", positionStrength);
            materialPropertyBlock.SetFloatArray("_PointsAngle", positionAngle);
            m_MeshRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        m_MeshRenderer.material.SetInt("_PointsLength", positions.Count);
        m_MeshRenderer.material.SetFloat("_Radius", Mathf.Deg2Rad * maxRadius);
		m_MeshRenderer.material.SetColor ("_RimColor", Color.Lerp (deadColor, healthyColor, health / 100f));

        transform.rotation *= Quaternion.Euler(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void EvaluateStrengths()
    {
        float curveTime = strengthCurve.keys[strengthCurve.length - 1].time;

        for(int i = positions.Count - 1; i >= 0; i--)
        {
            float currentTime = Time.time - positionsStartTime[i];

            if(currentTime > curveTime)
            {
                positions.RemoveAt(i);
                positionsStartTime.RemoveAt(i);
                positionStrength.RemoveAt(i);
                positionAngle.RemoveAt(i);

                continue;
            }

            positions[i] = Quaternion.Euler(Vector3.up * rotationSpeed * Time.deltaTime) * positions[i];
            positionStrength[i] = strengthCurve.Evaluate(currentTime);
            positionAngle[i] = angleCurve.Evaluate(currentTime) * Mathf.PI;
        }
    }
}
