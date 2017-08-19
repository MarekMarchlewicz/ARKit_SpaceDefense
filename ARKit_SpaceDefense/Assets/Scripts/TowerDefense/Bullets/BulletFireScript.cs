using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFireScript : MonoBehaviour
{
    public float fireTime = 0.05f;
    public GameObject bullet;

    public int pooledAmount = 20;

    List<GameObject> bullets;

    private void Start()
    {
        bullets = new List<GameObject>();

        for(int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(bullet);
            obj.SetActive(false);
            bullets.Add(obj);
        }

        InvokeRepeating("Fire", fireTime, fireTime);
    }

    private void Fire()
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (!bullets[i].activeInHierarchy)
            {
                bullets[i].transform.position = transform.position;
                bullets[i].transform.rotation = transform.rotation;
                bullets[i].SetActive(true);
                bullets[i].GetComponent<Rigidbody>().velocity = transform.forward * 50f;
                break;
            }
        }
    }
}
