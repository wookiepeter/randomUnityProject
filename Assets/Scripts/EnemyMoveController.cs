using UnityEngine;
using System.Collections;

public class EnemyMoveController : MonoBehaviour {

    Rigidbody2D rb2d;


    public float        speed;
    Vector2             playerPosition;
    Vector2             movementVector;
    EnemyViewController enemyViewController;
    bool                playerInViewRadius;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        enemyViewController = GetComponent<EnemyViewController>();
	}
	
	// Update is called once per frame
	void Update () {

        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        movementVector = playerPosition - new Vector2(transform.position.x, transform.position.y);



	}

    void FixedUpdate()
    {
        if (enemyViewController.getPlayerInViewRadius())
        {
            rb2d.velocity = movementVector.normalized * speed;
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }
    }
}
