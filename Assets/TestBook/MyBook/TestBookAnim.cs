using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBookAnim : MonoBehaviour
{
    public Vector3[] Points;
    public MyBook Book;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Book.Cur_Style = MyBook.TurningStyle.BottomRight;

            BJTweenPlugins.BJTweenAction be = this.gameObject.AddComponent<BJTweenPlugins.BJTweenAction>();
            be.AniAction = progress =>
            {
                var bpPos = BJTweenPlugins.BJBezier.GetBezierTPoint(Points, progress);
                Book.FollowPoint(bpPos);
            };
            be.Duration = 3f;
            be.Play();
        }
    }

}
