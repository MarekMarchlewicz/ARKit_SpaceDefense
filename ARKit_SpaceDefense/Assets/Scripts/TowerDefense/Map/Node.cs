using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Node : MonoBehaviour
{
    public bool walkable;

    public void SetMaterial(Material newMaterial)
    {
        GetComponent<MeshRenderer>().material = newMaterial;

        if(walkable)
        {
            GetComponent<Animation>().Play();
        }
    }
}
