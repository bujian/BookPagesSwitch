using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBezier : MonoBehaviour
{
    public Transform PointsTransGroup;
    LineRenderer Line;

    public LineRenderer TestLine;
    UnitBezier bezier = new UnitBezier();

    [Range(0,1)]
    public float Px1;
    [Range(0,1)]
    public float Py1;
    [Range(0,1)]
    public float Px2;
    [Range(0,1)]
    public float Py2;

    // Use this for initialization
    void Start()
    {
        Line = transform.GetComponent<LineRenderer>();

        EaseAnimation();
    }

    void EaseAnimation()
    {
        //for (float i = 0; i <= 1; i += 0.05f)
        //{
        //    var pos = GetBezierTPoint(new Vector3[] { new Vector3(0,0), new Vector3(.98f, .14f), new Vector3(0, .9f) , new Vector3(1,1)}, i);
        //    print("t : " + i + pos );
        //}
    }

    Vector3[] GetChildrenPoints(Transform trans)
    {
        Vector3[] points = new Vector3[PointsTransGroup.childCount];
        for (int i = 0; i < PointsTransGroup.childCount; i++)
        {
            points[i] = PointsTransGroup.GetChild(i).localPosition;
        }

        return points;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            var points = GetChildrenPoints(PointsTransGroup);

            var bezier = transform.gameObject.AddComponent<BJBezier>();
            bezier.isDebug = true;
            bezier.Duration = 5f;
            bezier.Points = points;
            bezier.Line = Line;
            bezier.Play();
        }

        DrawEaseLine(TestLine);
    }

    void DrawEaseLine(LineRenderer line)
    {
        bezier.UpdateValue(Px1, Py1, Px2, Py2);
        line.positionCount = 0;
        for (float x = 0; x <= 1; x += 0.01f)
        {
            float y = (float)bezier.solve(x, 1e-6);
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, new Vector3(x, y) * 20);
        }
    }
}
