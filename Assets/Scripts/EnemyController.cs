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
            
            rndPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) - new Vector2(Random.Range(-1f, 1f), 0);
            rb2d.velocity = rndPosition.normalized * speed;
        }

	}

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log(" Reichweite TRUE");
            playerRange = true;
            rb2d.velocity = movementVector.normalized * speed;
        }
        else
        {
            playerRange = false;
            Debug.Log(" NICHT Reichweite false");
        }
           
       
    }
   
}
