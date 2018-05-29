using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseAniHelper : MonoBehaviour
{
    public LineRenderer[] Lines;

    // Use this for initialization
    void Start()
    {

        //SingleXPosLine(Lines[0]);
        //SingleYPosLine(Lines[1]);
        

        //var a = GetTValue(0.9f, 0.1f, 0);
        //a.ForEach(aa => print(aa));

    }

    void SingleXPosLine(LineRenderer Line)
    {
        var xBer = transform.gameObject.AddComponent<BJTweenAction>();
        Line.positionCount = 0;
        xBer.Duration = 2f;
        xBer.AniAction += p =>
        {
            var pos = GetBezierTPoint(new Vector3[] { new Vector3(0, 0), new Vector3(.98f, .14f), new Vector3(0, .9f), new Vector3(1, 1) }, p);
            pos = new Vector3(p, 0, pos.x) * 20;
            Line.positionCount++;
            Line.SetPosition(Line.positionCount - 1, pos);
        };
        xBer.Play();
    }

    void SingleYPosLine(LineRenderer Line)
    {
        var xBer = transform.gameObject.AddComponent<BJTweenAction>();
        Line.positionCount = 0;
        xBer.Duration = 2f;
        xBer.AniAction += p =>
        {
            var pos = GetBezierTPoint(new Vector3[] { new Vector3(0, 0), new Vector3(.98f, .14f), new Vector3(0, .9f), new Vector3(1, 1) }, p);
            pos = new Vector3(p, 0, pos.y) * 20;
            Line.positionCount++;
            Line.SetPosition(Line.positionCount - 1, pos);
        };
        xBer.Play();
    }

    Vector3 GetBezierTPoint(Vector3[] points, float t)
    {
        if (points.Length == 1)
        {
            return points[0];
        }

        Vector3[] pointsNew = new Vector3[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
        {
            pointsNew[i] = (1 - t) * points[i] + t * points[i + 1];
        }

        return GetBezierTPoint(pointsNew, t);
    }

    // Update is called once per frame
    void Update()
    {
     
    }


}
