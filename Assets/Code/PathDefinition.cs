using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class PathDefinition : MonoBehaviour {

    public Transform[] points;
    public IEnumerator<Transform> GetPathEnumerator()
    {
        if (points == null || points.Length < 1) // 1 weil 1 waypoint ist ein konstanter waypoint
            yield break;

        var direction = 1;
        var index = 0;
        while (true)
        {
            yield return points[index];

            if (points.Length == 1)
                continue;

            if (index <= 0)
                direction = 1;
            else if (index >= points.Length - 1)
                direction = -1;

            index = index + direction;
        }


    }

    //Ziel: Wir wollen ein Linie von P1 nach P2 zwei zeichnen
    public void OnDrawGizmos()
    {

        //Befinden sich überhaupt 2 Elemente in points?
        if (points == null || points.Length < 2)
            return;

        var helpPoint = points.Where(t => t != null).ToList();
        if (helpPoint.Count < 2)
            return;

        //Wir fangen bei Position 1 (nicht 0) an und zeichnen eine Linie von [i-1] zu position [i]
        for (var i = 1; i< helpPoint.Count; i++)
        {
            Gizmos.DrawLine(helpPoint[i - 1].position, helpPoint[i].position);
        }
    }
}
