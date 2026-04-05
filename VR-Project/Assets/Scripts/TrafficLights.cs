using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    public Transform t1;
    public Transform t2;
    public Transform t3;

    public GameObject t1green;
    public GameObject t1red;
    public GameObject t2green;
    public GameObject t2red;
    public GameObject t3green;
    public GameObject t3red;

    private GameObject trafficLight1Collider;
    private GameObject trafficLight2Collider;
    private GameObject trafficLight3Collider;

    public float stateTimer; //to keep track of elapsed time / used ti stop the time elapsed in each state
    public int state; //to remember which light setting is currently being displayed (setting can be either 1 or 2)

    // Start is called before the first frame update
    void Start()
    {
        t1 = transform.Find("TL1");
        t2 = transform.Find("TL2");
        t3 = transform.Find("TL3");

        t1green = t1.Find("Green light").gameObject;
        t1red = t1.Find("Red light").gameObject;
        t2green = t2.Find("Green light").gameObject;
        t2red = t2.Find("Red light").gameObject;
        t3green = t3.Find("Green light").gameObject;
        t3red = t3.Find("Red light").gameObject;

        trafficLight1Collider = t1.Find("invisibleWall").gameObject;
        trafficLight2Collider = t2.Find("invisibleWall").gameObject;
        trafficLight3Collider = t3.Find("invisibleWall").gameObject;

        trafficLight2Collider.transform.position += new Vector3(0,10,0);
        trafficLight3Collider.transform.position += new Vector3(0,10,0);

        stateTimer = 10.0f; //initialising the traffic lights
        SetState(1);
    }

    // Update is called once per frame
    void Update()
    {
        if ((stateTimer -= Time.deltaTime) < 0)
        {
            stateTimer = 22.0f;
            if (state == 1) { SetState(2); }
            else { SetState(1); }
        }
        else { stateTimer -= Time.deltaTime; }
    }

    void SetState(int c)
    {
        state = c;
        if (c == 1)
        {
            t1green.active = true;
            t1red.active = false;
            t2green.active = false;
            t2red.active = true;
            t3green.active = false;
            t3red.active = true;
            trafficLight1Collider.transform.position += new Vector3(0, +10, 0);
            trafficLight2Collider.transform.position += new Vector3(0, -10, 0);
            trafficLight3Collider.transform.position += new Vector3(0, -10, 0);
        }
        else
        {
            t1green.active = false;
            t1red.active = true;
            t2green.active = true;
            t2red.active = false;
            t3green.active = true;
            t3red.active = false;
            trafficLight1Collider.transform.position += new Vector3(0, -10, 0);
            trafficLight2Collider.transform.position += new Vector3(0, +10, 0);
            trafficLight3Collider.transform.position += new Vector3(0, +10, 0);
        }
    }
}
