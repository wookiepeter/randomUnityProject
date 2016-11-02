using UnityEngine;
using System.Collections;


[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {

    float gravity;
    float moveSpeed = 6;

    float jumpVelocity;
    public float jumpHeight = 4;                    //Wie hoch soll der Character springen?
    public float timeToJumpApex = .4f;              //Wie schnell erreicht der Characer den höchsten Punkt?

    Vector3 velocity;                               //Geschwindigkeit
    float velocityXSmoothing;                       //Macht eine smootheBewegung
    float accelerationTimeAirborne = .2f;           // Smooth beim "jumpen"
    float accelerationTimeGrounded = .1f;           // Smooth beim laufen
    Controller2D controller;
    /*
        Berechnung von Gravity
        Known: JumpHeight, timeToJumpApex;
        Solve for: Gravity, jumpVelocity
        
          deltaMOvement = velocityInitial * time + (accerleration * time^2)/2 
        = jumptHeight = (gravity *timetoJumpApex^2) /2 ->Umformen
        -> gravity = 2*jumpHeigh/timetoJumpApex^2

          velocityFinal = velocityInitial + acceleration * time
        = jumpVelocity = gravity * timeToJumpApex;
    */
    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity =  -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);            //Siehe Formel (- weil es gravity ist :P)
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }
    void Update()
    {
        if(controller.collisionInfo.above || controller.collisionInfo.below)
        {//Befindet sich der Spieler auf den Boden ? Wenn ja velociy.y = 0, dh keine "Schwerkraft"
            velocity.y = 0;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetKeyDown(KeyCode.Space) && controller.collisionInfo.below)
        {//Wurde die Spacetaste gedrückt UND steht der Spieler auf einem Feld?
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisionInfo.below)?accelerationTimeGrounded:accelerationTimeAirborne);
        //SmoothDamp: (aktuelle Velocity, Ziel Velocity, X, aktuelle Velocity zu Ziel Velocity (ZEIT) -> Spieler auf dem Boden = Ground - sonst LuftZeit!)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
