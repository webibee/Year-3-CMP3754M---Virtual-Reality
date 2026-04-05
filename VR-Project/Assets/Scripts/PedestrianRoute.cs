using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class PedestrianRoute : MonoBehaviour
{
    public List<Transform> wps;
    public List<Transform> route;
    public int routeNumber = 0;
    public int targetWP = 0;
    public float dist = 0;
    public Rigidbody rb;
    public bool go = false;
    public float initialDelay;

    // Start is called before the first frame update
    void Start()
    {
        wps = new List<Transform>();
        GameObject wp;

        wp = GameObject.Find("WP1");
        wps.Add(wp.transform);
        wp = GameObject.Find("WP2");
        wps.Add(wp.transform);
        wp = GameObject.Find("WP3");
        wps.Add(wp.transform);
        wp = GameObject.Find("WP4");
        wps.Add(wp.transform);
        wp = GameObject.Find("WP5");
        wps.Add(wp.transform);
        wp = GameObject.Find("WP6");
        wps.Add(wp.transform);
        wp = GameObject.Find("WP7");
        wps.Add(wp.transform);
        wp = GameObject.Find("WP8");
        wps.Add(wp.transform);
        SetRoute();
        rb = GetComponent<Rigidbody>();
        initialDelay = UnityEngine.Random.Range(2.0f, 12.0f);
        transform.position = new Vector3(0.0f, -5.0f, 0.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!go)
        {
            initialDelay -= Time.deltaTime;
            if (initialDelay <= 0.0f)
            {
                go = true;
                SetRoute();
            }
            else return;
        }
        Vector3 displacement = route[targetWP].position - transform.position;
        displacement.y = 0;
        dist = displacement.magnitude;

        if (dist < 0.1f) 
        {
            /* This checks if the pedestrian is within 15cms of the way point
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
        // ~~~~~~~~~~~~~~~~~~~~ TO MOVE THE PEDESTRIAN ~~~~~~~~~~~~~~~~~~~~
        //calculate velocity for this frame
        Vector3 velocity = displacement;
        velocity.Normalize();
        velocity *= 2.5f;
        //apply velocity
        Vector3 newPosition = transform.position;
        newPosition += velocity * Time.deltaTime;
        rb.MovePosition(newPosition);
        /* This code is sufficient to move the pedestrian along the route, between waypoints, and start new routes when complete
         * It sets the velocity to be in the direction of the displacement vector (i.e. towards the next waypoint), but with a speed of 2.5m/s
         * It then uses the velocity to change the object position by calculating the displacement moved in the last time step (velocity X time elapsed), adding it to the current position, 
         * and then using the Rigidbody MovePosition() function to update the pedestrian position  */
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //align pedestrian to velocity
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity, 10.0f * Time.deltaTime, 0f); // 10.0f is the rotation speed
        Quaternion rotation = Quaternion.LookRotation(desiredForward);
        rb.MoveRotation(rotation);
    }

    void SetRoute()
    {
        //randomise the next route
        routeNumber = UnityEngine.Random.Range(0, 12);
        //set the route waypoints
        if (routeNumber == 0) route = new List<Transform> { wps[0], wps[4], wps[5], wps[6] };
        else if (routeNumber == 1) route = new List<Transform> { wps[0], wps[4], wps[5], wps[7] };
        else if (routeNumber == 2) route = new List<Transform> { wps[2], wps[1], wps[4], wps[5], wps[6] };
        else if (routeNumber == 3) route = new List<Transform> { wps[2], wps[1], wps[4], wps[5], wps[7] };
        else if (routeNumber == 4) route = new List<Transform> { wps[3], wps[4], wps[5], wps[6] };
        else if (routeNumber == 5) route = new List<Transform> { wps[3], wps[4], wps[5], wps[7] };
        else if (routeNumber == 6) route = new List<Transform> { wps[6], wps[5], wps[4], wps[0] };
        else if (routeNumber == 7) route = new List<Transform> { wps[6], wps[5], wps[4], wps[3] };
        else if (routeNumber == 8) route = new List<Transform> { wps[6], wps[5], wps[4], wps[1], wps[2] };
        else if (routeNumber == 9) route = new List<Transform> { wps[7], wps[5], wps[4], wps[0] };
        else if (routeNumber == 10) route = new List<Transform> { wps[7], wps[5], wps[4], wps[3] };
        else if (routeNumber == 11) route = new List<Transform> { wps[7], wps[5], wps[4], wps[1], wps[2] };

        //initialise position and waypoint counter
        transform.position = new Vector3(route[0].position.x, 0.0f, route[0].position.z);
        targetWP = 1;
    }
}
