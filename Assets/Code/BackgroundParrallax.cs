using UnityEngine;
using System.Collections;

public class BackgroundParrallax : MonoBehaviour {

    //Script für die Kamera 

    public Transform[] backgrounds;
    public float parrallaxScale;
    public float parallaxReductionFactor;
    public float smoothing;

    Vector3 _lastPosition; //Last Position der Camera
    public void Start()
    {
        _lastPosition = transform.position;
    }

    public void Update()
    {
        var parralax = (_lastPosition.x - transform.position.x) * parrallaxScale;

        for(var i = 0; i < backgrounds.Length; i++)
        {
            var backgroundTargetPosition = backgrounds[i].position.x + parralax * (i * parallaxReductionFactor + 1);
            backgrounds[i].position = Vector3.Lerp(
                backgrounds[i].position,                                                                            //FROM
                new Vector3(backgroundTargetPosition, backgrounds[i].position.y, backgrounds[i].position.z),        //TO
                smoothing * Time.deltaTime);

        }
        _lastPosition = transform.position;
    }
}
