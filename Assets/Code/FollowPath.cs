using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FollowPath : MonoBehaviour {

    public enum FollowType
    {
        MoveTowards, //Gegenüber/nach/in die Richtung
        Lerp
    }
    public FollowType Type = FollowType.MoveTowards;
    public PathDefinition path;
    public float speed = 1;
    public float MaxDistanceToGoal = .1f;

    private IEnumerator<Transform> _currentPoint;

    public void Start()
    {
        if(path == null)
        {
            Debug.LogError("Path cannot be null - need Pathcript", gameObject);
            return;
        }
        _currentPoint = path.GetPathEnumerator(); //gibt mir den transform zurück zu den ich aktuell muss
        _currentPoint.MoveNext();

        if (_currentPoint.Current == null)
            return;

        transform.position = _currentPoint.Current.position; //First point in the path
    }
    
    public void Update()
    {
        if(_currentPoint == null || _currentPoint.Current == null)
            return;

        if (Type == FollowType.MoveTowards)
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * speed); //Von, Nach, Tempo
        else if(Type == FollowType.Lerp)
            transform.position = Vector3.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);

        //close enough to target point
        var distanceQuared = (transform.position - _currentPoint.Current.position).sqrMagnitude;
        if (distanceQuared < MaxDistanceToGoal * MaxDistanceToGoal)
            _currentPoint.MoveNext();

    }


}
