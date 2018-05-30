using UnityEngine;
using System;
using System.Collections.Generic;

namespace BJTweenPlugins
{
    public abstract class BJTween : MonoBehaviour
    {
        public bool Repeated = false;
        public float Speed;
        float AniProgress = 0;
        public event EventHandler AniEnded;
        bool _plus = false;

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
                if (_plus)
                {
                    if (AniProgress + Speed > 1)
                    {
                        AniProgress = 1;
                    }
                    else
                    {
                        AniProgress += Speed;
                    }
                }
                else
                {
                    if (AniProgress - Speed < 0)
                    {
                        AniProgress = 0;
                    }
                    else
                    {
                        AniProgress -= Speed;
                    }
                }

                AnimationAction(AniProgress);

                if (_plus)
                {

                    if (AniProgress == 1)
                    {
                        if (!Repeated)
                        {
                            Ended();
                        }
                        else
                        {
                            _plus = false;
                        }
                    }

                }
                else
                {
                    if (AniProgress == 0)
                    {
                        if (!Repeated)
                        {
                            Ended();
                        }
                        else
                        {
                            _plus = true;
                        }
                    }

                }
            }
        }

        void Ended()
        {
            if (AniEnded != null)
            {
                AniEnded(transform, EventArgs.Empty);
            }
            AniProgress = 0;
            AniEnded = null;
            Destroy(this);
            _bStart = false;
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
        public event EventHandler AniEnded;
        public float Duration = 0;
        public bool Loop = false;
        public float LoopTime = 0;
        int frameCount = 0;
        float timeCount = 0;

        public EaseActionHelper.MyMethod _anitype = EaseActionHelper.MyMethod.Linear;
        public EaseActionHelper.MyMethod Anitype
        {
            get { return _anitype; }
            set
            {
                _anitype = value;
                if(_anitype == EaseActionHelper.MyMethod.Custom) _mybezier = new UnitBezier();
            }
        }

        //ÇúÏßÖúÊÖ
        UnitBezier _mybezier;
        public void SetBezierParams(double x1, double y1, double x2, double y2)
        {
            if (_mybezier != null)
            {
                _mybezier.UpdateValue(x1, y1, x2, y2);
            }
        }

        protected float BezierSolve(float progress)
        {
            if (_mybezier != null)
            {
                return (float)_mybezier.solve(progress, 1e-6);
            }
            return 1;
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
        void Update()
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
        public PosKind kind = PosKind.A;
        public Vector3 From;
        public Vector3 To;

        public override void ClearSameComponent()
        {
            var coms = new List<BJTweenPos>(this.transform.GetComponents<BJTweenPos>());
            coms.ForEach(com => { if (com != this) Destroy(com); });
        }
        public override void IniAni()
        {
            if (From == Vector3.zero)
            {
                From = transform.localPosition;
            }

        }

        public override void AnimationAction(float progress)
        {
            transform.localPosition = EaseActionHelper.Instance.VectorEaseAction(Anitype, From, To, progress);
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


    public class BJTweenNUIAlpha : BJTweenBaseOnSecond
    {
        public float From;
        public float To;


        public override void AnimationAction(float progress)
        {
            float alpha = EaseActionHelper.Instance.EaseCalculate(Anitype, From, To, progress);
            Color color = transform.GetComponent<UIWidget>().color;
            transform.GetComponent<UIWidget>().color = new Color(color.r, color.g, color.b, alpha);
        }

        public void StartAnimation(float from, float to, float duration)
        {
            From = from;
            To = to;
            Duration = duration;
            Play();
        }
    }


    public class BJTweenAlpha : BJTweenBaseOnSecond
    {
        public float From = 0;
        public float To = 1;

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

        public void StartAnimation(float from, float to, float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
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

        public override void IniAni()
        {

        }

        public override void AnimationAction(float progress)
        {
            if (Anitype == EaseActionHelper.MyMethod.Custom)
            {
                float x = BezierSolve(progress);
                Vector3 newScale = From + (To - From) * x;
                transform.localScale = newScale;
                return;
            }
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

        public void Play(float from, float to, float final, float duration, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
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

    public class BJTweenMainTex : BJTweenBaseOnSecond
    {
        Vector2 From;
        Vector2 To;
        Material Mater;


        public override void AnimationAction(float progress)
        {
            var nextAngle = EaseActionHelper.Instance.VectorEaseAction(Anitype, From, To, progress);
            Mater.SetTextureOffset("_MainTex", nextAngle);
        }

        public void StartAnimation(Material material, Vector2 from, Vector2 to, float duration)
        {
            From = from;
            To = to;
            Mater = material;
            Duration = duration;
            base.Play();
        }
    }

    public class BJTweenAnim : BJTweenBaseOnSecond
    {
        float From;
        float To;
        Material Mater;

        public override void AnimationAction(float progress)
        {
            var nextAngle = EaseActionHelper.Instance.EaseCalculate(Anitype, From, To, progress);
            Mater.SetFloat("_anim", nextAngle);
            Mater.SetFloat("_xian", Mathf.Clamp01(nextAngle));
        }

        public void StartAnimation(Material material, float from, float to, float duration)
        {
            From = from;
            To = to;
            Mater = material;
            Duration = duration;
            base.Play();
        }
    }

    public class BJTweenEmissionColor : BJTweenBaseOnSecond
    {
        Color From;
        Color To;
        Material Mater;

        public override void IniAni()
        {
            From = Mater.GetColor("_EmissionColor");
        }

        public override void AnimationAction(float progress)
        {
            var lerpColor = Color.Lerp(From, To, progress);
            Mater.SetColor("_EmissionColor", lerpColor);
        }

        public void StartAnimation(Material material, Color to, float duration)
        {
            To = to;
            Mater = material;
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

    public class BJTweenTime : BJTweenBaseOnSecond
    {

    }

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

        public void StartAnimation(List<Color> to, float duration)
        {
            this.To = to;
            this.Duration = duration;
            base.Play();
        }
    }

    public static class TweenExpand
    {
        public static void Rot(this Transform trans, Vector3 from, Vector3 to, float duration, EventHandler handler = null, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
        {
            var pos = trans.gameObject.AddComponent<BJTweenRot>();
            if (handler != null) pos.AniEnded += handler;
            pos.Play(from, to, duration, anitype);
        }


        public static void FadeOut(this Transform trans, float duration)
        {
            var alpha = trans.gameObject.AddComponent<BJTweenAlpha>();
            alpha.StartAnimation(1, 0, duration);
        }

        public static void FadeIn(this Transform trans, float duration)
        {
            var alpha = trans.gameObject.AddComponent<BJTweenAlpha>();
            alpha.StartAnimation(0, 1, duration);
        }

        public static void NGUIFadeIn(this Transform trans, float duration, float from = 0, float to = 1, EventHandler handler = null, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
        {
            var alpha = trans.gameObject.AddComponent<BJTweenNUIAlpha>();
            if(handler != null) alpha.AniEnded += handler;
            alpha.StartAnimation(from, to, duration);
        }

        public static void MoveBy(this Transform trans, Vector3 del, float duration, EventHandler handler = null, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
        {
            Vector3 to = trans.localPosition + del;
            MoveTo(trans, to, duration, handler, anitype);
        }

        public static void MoveTo(this Transform trans, Vector3 to, float duration, EventHandler handler = null, EaseActionHelper.MyMethod anitype = EaseActionHelper.MyMethod.Linear)
        {
            var pos = trans.gameObject.AddComponent<BJTweenPos>();
            if (handler != null) pos.AniEnded += handler;
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
            pos.kind = kind;
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
            pos.kind = kind;
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

        public static void ScaleTo(this GameObject obj, Vector3 to, float duration, EventHandler handle)
        {
            var scale = obj.AddComponent<BJTweenScale>();
            scale.To = to;
            scale.Duration = duration;
            scale.AniEnded += handle;
            scale.Play();
        }

        public static void Scale(this GameObject obj, Vector3 from, Vector3 to, float duration, EventHandler handle = null, EaseActionHelper.MyMethod type = EaseActionHelper.MyMethod.Linear)
        {
            var scale = obj.AddComponent<BJTweenScale>();
            scale.From = from;
            scale.To = to;
            scale.Anitype = type;
            scale.Duration = duration;
            scale.AniEnded += handle;
            scale.Play();
        }

        public static void StopBJAnimation(this GameObject obj)
        {
            var scale = obj.GetComponent<BJTweenScale>();
            if (scale)
            {
                MonoBehaviour.Destroy(scale);
            }
            var fill = obj.GetComponent<BJTweenAction>();
            if (fill)
            {
                MonoBehaviour.Destroy(fill);
            }
        }

        public static void FillAmountTo(this GameObject trans, float from, float to, float duration, EventHandler handle)
        {
            UITexture texture = trans.GetComponent<UITexture>();
            if (texture == null) return;
            EaseActionHelper.MyMethod Anitype = EaseActionHelper.MyMethod.Linear;
            BJTweenAction baseAni = texture.gameObject.AddComponent<BJTweenAction>();
            baseAni.Duration = duration;
            baseAni.AniAction = prog =>
            {
                texture.fillAmount = EaseActionHelper.Instance.EaseCalculate(Anitype, from, to, prog);
            };
            baseAni.AniEnded += handle;
            baseAni.Play();
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
            if (Line != null)
            {
                Line.positionCount = 1;
                Line.SetPosition(Line.positionCount - 1, Points[0]);
            }
        }

        public override void AnimationAction(float progress)
        {
            var tPos = GetBezierTPoint(Points, progress);
            transform.localPosition = tPos;
            if (Line != null)
            {
                Line.positionCount++;
                Line.SetPosition(Line.positionCount - 1, tPos);
            }

        }

        public static Vector3 GetBezierTPoint(Vector3[] points, float t)
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

    public class UnitBezier
    {
        public void UpdateValue(double p1x, double p1y, double p2x, double p2y)
        {
            // Calculate the polynomial coefficients, implicit first and last control points are (0,0) and (1,1).
            cx = 3.0 * p1x;
            bx = 3.0 * (p2x - p1x) - cx;
            ax = 1.0 - cx - bx;

            cy = 3.0 * p1y;
            by = 3.0 * (p2y - p1y) - cy;
            ay = 1.0 - cy - by;
        }

        double sampleCurveX(double t)
        {
            // `ax t^3 + bx t^2 + cx t' expanded using Horner's rule.
            return ((ax * t + bx) * t + cx) * t;
        }

        double sampleCurveY(double t)
        {
            return ((ay * t + by) * t + cy) * t;
        }

        double sampleCurveDerivativeX(double t)
        {
            return (3.0 * ax * t + 2.0 * bx) * t + cx;
        }

        // Given an x value, find a parametric value it came from.
        double solveCurveX(double x, double epsilon)
        {
            double t0;
            double t1;
            double t2;
            double x2;
            double d2;
            int i;

            // First try a few iterations of Newton's method -- normally very fast.
            for (t2 = x, i = 0; i < 8; i++)
            {
                x2 = sampleCurveX(t2) - x;
                if (Math.Abs(x2) < epsilon)
                    return t2;
                d2 = sampleCurveDerivativeX(t2);
                if (Math.Abs(d2) < 1e-6)
                    break;
                t2 = t2 - x2 / d2;
            }

            // Fall back to the bisection method for reliability.
            t0 = 0.0;
            t1 = 1.0;
            t2 = x;

            if (t2 < t0)
                return t0;
            if (t2 > t1)
                return t1;

            while (t0 < t1)
            {
                x2 = sampleCurveX(t2);
                if (Math.Abs(x2 - x) < epsilon)
                    return t2;
                if (x > x2)
                    t0 = t2;
                else
                    t1 = t2;
                t2 = (t1 - t0) * .5 + t0;
            }

            // Failure.
            return t2;
        }

        public double solve(double x, double epsilon)
        {
            return sampleCurveY(solveCurveX(x, epsilon));
        }

        double ax;
        double bx;
        double cx;

        double ay;
        double by;
        double cy;
    }


}

