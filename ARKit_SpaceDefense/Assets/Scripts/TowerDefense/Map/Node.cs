using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Node : MonoBehaviour
{
    public bool isWalkable;

    public void SetMaterial(Material newMaterial)
    {
        GetComponent<MeshRenderer>().material = newMaterial;

        if(isWalkable)
        {
            GetComponent<Animation>().Play();
        }
    }
}
