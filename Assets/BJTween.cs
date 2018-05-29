using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class BJTween : MonoBehaviour
{

    public float Speed;
    float AniProgress = 0;
    public event EventHandler AniEnded;

    public virtual void AnimationAction(float progress)
    {

    }

    bool _bStart;
    bool bStart
    {
        set
        {
            if (Speed == 0)
            {
                _bStart = false;
                return;
            }
            _bStart = value;
            AniProgress = 0;
        }
        get
        {
            return _bStart;
        }
    }

    // Use this for initialization
    void Start()
    {
        //bStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_bStart)
        {
            if (AniProgress + Speed > 1)
            {
                AniProgress = 1;
                _bStart = false;
            }
            else
            {
                AniProgress += Speed;
            }

            AnimationAction(AniProgress);

            if (AniProgress == 1)
            {
                if (AniEnded != null)
                {
                    AniEnded(transform, EventArgs.Empty);
                }
                AniProgress = 0;
                AniEnded = null;
                Destroy(this);
            }
        }
    }

    public void Stop()
    {
        bStart = false;
        AniProgress = 0;
    }

    public void Play()
    {
        bStart = true;
    }

}

public abstract class BJTweenBaseOnSecond : MonoBehaviour
{
    public bool AutoPlay = false;
    public event EventHandler AniEnded;
    public float Duration = 0;
    public bool Loop = false;
    public float LoopTime = 0;
    int frameCount = 0;
    float timeCount = 0;
    public bool isDebug = false;

    private void Awake()
    {
        if (AutoPlay) Play();
    }

    public virtual void AnimationAction(float progress)
    {

    }

    public virtual void IniAni()
    {

    }

    public virtual void EndAction()
    {

    }

    public virtual void AfterUpdate()
    {

    }

    public virtual void ClearSameComponent() { }

    bool _bStart;
    bool bStart
    {
        set
        {
            _bStart = value;
        }
        get
        {
            return _bStart;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bStart)
        {
            if (frameCount == 0)
            {
                IniAni();
            }

            float deltime = Time.deltaTime;
            timeCount += deltime;
            frameCount++;

            float progress = 1;
            if (Duration > 0)
            {
                progress = timeCount / Duration;
            }

            if (progress >= 1)
            {
                progress = 1;
                ResetCounter();
            }

            AnimationAction(progress);

            if (progress == 1)
            {
                if (Loop)
                {
                    Invoke("LoopDelay", LoopTime);
                }
                else
                {
                    if (AniEnded != null)
                    {
                        AniEnded(transform, EventArgs.Empty);
                        AniEnded = null;
                    }
                    Destroy(this);
                }
                EndAction();
            }

            AfterUpdate();
        }
    }

    public void Stop()
    {
        ResetCounter();
        Destroy(this);
    }

    public void Play()
    {
        bStart = true;
        ClearSameComponent();
    }

    void ResetCounter()
    {
        bStart = false;
        timeCount = 0;
        frameCount = 0;
    }

    void LoopDelay()
    {
        bStart = true;
    }

}

public class BJTweenPos : BJTweenBaseOnSecond
{
    public Vector3 From;
    public Vector3 To;

    public EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;

    public override void ClearSameComponent()
    {
        var coms = new List<BJTweenPos>(this.transform.GetComponents<BJTweenPos>());
        coms.ForEach(com => { if (com != this) Destroy(com); });
    }
    public override void IniAni()
    {
        if(From == Vector3.zero)
        {
            From = transform.localPosition;
        }

    }

    public override void AnimationAction(float progress)
    {
        transform.localPosition = EaseActionHelper.Instance.VectorEaseAction(Anitype ,From, To, progress);
    }

    public void Play(Vector3 from, Vector3 to, float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        this.From = from;
        this.To = to;
        this.Duration = duration;
        this.Anitype = anitype;
        base.Play();
    } 
}

public class BJTweenRot : BJTweenBaseOnSecond
{
    public Vector3 From;
    public Vector3 To;
    public EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;

    public override void ClearSameComponent()
    {
        var coms = new List<BJTweenRot>(this.transform.GetComponents<BJTweenRot>());
        coms.ForEach(com => { if (com != this) Destroy(com); });
    }

