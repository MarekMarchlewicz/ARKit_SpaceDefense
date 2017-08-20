using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Node : MonoBehaviour
{
    public bool walkable;

    private bool wasSet = false;
    
    public void SetMaterial(Material newMaterial)
    {
        GetComponent<MeshRenderer>().material = newMaterial;

        if(walkable)
        {
            GetComponent<Animation>().Play();

            wasSet = true;
        }
    }
}
