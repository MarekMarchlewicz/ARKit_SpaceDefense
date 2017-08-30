using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class Map : MonoBehaviour
{
    private  enum Direction
    {
        Forward,
        Right,
        Left
    }

    [Header("Map Generation")]

    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    [SerializeField]
    private float size;

    [SerializeField]
    private Material walkableMaterial;

    [SerializeField]
    private Material nonWalkableMaterial;

    [SerializeField]
    private GameObject node;

    [SerializeField]
    private float generationTime = 3f;

    [Header("Sounds")]

    [SerializeField]
    private float minPitch;

    [SerializeField]
    private float maxPitch;
    
    [SerializeField]
    private AudioClip tickSfx;

    [SerializeField]
    private AudioClip finishedSfx;

    [Header("NavMesh")]

    [SerializeField]
    private NavMeshSurface navMeshSurface;
    
    private List<Node> nodes = new List<Node>();
    private List<Node> walkableNodes = new List<Node>();

    public Vector3 DefendersPosition 
	{ 
		get 
		{ 
			return GetPositionOnNavmesh(walkableNodes[0].transform.position + Vector3.up * 0.5f); 
		} 
	}

    public Vector3 AttackersPosition 
	{ 
		get 
		{ 
			return GetPositionOnNavmesh(walkableNodes [walkableNodes.Count - 1].transform.position + Vector3.up * 0.5f);
		} 
	}

	private bool isBusy = false;
	public bool IsBusy { get { return isBusy; } }

    private Transform m_Transform;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
        m_AudioSource = GetComponent<AudioSource>();
    }

	public void GenerateNew()
	{
		RemoveOld ();

		Create();
		SetRandomPath();

		StartCoroutine(GenerationEffect());
	}

	private void RemoveOld()
	{
		navMeshSurface.RemoveData ();

		for (int i = 0; i < nodes.Count; i++) 
		{
			Destroy(nodes [i].gameObject);
		}

		nodes.Clear ();
		walkableNodes.Clear ();
	}

    private void Create()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = Vector3.right * (x - (width - 1) / 2f) * size + Vector3.forward * (y - (height - 1) / 2f) * size;

                GameObject newNodeGO = Instantiate(node);
                newNodeGO.name = "Node" + (x + y * width).ToString();
				newNodeGO.transform.parent = m_Transform;
				newNodeGO.transform.localPosition = position;
				newNodeGO.transform.localRotation = Quaternion.identity;
				newNodeGO.transform.localScale = Vector3.one;

                Node newNode = newNodeGO.GetComponent<Node>();
                newNode.isWalkable = false;
                newNode.SetMaterial(nonWalkableMaterial);
                nodes.Add(newNode);
            }
        }
    }

    private void SetRandomPath()
    {
        int currentHeight = 0;

        int currentWidth = Random.Range(0, width);

        nodes[currentWidth + currentHeight * width].isWalkable = true;
        walkableNodes.Add(nodes[currentWidth + currentHeight * width]);

        while (currentHeight < height - 1)
        {
            Direction newDirection = GetRandomDirection(currentWidth, currentHeight);

            if (newDirection == Direction.Forward)
                currentHeight++;
            else if (newDirection == Direction.Left)
                currentWidth--;
            else if (newDirection == Direction.Right)
                currentWidth++;

            nodes[currentWidth + currentHeight * width].isWalkable = true;
            walkableNodes.Add(nodes[currentWidth + currentHeight * width]);
        }
    }

    private List<Direction> tempDirections = new List<Direction>();

    private Direction GetRandomDirection(int currentWidth, int currentHeight)
    {
        tempDirections.Clear();

        for (int i = 0; i < System.Enum.GetNames(typeof(Direction)).Length; i++)
        {
            Direction newDirection = (Direction)i;

            if (newDirection == Direction.Right && !CanGoRight(currentWidth, currentHeight, newDirection))
                continue;

            if (newDirection == Direction.Left && !CanGoLeft(currentWidth, currentHeight, newDirection))
                continue;
            
            tempDirections.Add(newDirection);
        }

        return tempDirections[Random.Range(0, tempDirections.Count)];
    }

    private bool CanGoRight(int currentWidth, int currentHeight, Direction newDirection)
    {
        // is it at the edge?
        if (currentWidth >= width - 2)
            return false;
        
        // is the one on the right walkable?
        if (nodes[Mathf.Clamp(currentWidth + 1, 0, width - 1) + currentHeight * width].isWalkable)
            return false;
        // is the lower-right one walkable?
        if (nodes[currentWidth + 1 + Mathf.Clamp((currentHeight - 1), 0, height - 1) * width].isWalkable)
            return false;

        return true;
    }

    private bool CanGoLeft(int currentWidth, int currentHeight, Direction newDirection)
    {
        // is it at the edge?
        if (currentWidth <= 1)
            return false;
        // is the one on the left walkable?
        if (nodes[Mathf.Clamp(currentWidth - 1, 0, width - 1) + currentHeight * width].isWalkable)
            return false;
        // is the lower-left one walkable?
        if (nodes[currentWidth - 1 + Mathf.Clamp((currentHeight - 1), 0, height - 1) * width].isWalkable)
            return false;

        return true;
    }

    private IEnumerator GenerationEffect()
    {
		isBusy = true;

		yield return new WaitForSeconds(generationTime / walkableNodes.Count);

		BakeNavMesh();

        for(int i = 0; i < walkableNodes.Count; i++)
        {
            float pitch = Mathf.Lerp(minPitch, maxPitch, i / (walkableNodes.Count - 1));

            m_AudioSource.pitch = pitch;
            m_AudioSource.PlayOneShot(tickSfx);

            walkableNodes[i].SetMaterial(walkableMaterial);
            
            yield return new WaitForSeconds(generationTime / walkableNodes.Count);
        }

        m_AudioSource.pitch = 1f;
        m_AudioSource.PlayOneShot(finishedSfx);

		isBusy = false;
    }

    private void BakeNavMesh()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isWalkable)
            {
                nodes[i].GetComponent<Collider>().enabled = false;
            }
        }
			
        navMeshSurface.Bake();

        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].GetComponent<Collider>().enabled = true;
        }
    }

	private Vector3 GetPositionOnNavmesh(Vector3 positionToSample)
	{
		NavMeshHit hit;
		if (NavMesh.SamplePosition (positionToSample, out hit, 5f, NavMesh.AllAreas)) 
		{
			return hit.position;
		} 
		else 
		{
			Debug.LogError ("Can't find position on navmesh");

			return positionToSample;
		}
	}
}
