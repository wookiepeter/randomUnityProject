using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public CircleCollider2D cc2d;
    public float speed = 0.1f;
    Rigidbody2D rb2d;
    Vector2 playerposition;
    Vector2 movementVector;
    Vector2 rndPosition;
    bool playerRange = false;
	// Use this for initialization
	void Start () {
	
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

        playerposition = GameObject.FindGameObjectWithTag("Player").transform.position;
        movementVector = playerposition - new Vector2(transform.position.x, transform.position.y);
        if (!playerRange)
        {
            
        }

	}


}