    public override void IniAni()
    {
        if (From == Vector3.zero) From = this.transform.localRotation.eulerAngles;

        if (From.x > 180) From -= new Vector3(360, 0, 0);
        else if (From.x < -180) From += new Vector3(360, 0, 0);
        if (From.y > 180) From -= new Vector3(0, 360, 0);
        else if (From.y < -180) From += new Vector3(0, 360, 0);
        if (From.z > 180) From -= new Vector3(0, 0, 360);
        else if (From.z < -180) From += new Vector3(0, 0, 360);

        if (Mathf.Abs(To.x - From.x) > 180)
        {
            if (To.x > 0) To -= new Vector3(360, 0, 0);
            else To += new Vector3(360, 0, 0);
        }
        if (Mathf.Abs(To.y - From.y) > 180)
        {
            if (To.y > 0) To -= new Vector3(0, 360, 0);
            else To += new Vector3(0, 360, 0);
        }
        if (Mathf.Abs(To.z - From.z) > 180)
        {
            if (To.z > 0) To -= new Vector3(0, 0, 360);
            else To += new Vector3(0, 0, 360);
        }
    }

    public override void AnimationAction(float progress)
    {
        transform.localRotation = Quaternion.Euler(EaseActionHelper.Instance.VectorEaseAction(Anitype, From, To, progress));
        //print(transform.localRotation.eulerAngles);
    }

    public void Play(Vector3 from, Vector3 to, float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        this.From = from;
        this.To = to;
        this.Duration = duration;
        this.Anitype = anitype;
        base.Play();
    }
}

#if NGUI
public class BJTweenNUIAlpha: BJTweenBaseOnSecond
{
    public float From;
    public float To;
    public EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;


    public override void AnimationAction(float progress)
    {
        float alpha = EaseActionHelper.Instance.EaseCalculate(Anitype, From, To, progress);
        Color color = transform.GetComponent<UIWidget>().color;
        transform.GetComponent<UIWidget>().color = new Color(color.r, color.g, color.b, alpha);
    }
}
#endif


public class BJTweenAlpha : BJTweenBaseOnSecond
{
    public float From = 0;
    public float To = 1;
    public EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;

    public override void ClearSameComponent()
    {
        var coms = new List<BJTweenAlpha>(this.transform.GetComponents<BJTweenAlpha>());
        coms.ForEach(com => { if (com != this) Destroy(com); });
    }

    public override void IniAni()
    {
        if (transform.GetComponent<Renderer>() == null)
        {
            Stop();
        }
    }

