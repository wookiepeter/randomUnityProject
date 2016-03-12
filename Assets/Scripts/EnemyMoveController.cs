﻿using UnityEngine;
using System.Collections;

public class EnemyMoveController : MonoBehaviour {

    Rigidbody2D rb2d;

    public float speed;
    Vector2 playerPosition;
    Vector2 moveVector;
    Vector2 vectorTowardsPlayer;

    EnemyViewController enemyViewController;
    bool playerInViewRadius;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        enemyViewController = GetComponent<EnemyViewController>();
	}
	
	// Update is called once per frame
	void Update () {
       

        playerInViewRadius = enemyViewController.getPlayerInViewRadius();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        vectorTowardsPlayer = playerPosition - new Vector2(transform.position.x, transform.position.y);


    }

    void FixedUpdate()
    {
        moveVector = Vector2.zero;

        if (playerInViewRadius)
        {
            moveVector = vectorTowardsPlayer;
        }
        else
        {
            Vector2 rndPosition;
            rndPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) + new Vector2(Random.Range(-1f, 1f), 0);
            moveVector = new Vector2(transform.position.x, transform.position.y) - rndPosition;
        }

        rb2d.velocity = moveVector.normalized * speed;
    }
}
