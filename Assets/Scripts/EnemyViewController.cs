using UnityEngine;
using System.Collections;

public class EnemyViewController : MonoBehaviour {

    bool playerInViewRadius;

    // Use this for initialization
    void Start()
    {

        playerInViewRadius = false;
    }

    public bool getPlayerInViewRadius()
    {
        return playerInViewRadius;
    }

    void FixedUpdate()
    {
        playerInViewRadius = false;
    }

    

    void OnTriggerStay2D(Collider2D other)
    {
        
        if(other.CompareTag("Player"))
        {
            Debug.Log(" Reichweite TRUE");
            playerInViewRadius = true;
        }
        /*
        else
        {
            playerInViewRadius = false;
            Debug.Log(" NICHT Reichweite false");
        }
        */
        

    }

}