    public override void AnimationAction(float progress)
    {
        float alpha = EaseActionHelper.Instance.EaseCalculate(Anitype, From, To, progress);

        if (transform.GetComponent<Renderer>() != null)
        {
            foreach (var m in transform.GetComponent<Renderer>().materials)
            {
                Color color = m.color;
                m.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
    }

    public void Play(float from, float to, float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        this.From = from;
        this.To = to;
        this.Duration = duration;
        this.Anitype = anitype;
        base.Play();
    }

}

public class BJCameraFar : BJTweenBaseOnSecond
{
    public float From;
    public float To;
    public EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;

    public override void AnimationAction(float progress)
    {
        transform.GetComponent<Camera>().farClipPlane = EaseActionHelper.Instance.EaseCalculate(Anitype, From, To, progress);
    }

    public void Play(float from, float to, float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        this.From = from;
        this.To = to;
        this.Duration = duration;
        this.Anitype = anitype;
        base.Play();
    }
}

public class BJTweenScale : BJTweenBaseOnSecond
{
    public Vector3 From;
    public Vector3 To;
    public EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;

    public override void IniAni()
    {
        if(From == Vector3.zero) From = this.transform.localScale;
    }

    public override void AnimationAction(float progress)
    {
        transform.localScale = EaseActionHelper.Instance.VectorEaseAction(Anitype, From, To, progress);
    }


    public void Play(Vector3 from, Vector3 to, float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        this.From = from;
        this.To = to;
        this.Duration = duration;
        this.Anitype = anitype;
        base.Play();
    }
}

public class BJTweenCameraSize : BJTweenBaseOnSecond
{
    public float From;
    public float To;
    public EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;

    public override void IniAni()
    {
        From = transform.GetComponent<Camera>().orthographicSize;
    }

    public override void AnimationAction(float progress)
    {
        transform.GetComponent<Camera>().orthographicSize = EaseActionHelper.Instance.EaseCalculate(Anitype, From, To, progress);
    }

    public void Play(float from, float to, float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        this.From = from;
        this.To = to;
        this.Duration = duration;
        this.Anitype = anitype;
        base.Play();
    }

}



public class BJTweenAction : BJTweenBaseOnSecond
{
    public Action<float> AniAction;

    public override void AnimationAction(float progress)
    {
        //print("progress: "  + progress);
        if (AniAction != null)
        {
            AniAction(progress);
        }
    }
}

public class BuildingFadeInAnimation : BJTweenBaseOnSecond
{
    public float From;
    public float To;
    public float Final;
    private EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;

    public override void AnimationAction(float progress)
    {
        float param = EaseActionHelper.Instance.EaseCalculate(Anitype, From, To, progress);
        transform.GetComponent<Camera>().nearClipPlane = param;
    }

    public override void EndAction()
    {
        if (Final != 0)
        {
            transform.GetComponent<Camera>().nearClipPlane = Final;
        }
    }

    public void Play(float from, float to, float final,float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        this.From = from;
        this.To = to;
        this.Duration = duration;
        this.Anitype = anitype;
        base.Play();
    }
}

public class BJTweenQuaternion : BJTweenBaseOnSecond
{
    public Quaternion From;
    public Quaternion To;

    public override void IniAni()
    {
        From = transform.localRotation;
    }

    public override void AnimationAction(float progress)
    {
        transform.rotation = Quaternion.Lerp(From, To, progress);
    }

    public void Play(Quaternion from, Quaternion to, float duration)
    {
        this.From = from;
        this.To = to;
        this.Duration = duration;
        base.Play();
    }
}

public class BJTweenRotateAround : BJTweenBaseOnSecond
{
    float TargetAngle;
    float currAngle = 0;
    private EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;
    Vector3 Center;
    Vector3 Axis;
    public override void AnimationAction(float progress)
    {
        var nextAngle = EaseActionHelper.Instance.EaseCalculate(Anitype, 0, TargetAngle, progress);
        var delta = nextAngle - currAngle;
        transform.RotateAround(Center, Axis, delta);
        currAngle = nextAngle;
    }

    public void StartAnimation(Vector3 center, Vector3 axis, float tarangle, float duration)
    {
        Center = center;
        Axis = axis;
        TargetAngle = tarangle;
        Duration = duration;
        base.Play();
    }

}

public class BJTweenRotate : BJTweenBaseOnSecond
{
    float Angle;
    float currAngle = 0;
    private EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;
    Vector3 Axis;

    public override void AnimationAction(float progress)
    {
        var nextAngle = EaseActionHelper.Instance.EaseCalculate(Anitype, 0, Angle, progress);
        var delta = nextAngle - currAngle;
        transform.Rotate(Axis, delta);
        currAngle = nextAngle;
    }

    public void StartAnimation(Vector3 axis, float angle, float duration)
    {
        Axis = axis;
        Angle = angle;
        Duration = duration;
        base.Play();
    }

}

public class BJTweenText : BJTweenBaseOnSecond
{
    public string Content;
    TextMesh TextMesh;
    int _length;


    public override void IniAni()
    {
        TextMesh = transform.GetComponent<TextMesh>();
        if (TextMesh == null)
        {
            Destroy(this);
            return;
        }

        BJTweenText[] texts = transform.GetComponents<BJTweenText>();
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != this)
            {
                Destroy(texts[i]);
            }
        }

        _length = Content.Length;
        //print("====> " + _length);
    }

    public override void AnimationAction(float progress)
    {
        float single = 1.0f / (_length + 1);
        int index = 0;
        while (true)
        {
            if (index * single <= progress && progress < (index + 1) * single)
            {
                if (index > _length) index = _length;
                TextMesh.text = Content.Substring(0, index);
                break;
            }
            index++;
        }


    }


    public void Play(float duration)
    {
        this.Duration = duration;
        base.Play();
    }

}


//public class BJTweenText : BJTweenBaseOnSecond
//{
//    public string Content;
//    TextMesh TextMesh;
//    int _length;
//    int _proindex = 0;

//    public override void IniAni()
//    {
//        TextMesh = transform.GetComponent<TextMesh>();
//        if (TextMesh == null)
//        {
//            Destroy(this);
//            return;
//        }

//        BJTweenText[] texts = transform.GetComponents<BJTweenText>();
//        for (int i = 0; i < texts.Length; i++)
//        {
//            if (texts[i] != this)
//            {
//                Destroy(texts[i]);
//            }
//        }

//        _length = Content.Length;
//        //print("====> " + _length);
//    }

//    public override void AnimationAction(float progress)
//    {
//        float single = 1.0f / (_length + 1);
//        //int index = 0;
//        //while (true)
//        //{
//        //    if (index * single <= progress && progress < (index + 1) * single)
//        //    {
//        //        TextMesh.text = Content.Substring(0, index);
//        //        break;
//        //    }
//        //    index++;
//        //}

