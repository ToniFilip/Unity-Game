using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawProjection : MonoBehaviour
{
    Weapon launchProjectile; 
    LineRenderer lineRenderer;

    //number of points the line will have
    public int numPoints = 50;
    //the distance between the points
    public float timeBetweenPoints = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        launchProjectile = GetComponent<Weapon>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = numPoints;
        List<Vector3> points = new List<Vector3>();
        
        //get position and velocity of the projectiles
        Vector3 startingPosition = launchProjectile.firePoint.position;
        //Vector3 startingVelocity = launchProjectile.firePoint.up * launchProjectile.launchVelocity * 0.02f;
        
        //create the line
        for (float t = 0; t < numPoints; t += timeBetweenPoints)
        {
            //Vector3 newPoint = startingPosition + t * startingVelocity;
            //newPoint.y = startingPosition.y + startingVelocity.y * t + (Physics.gravity.y/2f) * t * t;
            points.Add(newPoint);
        }
        lineRenderer.SetPositions(points.ToArray());
    }
    */
}