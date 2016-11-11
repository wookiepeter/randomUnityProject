using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    //Player kann nicht außerhalb des bounds bereich sehen
    // Use this for initialization
    public Transform player;
    public Vector2 
        margin,
        smoothing;
    public BoxCollider2D BoxCollider2DBounds;

    Vector3
        _min,
        _max;

    public bool isFollowing { get; set; }


    public void Start()
    {
        _min = BoxCollider2DBounds.bounds.min;
        _max = BoxCollider2DBounds.bounds.max;
        isFollowing = true;
    }
    public void Update()
    {
        var x = transform.position.x;
        var y = transform.position.y;

        if (isFollowing)
        {
            if (Mathf.Abs(x - player.position.x) > margin.x)
                x = Mathf.Lerp(x, player.position.x, smoothing.x * Time.deltaTime);
            if (Mathf.Abs(y - player.position.y) > margin.y)
                y = Mathf.Lerp(y, player.position.y, smoothing.y * Time.deltaTime);
        }

        //Wenden die Formal an x = size * (w/n)
        var cameraHalfWidth = GetComponent<Camera>().orthographicSize * ((float) Screen.width / Screen.height);

        x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
        y = Mathf.Clamp(y, _min.y + GetComponent<Camera>().orthographicSize, _max.y - GetComponent<Camera>().orthographicSize);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