//        if (progress > single * _proindex)
//        {
//            if (_proindzex <= Content.Length)
//            {
//                TextMesh.text = Content.Substring(0, _proindex);
//            }
//            _proindex++;

//            print("text index: " + _proindex + ", progress : " + progress + ", length: " + _length);

//        }


//    }

//}

public class BJTweenColor : BJTweenBaseOnSecond
{
    public List<Color> To;
    List<Color> From;

    public override void IniAni()
    {
        if (transform.GetComponent<Renderer>() != null)
        {
            From = new List<Color>();
            Material[] ms = transform.GetComponent<Renderer>().materials;
            for (int i = 0; i < ms.Length; i++)
            {
                Material m = ms[i];
                From.Add(m.color);
            }
        }
    }

    public override void AnimationAction(float progress)
    {
        if (To != null && From != null)
        {
            if (To.Count == 1)
            {
                Material[] ms = transform.GetComponent<Renderer>().materials;
                for (int i = 0; i < ms.Length; i++)
                {
                    Material m = ms[i];
                    m.color = Color.Lerp(From[i], To[0], progress);
                }
            }
            else if (To.Count > 1)
            {
                Material[] ms = transform.GetComponent<Renderer>().materials;
                for (int i = 0; i < ms.Length; i++)
                {
                    if (i >= To.Count) break;
                    Material m = ms[i];
                    m.color = Color.Lerp(From[i], To[i], progress);
                }
            }
        }
    }

    public void Play(List<Color> to, float duration)
    {
        this.To = to;
        this.Duration = duration;
        base.Play();
    }
}

public static class TweenExpand
{
    public static void FadeOut(this Transform trans, float duration)
    {
        var alpha = trans.gameObject.AddComponent<BJTweenAlpha>();
        alpha.Play(1, 0, duration);
    }

    public static void MoveBy(this Transform trans, Vector3 del, float duration, EventHandler handler = null, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        Vector3 to = trans.localPosition + del;
        MoveTo(trans, to, duration, handler, anitype);
    }

    public static void MoveTo(this Transform trans, Vector3 to, float duration, EventHandler handler = null,EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        var pos = trans.gameObject.AddComponent<BJTweenPos>();
        if(handler != null) pos.AniEnded += handler;
        pos.Play(trans.localPosition, to, duration, anitype);
    }

    public static void Move(this Transform trans, Vector3 from, Vector3 to, float duration, EventHandler handler = null, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        var pos = trans.gameObject.AddComponent<BJTweenPos>();
        if (handler != null) pos.AniEnded += handler;
        pos.Play(from, to, duration, anitype);
    }

    public static void MoveSingleTo(this Transform trans, PosKind kind, float value, float duration, EventHandler handler = null, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        var pos = trans.gameObject.AddComponent<BJTweenPos>();
        if (handler != null) pos.AniEnded += handler;
        var from = trans.localPosition;
        var to = from;
        switch (kind)
        {
            case PosKind.X: to.x = value; break;
            case PosKind.Y: to.y = value; break;
            case PosKind.Z: to.z = value; break;
        }
        pos.Play(from, to, duration, anitype);
    }

    public static void MoveSingle(this Transform trans, PosKind kind, float fromValue, float toValue, float duration, EventHandler handler = null, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
    {
        if (kind == PosKind.A) return;
        var pos = trans.gameObject.AddComponent<BJTweenPos>();
        if (handler != null) pos.AniEnded += handler;
        var from = trans.localPosition;
        var to = from;
        switch (kind)
        {
            case PosKind.X: from.x = fromValue; to.x = toValue; break;
            case PosKind.Y: from.y = fromValue; to.y = toValue; break;
            case PosKind.Z: from.z = fromValue; to.z = toValue; break;
        }
        pos.Play(from, to, duration, anitype);
    }

}

public class BJBezier : BJTweenBaseOnSecond
{
    public Vector3[] Points;
    public LineRenderer Line;

    public override void IniAni()
    {
        if (Points == null || Points.Length <= 0)
        {
            Stop();
            return;
        }
        if (isDebug && Line != null)
        {
            Line.positionCount = 1;
            Line.SetPosition(Line.positionCount - 1, Points[0]);
        }
    }

    public override void AnimationAction(float progress)
    {
        var tPos = GetBezierTPoint(Points, progress);
        transform.localPosition = tPos;
        if (isDebug && Line != null)
        {
            Line.positionCount++;
            Line.SetPosition(Line.positionCount - 1, tPos);
        }

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
}


public enum PosKind
{
    X,
    Y,
    Z,
    A,
}


