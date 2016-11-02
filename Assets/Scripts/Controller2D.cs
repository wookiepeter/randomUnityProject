using UnityEngine;
using System.Collections;


[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    //Variabeln

    public LayerMask collisionMask;
    const float skinWidth = .015f;              //Gibt an, ab wann der raycast (IM OBJEKT) erstellt werden soll

    //Anzahl der Raycast die gezeichnet werden sollen
    public int horizonzalRayCount = 4;
    public int verticalRayCount = 4;

    //Abstand von den Raycasts
    float horizontalRaySpacing;
    float verticalRaySpacing;

    //Climbing
    float maxClimbAngle = 80;               //Welchen winkel kann mein Player laufen(maximal)

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

    public CollisionInfo collisionInfo;

	void Start () {
       collider = GetComponent<BoxCollider2D>();
       CalculateRaySpacing();                                                       //Nur im Start, nachdem wir den Collider bekommen haben

    }


    public void Move(Vector3 velocity)
    {
        UpdateRayastOrigins();
        collisionInfo.Reset();

        if(velocity.x != 0)
            HorizonalCollision(ref velocity);

        if (velocity.y != 0)
            VerticalCollision(ref velocity);

        transform.Translate(velocity);                                              //Setzt das Ziel in bewegung
    }
    void HorizonalCollision(ref Vector3 veloctiy)                                   //Immer wenn die Variabeln irgendwo geändert wird, wird sie auch hier verändert!
    {
        float directionX = Mathf.Sign(veloctiy.x);                                  //Return value is 1 when f is positive or zero, -1 when f is negative.
        float rayLength = Mathf.Abs(veloctiy.x) + skinWidth;
        Vector2 forward = Vector2.right * directionX * rayLength;

        for (int i = 0; i < horizonzalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, forward, Color.red);

            if (hit)
            {
                //Climbing - Berechnen von der Position des Spielers den Winkel des Objects(Wall)
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle) 
                {
                    float distanceToSlopeStart = 0;
                    if(slopeAngle != collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        veloctiy.x -= distanceToSlopeStart * directionX;
                    }
                    //print(slopeAngle);
                    ClimbSlope(ref veloctiy, slopeAngle);
                    veloctiy.x += distanceToSlopeStart * directionX;
                    
                }
                if(!collisionInfo.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    veloctiy.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    //Setzt CollisionInfo box auf -1 oder 1 (Kommt auf die Richtung an)
                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                }

            }
        }
    }
    void VerticalCollision(ref Vector3 veloctiy)                                    //Immer wenn die Variabeln irgendwo geändert wird, wird sie auch hier verändert!
    {
        float directionY = Mathf.Sign(veloctiy.y);                                  //Setzt directionY auf 1 oder -1
        float rayLength = Mathf.Abs(veloctiy.y)+skinWidth;                          //Länge des Rays

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;    //Down oder Top? Gibt an von wo gestartet werden soll!

            rayOrigin += Vector2.right * (verticalRaySpacing * i + veloctiy.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask); //
            // StartPunkt, Vector2.up gibt y achse an * -1 oder +1, Länge, Mask(Wall)
            //public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity); 
            //https://docs.unity3d.com/ScriptReference/Physics2D.Raycast.html

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                veloctiy.y = (hit.distance-skinWidth) * directionY;
                rayLength = hit.distance;

                //Setzt CollisionInfo box auf -1 oder 1 (Kommt auf die Richtung an)
                collisionInfo.below = directionY == -1;
                collisionInfo.above = directionY == 1;
            }
        }
    }
    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {//Ziel: wenn wir climben wollen wir die velocity richtig einstellen
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y < climbVelocityY)
        {//Jumping on Slope =Steigung
            //print("Jumping on Slope");

            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisionInfo.below = true;                                                                             //Wenn wir auf der Schräge sind soll below true sein damit wir springen können.
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
        }

    }

    void UpdateRayastOrigins()
    {
        Bounds bounds = collider.bounds;     //The world space bounding area of the collider.
        bounds.Expand(skinWidth * -2);      //Expand the bounds by increasing its size by amount along each side. Vergrößer/verkleiner die Grenzen


        //Definiert die Punkte vom Collider
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        //Sicherstellen das mehr als 2 Raycast (Oben Unten zumBeispiel) gefeuert werden
        horizonzalRayCount = Mathf.Clamp(horizonzalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizonzalRayCount - 1); //Der Abstand = GesamteHorizontalSeite / Anzahl der gefeuerten Linien
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins                           //Speicher die Position
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    //Gibt uns INformation über unseren Collision
    public struct CollisionInfo
    {
        public bool above, below; //Über,Unten
        public bool left, right;
        public bool climbingSlope;
        public float slopeAngle, slopeAngleOld;

        public void Reset()
        {//Setzt alles auf False

            above = below = false;
            left = right = false;
            climbingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
	
}
