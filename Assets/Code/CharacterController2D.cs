using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {

    //Ziel: Soll sich um die Velocity, Gravity und Collision kümmern

    private const float skinWidth = .02f;
    private const int totalHorizontalRays = 8;
    private const int totalVerticalRays = 4;

    private static readonly float slopeLimitTangant = Mathf.Tan(75 * Mathf.Deg2Rad);

    public LayerMask PlatformMask;
    public ControllerParameters2D defaultParameters; //Allow us to use this parameters
    public ControllerState2D state { get; private set; }

    public Vector2 velocity { get {  return _velocity; } }

    public bool handleCollisions { get; set; }
    public ControllerParameters2D parameters { get { return _overrideParameters ?? defaultParameters; } }
    //Wenn es null ist defaultParameters -> _overrideParameters ?_overrideParameters:  defaultParameters;
    public GameObject standingOn { get; private set; }
    public bool canJump
    {
        get
        {
            if (parameters.jumpRestriction == ControllerParameters2D.jumpBehavior.canJumpAnywhere)
                return _jumpIn <= 0;
            if (parameters.jumpRestriction == ControllerParameters2D.jumpBehavior.canJumpOnGround)
                return state.isGrounded;
            return false;
        }
    }
    public Vector3 platformVelocity { get; private set; }

    Vector2 _velocity;
    Transform _transform;
    Vector3 _localScale;
    BoxCollider2D _boxCollider;
    ControllerParameters2D _overrideParameters;
    float _jumpIn;
    private Vector3
         _activeLocalPlatformPoint,
         _activeGlobalPlatformPoint;

    private Vector3
        _raycastTopLeft,
        _raycastBottomRight,
        _raycastBottomLeft;

    float _verticalDistanceBetweenRays;
    float _hoorizontalDistanceBetweenRays;
    GameObject _lastStandingOn;



    public void Awake()
    {
        handleCollisions = true;
        state = new ControllerState2D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        //Wir wollen den Abstand zwischen den verschiedenen RayCast ermitteln
        var colliderWidth = (_boxCollider.size.x * Mathf.Abs(transform.localScale.x) - 2 *skinWidth); //ABs weil beim Flippen der ScaleFactor negativ wird!
        _hoorizontalDistanceBetweenRays = colliderWidth / (totalVerticalRays - 1);

        var colliderHeight = (_boxCollider.size.y * Mathf.Abs(transform.localScale.y) - 2 * skinWidth);
        _verticalDistanceBetweenRays = colliderHeight / (totalHorizontalRays - 1);
    }

    //Füge der velocity die gegeben force bewegungs hinzu
    public void AddForce(Vector2 force)
    {
        _velocity = force;
    }
    public void SetForce(Vector2 force)
    {
        _velocity += force;
    }
    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }
    public void SetVericalForce(float y)
    {
        _velocity.y = y;
    }

    public void Jump()
    {
        //todo Moving platform support
        AddForce(new Vector2(0, parameters.jumpMagnitued)); //Gebe jumpMagnitued als kraft hinzu
        _jumpIn = parameters.jumpFrequency; 
    }
    public void LateUpdate()
    {
        _jumpIn -= Time.deltaTime;
        _velocity.y += parameters.gravity * Time.deltaTime;
        Move(velocity * Time.deltaTime);
    }
    private void Move(Vector2 deltaMovement)
    {
 
        var wasGrounded = state.isCollidingBelow;
        state.Reset();

        if (handleCollisions)
        {
            HandlePlatforms();
            CalculateRayOrigins();

            if (deltaMovement.y < 0 && wasGrounded) //Moving down?
                HandleVerticalSlope(ref deltaMovement);
            if (Mathf.Abs(deltaMovement.x) > 0.01f) //Rechts oder links?
                MoveHorizontally(ref deltaMovement);

            MoveVertically(ref deltaMovement);

            CorrectHorizontalPlacement(ref deltaMovement, true);
            CorrectHorizontalPlacement(ref deltaMovement, false);
        }
        _transform.Translate(deltaMovement, Space.World);

        if (Time.deltaTime > 0)
            _velocity = deltaMovement / Time.deltaTime;

        _velocity.x = Mathf.Min(_velocity.x, parameters.maxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, parameters.maxVelocity.y);

        if (state.isMovingUpSlope)
            _velocity.y = 0;

        if(standingOn != null)
        {
            _activeGlobalPlatformPoint = transform.position;
            _activeLocalPlatformPoint = standingOn.transform.InverseTransformPoint(transform.position);

            if (_lastStandingOn != standingOn)
            {
                if (_lastStandingOn != null)
                    _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);

                standingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
                _lastStandingOn = standingOn;
            }
            else if (standingOn != null)
                standingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);


        }

        else if(_lastStandingOn != null)
        {
            _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
            _lastStandingOn = null;
        }

    }
    private void HandlePlatforms()
    {

        if (standingOn != null) //Standin on Null
        {
            var newGlobalPlatformPoint = standingOn.transform.TransformPoint(_activeLocalPlatformPoint);
            var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

            if (moveDistance != Vector3.zero)
                transform.Translate(moveDistance, Space.World);

            platformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;


        }
        else
            platformVelocity = Vector3.zero;

        standingOn = null;
    }
    private void CalculateRayOrigins()
    {
        var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
        var center = new Vector2(_boxCollider.offset.x * Mathf.Abs(_localScale.x), _boxCollider.offset.y * Mathf.Abs(_localScale.y));

        _raycastTopLeft = _transform.position + new Vector3(center.x - size.x + skinWidth, center.y + size.y - skinWidth);
        _raycastBottomRight = _transform.position + new Vector3(center.x + size.x - skinWidth, center.y - size.y + skinWidth);
        _raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + skinWidth, center.y - size.y + skinWidth);
    }
    void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
    {//Diese Funktion soll verhindertn, dass man seitlich in platformen reinspringen kann.

        var halfWidth = (_boxCollider.size.x * _localScale.x) / 2f;
        var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

        /*
            Wir wollen den Origin mittig setzten, dafür müssen wir vom  _raycastBottomRight ausgehen.
            Wir müssen also die hälfte davon subrahieren um in die Mitte zu kommen. 
            Da wir genau bei der Mitte ankommen wollen müssen wir noch SkinWidth dazu addieren, da es bei _raycastBottomRight subtrahiert wurde.
            => wir setzten quasi den RayOrigin in der Mitte des Vierecks!
         */
        if (isRight)
            rayOrigin.x -= halfWidth - skinWidth;
        else
            rayOrigin.x += halfWidth - skinWidth;

        var rayDirection = isRight ? Vector2.right : -Vector2.right;
        var offset = 0f;

        for(var i= 1; i < totalHorizontalRays -1; i++)
        {//Zeichnet einzeln jeden Ray
            var rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
            //Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);
            if (!raycastHit)
                continue;

            offset = isRight ? (raycastHit.point.x - _transform.position.x) - halfWidth : (halfWidth - (transform.position.x - raycastHit.point.x));

        }
        deltaMovement.x += offset;

    }
    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
        var isGoingRight = deltaMovement.x > 0;                                                 //We r going Right!
        var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;                               //Distance des Rays! 
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;                       //Wohin schaut der Player?
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;                //Wohinn der Spieler schaut sende ich ein Raycast(Anfang)

        for(var i = 0; i <totalHorizontalRays; i++)
        {

            //ist ein Vector der x immer gleich ist und sich y pro durchlauf(i) um die distance(_verticalDistanceBetweenRays) verändert.
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);

            if (!rayCastHit)
                continue;

            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
                break;

            // rayCastHit ist das Objekt, welches getroffen wurde (Berechnet den Abstrand vom Origin zum Punkt anhand des rayVectors) Vektoren juhu ^^ (ABstand bis wir das Objekt treffen)
            deltaMovement.x = rayCastHit.point.x - rayVector.x; 
            rayDistance = Mathf.Abs(deltaMovement.x);

            if (isGoingRight)
            {
                deltaMovement.x -= skinWidth;
                state.isCollidingRight = true;

            }
            else
            {
                deltaMovement.x += skinWidth;
                state.isCollidingLeft = true;
            }
            if (rayDistance < skinWidth + 0.0001f)
                break;
        }
    }
    private void MoveVertically(ref Vector2 deltaMovement)
    {
        var isGoingUp = deltaMovement.y > 0;                                                    //We r going Right!
        var rayDistance = Mathf.Abs(deltaMovement.y) + skinWidth;                               //Distance des Rays! 
        var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;                       //Wohin schaut der Player?
        var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;                //Wohinn der Spieler schaut sende ich ein Raycast(Anfang)

        rayOrigin.x += deltaMovement.x; //POsition wo wir hingehen werden

        var standingOnDistance = float.MaxValue;

        for (var i = 0; i < totalVerticalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x + (i * _hoorizontalDistanceBetweenRays), rayOrigin.y); //Caste von der X-Achse nach oben/unten
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            if (!raycastHit)
                continue;

            if (!isGoingUp) //GoingDown dann 
            {
                var verticalDistanceToHit = _transform.position.y - raycastHit.point.y;  //DIstance vom Object und Player (y-Achse)
                //Überprüfen alle RayCasts -> welches trifft am frühsten ein Collider
                if(verticalDistanceToHit < standingOnDistance)
                {
                    standingOnDistance = verticalDistanceToHit; //Setzte die distance als neues erstes
                    standingOn = raycastHit.collider.gameObject; //Das gameobject das ich hitte

                }
            }
            deltaMovement.y = raycastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            if (isGoingUp)
            { //Wenn ich nach Oben mich bewege 
                deltaMovement.y -= skinWidth; //Bewege ich mich mit deltaMovement ohne skinwidth
                state.isCollidingAbove = true;
            }
            else
            { //Wenn ich nach unten mich bewege 
                deltaMovement.y += skinWidth;
                state.isCollidingBelow = true;

            }

            //Bewege ich mich nach oben und ich befinde mich quasi noch am Boden -> slope
            if (!isGoingUp && deltaMovement.y > .0001f)
                state.isMovingUpSlope = true;

            if (rayDistance < skinWidth + .0001f)
                break;
        }
    }
    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
        var center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2; //Bestimmte die Mitte von den beiden Punkten
        var direction = -Vector2.up;                                     //Nach unten

        var slopeDistance = slopeLimitTangant * (_raycastBottomRight.x - center);
        var slopeRayVector = new Vector2(center, _raycastBottomLeft.y); //Vecotr von wo der RayCast starten kann

        Debug.DrawRay(slopeRayVector, direction * slopeDistance, Color.yellow);
        var raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
        if (!raycastHit)
            return;

        var isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(deltaMovement.x); //Positive 1, neagtive 0, 0 0
        if (!isMovingDownSlope)
            return;

        var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
        if (Mathf.Abs(angle) < .0001f)
            return;

        //Bin auf einem slope
        state.isMovingDownSlope = true;
        state.slopeAngle = angle;
        deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
    }
    private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight)
    {
        if (Mathf.RoundToInt(angle) == 90) //Object ist 90 Grad -> nicht begehbar
            return false;

        if(angle > parameters.slopeLimit)
        {
            deltaMovement.x = 0;
            return true;
        }
        if (deltaMovement.y > .07f) //Wir sind in der Luft
            return true;

        deltaMovement.x += isGoingRight ? -skinWidth : skinWidth;
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
        state.isMovingUpSlope = true;
        state.isCollidingBelow = true;
        return true;

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhysics>();
        if (parameters == null)
            return;
        _overrideParameters = parameters.parameters;
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhysics>();
        if (parameters == null)
            return;
        _overrideParameters = null;
    }
}
