using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyBook : MonoBehaviour
{
    /// <summary>
    /// 子孙根
    /// </summary>
    public RectTransform BookPanel;
    /// <summary>
    /// 翻页右面
    /// </summary>
    public Image TurnRight;
    /// <summary>
    /// 翻页左面
    /// </summary>
    public Image TurnLeft;
    /// <summary>
    /// 翻页裁剪布局
    /// </summary>
    public Image TurnClip;
    /// <summary>
    /// 新页裁剪布局
    /// </summary>
    public Image NextClip;
    /// <summary>
    /// 左页面
    /// </summary>
    //public UITexture Left;
    /// <summary>
    /// 右页面
    /// </summary>
    public Image Right;
    /// <summary>
    /// 左翻右的阴影
    /// </summary>
    public Image ShadowLtR;
    /// <summary>
    /// 右翻左的阴影
    /// </summary>
    public Image ShadowRtoL;

    /// <summary>
    /// 右下角位置
    /// </summary>
    Vector3 brpos;
    /// <summary>
    /// 左下角位置
    /// </summary>
    Vector3 blpos;
    /// <summary>
    /// 左上角位置
    /// </summary>
    Vector3 tlpos;
    /// <summary>
    /// 右上角位置
    /// </summary>
    Vector3 trpos;

    public BookSource Source;

    public TurningStyle Cur_Style
    {
        get { return cur_style; }
        set
        {
            cur_style = value;
            InitStyle(cur_style);
        }
    }
    TurningStyle cur_style = TurningStyle.BottomLeft;

    int CenterPicLayer = 2;
    //单页面宽度
    float Width;
    //单页面高度
    float Height;


    // Use this for initialization
    void Start()
    {


        Width = Right.rectTransform.sizeDelta.x;
        Height = Right.rectTransform.sizeDelta.y;

        brpos = new Vector3(-BookPanel.sizeDelta.x / 2 + Width, -Height / 2);
        blpos = new Vector3(-BookPanel.sizeDelta.x / 2 + 0, -Height / 2);
        trpos = new Vector3(-BookPanel.sizeDelta.x / 2 + Width, Height / 2);
        tlpos = new Vector3(-BookPanel.sizeDelta.x / 2 + 0, Height / 2);

        InitStyle(cur_style);
    }

    // Update is called once per frame
    void Update()
    {
        var pos = BookPanel.transform.InverseTransformPoint(Input.mousePosition);
        print(pos);
        FollowPoint(pos);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if ((int)Cur_Style + 1 <= 3) Cur_Style++;
            else Cur_Style = 0;
        }
    }

    void InitStyle(TurningStyle style)
    {
        switch (style)
        {
            case TurningStyle.BottomRight:
                {
                    //清除翻页左右面的旋转角度
                    InitCenterPicEular();
                    //设置翻页右面的对齐方式
                    ImagePivotChanged(TurnRight, TurningStyle.BottomLeft);
                    //设置翻页左面的对齐方式
                    TurnLeft.rectTransform.pivot = Right.rectTransform.pivot;
                    //设置翻页左面的位置
                    TurnLeft.transform.position = Right.transform.position;
                    //设置翻页裁剪容器的锚点
                    ClipPivotChange(Cur_Style);
                    //设置阴影的对齐方式
                    ImagePivotChanged(ShadowRtoL, style);

                    ShadowRtoL.gameObject.SetActive(true);
                    ShadowLtR.gameObject.SetActive(false);

                    SetImageOrder(TurnLeft, TurnRight, Right);

                }
                break;
            case TurningStyle.BottomLeft:
                {
                    InitCenterPicEular();
                    ImagePivotChanged(TurnLeft, TurningStyle.BottomRight);

                    TurnRight.rectTransform.pivot = Right.rectTransform.pivot;
                    TurnRight.transform.position = Right.transform.position;

                    ClipPivotChange(Cur_Style);

                    ImagePivotChanged(ShadowLtR, style);

                    ShadowRtoL.gameObject.SetActive(false);
                    ShadowLtR.gameObject.SetActive(true);

                    SetImageOrder(TurnRight, TurnLeft, Right);

                }
                break;
            case TurningStyle.TopRight:
                {
                    InitCenterPicEular();
                    ImagePivotChanged(TurnRight, TurningStyle.TopLeft);

                    TurnLeft.rectTransform.pivot = Right.rectTransform.pivot;
                    TurnLeft.transform.position = Right.transform.position;

                    ClipPivotChange(TurningStyle.TopRight);
                    ImagePivotChanged(ShadowRtoL, style);

                    ShadowRtoL.gameObject.SetActive(true);
                    ShadowLtR.gameObject.SetActive(false);
                    SetImageOrder(TurnLeft, TurnRight, Right);

                }
                break;
            case TurningStyle.TopLeft:
                {
                    InitCenterPicEular();

                    ImagePivotChanged(TurnLeft, TurningStyle.TopRight);

                    TurnRight.rectTransform.pivot = Right.rectTransform.pivot;
                    TurnRight.transform.position = Right.transform.position;
                    ClipPivotChange(Cur_Style);

                    ImagePivotChanged(ShadowLtR, style);

                    ShadowRtoL.gameObject.SetActive(false);
                    ShadowLtR.gameObject.SetActive(true);
                    SetImageOrder(TurnRight, TurnLeft, Right);

                }
                break;
        }

    }

    void SetImageOrder(Image thisP, Image centerP, Image nextP)
    {
        thisP.sprite = Source.ThisPage;
        centerP.sprite = Source.CenterPage;
        nextP.sprite = Source.NextPage;
    }

    public void FollowPoint(Vector3 bpPos)
    {
        switch (Cur_Style)
        {
            case TurningStyle.BottomRight:
            case TurningStyle.TopRight:
                {
                    if(bpPos.x <= brpos.x)
                    //开始翻页
                        FollowHand(bpPos);
                }
                break;
            case TurningStyle.BottomLeft:
            case TurningStyle.TopLeft:
                {
                    if(bpPos.x >= blpos.x)
                        FollowHand(bpPos, false);
                }
                break;
        }
    }

    void InitCenterPicEular()
    {
        TurnLeft.transform.eulerAngles = Vector3.zero;
        TurnRight.transform.eulerAngles = Vector3.zero;
        TurnLeft.transform.SetParent(BookPanel, true);
        TurnRight.transform.SetParent(BookPanel, true);
        Right.transform.SetParent(BookPanel, true);
    }

    /// <summary>
    /// 计算点和角度
    /// </summary>
    /// <param name="c">翻页的页脚点</param>
    /// <param name="t1">折痕点</param>
    /// <param name="clipAngle">翻页裁剪容器需要翻转的角度</param>
    /// <param name="clipPicAngle">翻页图片需要翻转的角度</param>
    void GetT1AndAngle(Vector3 c, out Vector3 t1, out Vector3 clipAngle, out Vector3 clipPicAngle)
    {
        clipAngle = Vector4.zero;
        clipPicAngle = Vector4.zero;
        t1 = c;
        switch (Cur_Style)
        {
            case TurningStyle.BottomRight:
                {
                    float cbXdel = Mathf.Abs(c.x - brpos.x);
                    float cbYdel = Mathf.Abs(c.y - brpos.y);
                    float cbAngle = Mathf.Atan(cbYdel / cbXdel);

                    t1.y = brpos.y;

                    if (cbAngle == 0) t1.x = brpos.x - cbXdel / 2;
                    else t1.x = brpos.x - cbYdel / Mathf.Sin(2 * cbAngle);

                    t1 = transform.TransformPoint(t1);

                    clipAngle = new Vector3(0, 0, -cbAngle * Mathf.Rad2Deg);
                    clipPicAngle = new Vector3(0, 0, -2 * cbAngle * Mathf.Rad2Deg);
                    if (c.y < brpos.y)
                    {
                        clipAngle = new Vector3(0, 0, cbAngle * Mathf.Rad2Deg);
                        clipPicAngle = new Vector3(0, 0, 2 * cbAngle * Mathf.Rad2Deg);
                    }
                }
                break;
            case TurningStyle.BottomLeft:
                {
                    float cbXdel = Mathf.Abs(c.x - blpos.x);
                    float cbYdel = Mathf.Abs(c.y - blpos.y);
                    float cbAngle = Mathf.Atan(cbYdel / cbXdel);

                    t1.y = blpos.y;

                    if (cbAngle == 0) t1.x = blpos.x + cbXdel / 2;
                    else t1.x = blpos.x + cbYdel / Mathf.Sin(2 * cbAngle);

                    t1 = transform.TransformPoint(t1);

                    clipAngle = new Vector3(0, 0, cbAngle * Mathf.Rad2Deg);
                    clipPicAngle = new Vector3(0, 0, 2 * cbAngle * Mathf.Rad2Deg);
                    if (c.y < blpos.y)
                    {
                        clipAngle = new Vector3(0, 0, -cbAngle * Mathf.Rad2Deg);
                        clipPicAngle = new Vector3(0, 0, -2 * cbAngle * Mathf.Rad2Deg);
                    }
                }
                break;
            case TurningStyle.TopRight:
                {
                    float cbXdel = Mathf.Abs(c.x - trpos.x);
                    float cbYdel = Mathf.Abs(c.y - trpos.y);
                    float cbAngle = Mathf.Atan(cbYdel / cbXdel);

                    t1.y = trpos.y;
                    if (cbAngle == 0) t1.x = trpos.x - cbXdel / 2;
                    else t1.x = trpos.x - cbYdel / Mathf.Sin(2 * cbAngle);

                    t1 = transform.TransformPoint(t1);

                    clipAngle = new Vector3(0, 0, cbAngle * Mathf.Rad2Deg);
                    clipPicAngle = new Vector3(0, 0, 2 * cbAngle * Mathf.Rad2Deg);
                    if (c.y > trpos.y)
                    {
                        clipAngle = new Vector3(0, 0, -cbAngle * Mathf.Rad2Deg);
                        clipPicAngle = new Vector3(0, 0, -2 * cbAngle * Mathf.Rad2Deg);
                    }
                }
                break;
            case TurningStyle.TopLeft:
                {
                    float cbXdel = Mathf.Abs(c.x - tlpos.x);
                    float cbYdel = Mathf.Abs(c.y - tlpos.y);
                    float cbAngle = Mathf.Atan(cbYdel / cbXdel);

                    t1.y = tlpos.y;
                    if (cbAngle == 0) t1.x = tlpos.x + cbXdel / 2;
                    else t1.x = tlpos.x + cbYdel / Mathf.Sin(2 * cbAngle);

                    t1 = transform.TransformPoint(t1);

                    clipAngle = new Vector3(0, 0, -cbAngle * Mathf.Rad2Deg);
                    clipPicAngle = new Vector3(0, 0, -2 * cbAngle * Mathf.Rad2Deg);
                    if (c.y > tlpos.y)
                    {
                        clipAngle = new Vector3(0, 0, cbAngle * Mathf.Rad2Deg);
                        clipPicAngle = new Vector3(0, 0, 2 * cbAngle * Mathf.Rad2Deg);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 右下角翻页
    /// </summary>
    /// <param name="c"></param>
    void FollowHand(Vector3 c, bool rToL = true)
    {
        Image shadow = rToL ? ShadowRtoL : ShadowLtR;
        Image cipPic = rToL ? TurnRight : TurnLeft;

        TurnLeft.transform.SetParent(BookPanel.transform, true);
        TurnRight.transform.SetParent(BookPanel.transform, true);
        Right.transform.SetParent(BookPanel.transform, true);
        shadow.transform.SetParent(BookPanel.transform, true);

        Vector3 clipAngle;
        Vector3 t1;
        Vector3 clipPicAngle;

        GetT1AndAngle(c, out t1, out clipAngle, out clipPicAngle);

        TurnClip.transform.localEulerAngles = clipAngle;
        TurnClip.transform.position = t1;

        shadow.transform.SetParent(TurnClip.transform);
        shadow.transform.localPosition = Vector3.zero;
        shadow.transform.localEulerAngles = Vector3.zero;

        NextClip.transform.eulerAngles = clipAngle;
        NextClip.transform.position = t1;

        cipPic.transform.eulerAngles = clipPicAngle;
        cipPic.transform.position = transform.TransformPoint(c);

        TurnLeft.transform.SetParent(TurnClip.transform, true);
        TurnRight.transform.SetParent(TurnClip.transform, true);
        Right.transform.SetParent(NextClip.transform, true);
        shadow.transform.SetParent(cipPic.transform, true);
        cipPic.transform.SetAsLastSibling();
    }

    void ClipPivotChange(TurningStyle style)
    {
        switch (style)
        {
            case TurningStyle.BottomLeft:
                {
                    SetPanelOffset(NextClip, new Vector2(1, 0.33f));
                    SetPanelOffset(TurnClip, new Vector2(0, 0.33f));
                }
                break;
            case TurningStyle.BottomRight:
                {
                    SetPanelOffset(TurnClip, new Vector2(1, 0.33f));
                    SetPanelOffset(NextClip, new Vector2(0, 0.33f));
                }
                break;
            case TurningStyle.TopLeft:
                {
                    SetPanelOffset(NextClip, new Vector2(1, 0.66f));
                    SetPanelOffset(TurnClip, new Vector2(0, 0.66f));
                }
                break;
            case TurningStyle.TopRight:
                {
                    SetPanelOffset(TurnClip, new Vector2(1, 0.66f));
                    SetPanelOffset(NextClip, new Vector2(0, 0.66f));
                }
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="pivot">左下角为0，0</param>
    void SetPanelOffset(Image panel, Vector2 pivot)
    {
        if (panel == null) return;
        panel.rectTransform.pivot = pivot;
    }

    void ImagePivotChanged(Image left, TurningStyle pivot)
    {
        switch (pivot)
        {
            case TurningStyle.TopLeft:
                {
                    left.rectTransform.pivot = new Vector2(0, 1);
                }
                break;
            case TurningStyle.TopRight:
                {
                    left.rectTransform.pivot = new Vector2(1, 1);
                }
                break;
            case TurningStyle.BottomLeft:
                {
                    left.rectTransform.pivot = new Vector2(0, 0);
                }
                break;
            case TurningStyle.BottomRight:
                {
                    left.rectTransform.pivot = new Vector2(1, 0);
                }
                break;
        }
    }

    public enum TurningStyle
    {
        BottomLeft,
        BottomRight,
        TopLeft,
        TopRight,
    }
}

[System.Serializable]
public class BookSource
{
    public Sprite NextPage;
    public Sprite ThisPage;
    public Sprite CenterPage;
}
