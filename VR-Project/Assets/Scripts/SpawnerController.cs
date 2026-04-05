using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public GameObject carsContainer;
    List<GameObject> carsList = new List<GameObject>();
    float delay = 2;
    float interval = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform car in carsContainer.transform)
        {
            carsList.Add(car.gameObject);
        }
        interval = delay + UnityEngine.Random.Range(1.0f, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (carsList.Count > 0) 
        {
            interval -= Time.deltaTime;
            if (interval <= 0)
            {
                int index = UnityEngine.Random.Range(0, carsList.Count);
                carsList[index].SetActive(true);
                carsList.RemoveAt(index);
                interval = delay + UnityEngine.Random.Range(2.5f, 4.5f);
            }
        }
    }
}
