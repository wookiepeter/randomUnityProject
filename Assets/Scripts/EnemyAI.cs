using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour {

    //what to chase?
    public Transform target;

    //How many times each secon we will update our path
    public float updateRate = 2f;

    //caching
    private Seeker seeker;
    Rigidbody2D rb2d;

    //Calculated Path
    public Path path;

    //The AIs Speed per second
    public float speed;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    //The Max Distance from the AI to a Waypoint for it to contrinue to the next waypoint
    public float nextWayPointDistance = 3;

    //The Waypoint we r currently moving towards
    private int currentWayPoint = 0;

	void Start()
    {
        seeker = GetComponent<Seeker>();
        rb2d = GetComponent<Rigidbody2D>();

        if(target == null)
        {
            Debug.LogError("No Player found!");
            return;
        }
        //Start a new Path to the target position, return the result to the OnPathComplete Method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        if(target == null)
        {
            //TODO: insert a player search here
            yield return false;
        }
        seeker.StartPath(transform.position, target.position, OnPathComplete);
        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());

    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("We got a Path, did i have an error?" + p.error); //succes? or not
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }
    
    void FixedUpdate()
    {
        if(target == null)
        {
            return;
        }
        //TODO: Always look at player

        if(path == null)
        {
            return;
        }
        if(currentWayPoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
            {
                return;
            }
            Debug.Log("End of path reached.");
            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

        //Direction to the next waypoint
        Vector2 dir = (path.vectorPath[currentWayPoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        //Move the AI
        rb2d.AddForce(dir, fMode);
        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWayPoint]);

        if (dist < nextWayPointDistance)
        {
            currentWayPoint++;
            return;
        }
    }
}
