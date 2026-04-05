using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class CarRoute : MonoBehaviour
{
    public List<Transform> wps;
    public List<Transform> route;
    public int routeNumber = 0;
    public int targetWP = 0;
    public float dist = 0;
    public Rigidbody rb;
    public bool go = false;
    public float initialDelay;
    public float currentSpeed = 0;
    private float maxSpeed = 7.0f;
    private int accelerationRate = 10;
    private int decelerationRate = 20;
    public int carRouteNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        wps = new List<Transform>();
        GameObject wp;
        wp = GameObject.Find("WPRoad1LeftToRight"); // index 0
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoad2LeftToRight"); // index 1
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoad1RightToLeft"); // index 2
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoad2RightToLeft"); // index 3
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoadRightToLeftLeftTurn"); // index 4
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoadRightToLeftLeftTurnEnd"); // index 5
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoadNewStartRightToLeftTurn"); // index 6
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoadNewRightToLeftTurn"); // index 7
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoadNewRightToRightTurn"); // index 8
        wps.Add(wp.transform);
        wp = GameObject.Find("WPRoad1LeftToRightRightTurn"); // index 9
        wps.Add(wp.transform);
        SetRoute();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 displacement = route[targetWP].position - transform.position;
        displacement.y = 0;
        dist = displacement.magnitude;

        if (dist < 0.1f)
        {
            /* This checks if the car is within 15cms of the way point
             * If it is, it sets the current waypoint indicator to the next waypoint by incrementing targetWP
             * If targetWP is now greater than the number of waypoints in the route, then the route is completed, and the code section calls SetRoute() to start a new route, and then exists the FixedUpdate() function
             */
            targetWP++;
            if (targetWP >= route.Count)
            {
                SetRoute();
                return;
            }
        }
        // ~~~~~~~~~~~~~~~~~~~~ TO MOVE THE CAR ~~~~~~~~~~~~~~~~~~~~
        //calculate velocity for this frame
        Vector3 velocity = displacement;
        velocity.Normalize();
        //apply velocity

        
        if ((go == true) && (currentSpeed < maxSpeed))//(accelerate == true)
        {
            currentSpeed += accelerationRate*Time.deltaTime;
            if (currentSpeed > maxSpeed) { currentSpeed = maxSpeed; }
        }
        else if (go == false)//(decelerate == true) // car decelerates to stop
        {
            if (currentSpeed > decelerationRate * Time.deltaTime) { currentSpeed -= decelerationRate * Time.deltaTime; }
            else { currentSpeed = 0.0f; }
        }
        else { currentSpeed = maxSpeed; }
        velocity *= currentSpeed;

        Vector3 newPosition = transform.position;
        newPosition += velocity * Time.deltaTime;
        rb.MovePosition(newPosition);
        /* This code is sufficient to move the car along the route, between waypoints, and start new routes when complete
         * It sets the velocity to be in the direction of the displacement vector (i.e. towards the next waypoint), but with a speed of 2.5m/s
         * It then uses the velocity to change the object position by calculating the displacement moved in the last time step (velocity X time elapsed), adding it to the current position, 
         * and then using the Rigidbody MovePosition() function to update the car's position  */
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //align car to velocity
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity, 10.0f * Time.deltaTime, 0f); // 10.0f is the rotation speed
        Quaternion rotation = Quaternion.LookRotation(desiredForward);
        rb.MoveRotation(rotation);
    }

    void SetRoute()
    {
        if (!go)
        {
            go = true;
            SetRoute();
        }
        //randomise the next route
        routeNumber = UnityEngine.Random.Range(0, 6); // Randomly generates initial car route
        List<Transform> leftToRight = new List<Transform>() { wps[0], wps[1] };
        List<Transform> RightToleft = new List<Transform>() { wps[3], wps[2] };
        List<Transform> leftToRightTurn = new List<Transform>() { wps[0], wps[8], wps[9], wps[4], wps[5] };
        List<Transform> RightToleftTurn = new List<Transform>() { wps[3], wps[4], wps[5] };
        List<Transform> UpToLeftTurn = new List<Transform>() { wps[6], wps[7], wps[2] };
        List<Transform> UpToRightTurn = new List<Transform>() { wps[6], wps[7], wps[8], wps[9], wps[1] };
        List<List<Transform>> listOfRoutes = new List<List<Transform>>() { leftToRight, RightToleft, leftToRightTurn, RightToleftTurn, UpToLeftTurn, UpToRightTurn};
        route = listOfRoutes[routeNumber];

        //initialise position and waypoint counter
        if (gameObject.tag == "Car") { transform.position = new Vector3(route[0].position.x, 0.5f, route[0].position.z); }
        else if (gameObject.tag == "Sports Car") { transform.position = new Vector3(route[0].position.x, 0f, route[0].position.z); }
        targetWP = 1;
    }

    void OnTriggerEnter(Collider collision) // e.g. if a pedestrian walks in the way of the car
    {
        if ((transform.position.z < 59) && (transform.position.z > -59))
        {
            if ((collision.gameObject.tag == "Pedestrian") || (collision.gameObject.tag == "TrafficLight") || (collision.gameObject.tag == "Car") || (collision.gameObject.tag == "Sports Car")) { go = false; } 
        }
    }

    void OnTriggerExit(Collider collision) // e.g. if the pedestrian moves out of the cube/out of the way of the car
    {
        if ((collision.gameObject.tag == "Pedestrian") || (collision.gameObject.tag == "TrafficLight") || (collision.gameObject.tag == "Car") || (collision.gameObject.tag == "Sports Car")) 
        {
            StartCoroutine(WaitBeforeMoving()); // after stopping, the car takes 1.5 seconds to resume accelerating and moving again
            go = true; 
        }
        //go = true;
    }

    IEnumerator WaitBeforeMoving()
    {
        yield return new WaitForSeconds(3.0f);
    }
}