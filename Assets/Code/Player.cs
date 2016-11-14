using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    //Ziel: Der Player soll als Input klassen dienen, die nur Inputs entgegene nimmt.
    bool _isFacingRight;
    CharacterController2D _controller;
    float _normalizedHorizontalSpeed; //Left or Right

    //Wie schnell der Speed des Spielers wechseln kann! Not Moving to moving
    public float maxSpeed = 8;
    public float speedAccelerationOnGround = 10f;
    public float speedAccelerationInAir = 5f;

    public bool isDead { get; private set; }

    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        _isFacingRight = transform.localScale.x > 0; //Schauen wir rechts?
    }
    public void Update()
    {
        if(!isDead)
            HandleInput();  //_normalizedHorizontalSpeed wird 1 -1 oder 0

        var movementFactor = _controller.state.isGrounded ? speedAccelerationOnGround : speedAccelerationInAir;
        _controller.SetHorizontalForce(Mathf.Lerp(_controller.velocity.x, _normalizedHorizontalSpeed * maxSpeed, Time.deltaTime * movementFactor));
    }

    public void Kill()
    {
        _controller.handleCollisions = false;
        GetComponent<Collider2D>().enabled = false;
        isDead = true;
    }
    public void RespawnAt(Transform spawnpoint)
    {
        if (!_isFacingRight)
            Flip();
        isDead = false;
        GetComponent<Collider2D>().enabled = true;
        _controller.handleCollisions = true;

     
        transform.position = new Vector3(spawnpoint.position.x, spawnpoint.position.y, 0);

    }

    //Jumping
    void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _normalizedHorizontalSpeed = 1;
            if (!_isFacingRight)
                Flip();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _normalizedHorizontalSpeed = -1;
            if (_isFacingRight)
                Flip();
        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }
        if(_controller.canJump && Input.GetKey(KeyCode.Space))
        {
            _controller.Jump();
        }

    }

    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = transform.localScale.x > 0 ;
    }
}
