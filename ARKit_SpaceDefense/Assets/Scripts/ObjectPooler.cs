using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    private GameObject pooledObject;
    private int pooledAmount;
    private bool isGrowing;
    
    private List<GameObject> pooledObjects;

    public void Initialize(GameObject newPooledObject, int pooledNum = 20, bool allowGrow = true)
    {
        pooledObject = newPooledObject;
        pooledAmount = pooledNum;
        isGrowing = allowGrow;

        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
    
    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if(isGrowing)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);

            return obj;
        }

        return null;
    }
}
