using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBook : MonoBehaviour
{
    /// <summary>
    /// 子孙根
    /// </summary>
    public GameObject BookPanel;
    /// <summary>
    /// 翻页右面
    /// </summary>
    public UITexture CenterRight;
    /// <summary>
    /// 翻页左面
    /// </summary>
    public UITexture CenterLeft;
    /// <summary>
    /// 翻页裁剪布局
    /// </summary>
    public UIPanel TurnClip;
    /// <summary>
    /// 新页裁剪布局
    /// </summary>
    public UIPanel NextClip;
    /// <summary>
    /// 左页面
    /// </summary>
    public UITexture Left;
    /// <summary>
    /// 右页面
    /// </summary>
    public UITexture Right;
    /// <summary>
    /// 左翻右的阴影
    /// </summary>
    public UITexture ShadowLtR;
    /// <summary>
    /// 右翻左的阴影
    /// </summary>
    public UITexture ShadowRtoL;

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

    //单页面宽度
    float Width;
    //单页面高度
    float Height;

    public Camera uicam;

    public TurningStyle Cur_Style
    {
        get { return cur_style; }
        set
        {
            cur_style = value;
            InitStyle(cur_style);
        }
    }
    TurningStyle cur_style = TurningStyle.TopRight;

    int CenterPicLayer = 2;


    // Use this for initialization
    void Start()
    {
        Width = Right.width;
        Height = Right.height;

        brpos = new Vector3(Width, -Height / 2);
        blpos = new Vector3(0, -Height / 2);
        trpos = new Vector3(Width, Height / 2);
        tlpos = new Vector3(0, Height / 2);

        InitStyle(cur_style);
    }

    // Update is called once per frame
    void Update()
    {
        var pos = Input.mousePosition;
        var bpPos = transform.InverseTransformPoint( uicam.ScreenToWorldPoint(pos));
        bpPos.z = 0;
    }

    void InitStyle(TurningStyle style)
    {
        switch (Cur_Style)
        {
            case TurningStyle.BottomRight:
                {
                    //清除翻页左右面的旋转角度
                    InitCenterPicEular();
                    //设置翻页右面的对齐方式
                    CenterPicturePivotAdjust(CenterRight, UIWidget.Pivot.BottomLeft);
                    //设置翻页左面的对齐方式
                    CenterPicturePivotAdjust(CenterLeft, Right.pivot);
                    //设置翻页左面的位置
                    CenterLeft.transform.position = Right.transform.position;
                    //设置翻页裁剪容器的大小
                    ClipPivotChange(Cur_Style);
                    //设置阴影的对齐方式
                    ShadowRtoL.pivot = UIWidget.Pivot.BottomRight;
                    //设置层关系
                    CenterRight.depth = CenterPicLayer + 1;
                    CenterLeft.depth = CenterPicLayer;

                    ShadowRtoL.gameObject.SetActive(true);
                    ShadowLtR.gameObject.SetActive(false);

                }
                break;
            case TurningStyle.BottomLeft:
                {
                    InitCenterPicEular();
                    CenterPicturePivotAdjust(CenterLeft, UIWidget.Pivot.BottomRight);
                    CenterPicturePivotAdjust(CenterRight, Right.pivot);
                    CenterRight.transform.position = Right.transform.position;
                    ClipPivotChange(Cur_Style);
                    ShadowLtR.pivot = UIWidget.Pivot.BottomLeft;

                    CenterRight.depth = CenterPicLayer;
                    CenterLeft.depth = CenterPicLayer + 1;

                    ShadowRtoL.gameObject.SetActive(false);
                    ShadowLtR.gameObject.SetActive(true);

                }
                break;
            case TurningStyle.TopRight:
                {
                    InitCenterPicEular();
                    CenterPicturePivotAdjust(CenterRight, UIWidget.Pivot.TopLeft);
                    CenterPicturePivotAdjust(CenterLeft, Right.pivot);
                    CenterLeft.transform.position = Right.transform.position;
                    ClipPivotChange(TurningStyle.TopRight);
                    ShadowRtoL.pivot = UIWidget.Pivot.TopRight;

                    CenterRight.depth = CenterPicLayer + 1;
                    CenterLeft.depth = CenterPicLayer;

                    ShadowRtoL.gameObject.SetActive(true);
                    ShadowLtR.gameObject.SetActive(false);

                }
                break;
            case TurningStyle.TopLeft:
                {
                    InitCenterPicEular();
                    CenterPicturePivotAdjust(CenterLeft, UIWidget.Pivot.TopRight);
                    CenterPicturePivotAdjust(CenterRight, Right.pivot);
                    CenterRight.transform.position = Right.transform.position;
                    ClipPivotChange(Cur_Style);
                    ShadowLtR.pivot = UIWidget.Pivot.TopLeft;

                    CenterRight.depth = CenterPicLayer;
                    CenterLeft.depth = CenterPicLayer + 1;

                    ShadowRtoL.gameObject.SetActive(false);
                    ShadowLtR.gameObject.SetActive(true);

                }
                break;
        }
    }

    public void FollowPoint(Vector3 bpPos)
    {
        switch (Cur_Style)
        {
            case TurningStyle.BottomRight:
            case TurningStyle.TopRight:
                {
                    //开始翻页
                    FollowHand(bpPos);
                }
                break;
            case TurningStyle.BottomLeft:
            case TurningStyle.TopLeft:
                {
                    FollowHand(bpPos, false);
                }
                break;
        }
    }

    void InitCenterPicEular()
    {
        CenterLeft.transform.eulerAngles = Vector3.zero;
        CenterRight.transform.eulerAngles = Vector3.zero;
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
                    t1.x = brpos.x - cbYdel / Mathf.Sin(2 * cbAngle);

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
                    t1.x = blpos.x + cbYdel / Mathf.Sin(2 * cbAngle);

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
                    t1.x = trpos.x - cbYdel / Mathf.Sin(2 * cbAngle);

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
                    t1.x = tlpos.x + cbYdel / Mathf.Sin(2 * cbAngle);

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
        UITexture shadow = rToL ? ShadowRtoL : ShadowLtR;
        UITexture cipPic = rToL ? CenterRight : CenterLeft;

        CenterLeft.transform.SetParent(BookPanel.transform, true);
        CenterRight.transform.SetParent(BookPanel.transform, true);
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

        CenterLeft.transform.SetParent(TurnClip.transform, true);
        CenterRight.transform.SetParent(TurnClip.transform, true);
        Right.transform.SetParent(NextClip.transform, true);
        shadow.transform.SetParent(cipPic.GetComponentInChildren<UIPanel>().transform, true);

        //Fresh(CenterLeft.transform);
        //Fresh(CenterRight.transform);
        //Fresh(Right.transform);

        // print(Right.transform.position);
        PanelFresh();
    }

    void PanelFresh()
    {
        CenterLeft.transform.localScale = Vector3.one;
        CenterRight.transform.localScale = Vector3.one;
        Right.transform.localScale = Vector3.one;

        var ps = BookPanel.GetComponentsInChildren<UIPanel>();
        for (int i = 0; i < ps.Length; i++)
        {
            ps[i].Refresh();
        }
    }

    void Fresh(Transform trans)
    {
        trans.gameObject.SetActive(false);
        trans.localScale = Vector3.one;
        trans.gameObject.SetActive(true);
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
    void SetPanelOffset(UIPanel panel, Vector2 pivot)
    {
        if (panel == null) return;
        Vector2 vec = panel.GetViewSize();
        panel.clipOffset = new Vector2(vec.x / 2 - vec.x * pivot.x, vec.y / 2 - pivot.y * vec.y);
    }

    void CenterPicturePivotAdjust(UITexture left, UIWidget.Pivot pivot)
    {
        var panel = left.GetComponentInChildren<UIPanel>();
        left.pivot = pivot;
        Vector2 panelSize = panel.GetViewSize();
        float pHeight = panelSize.y;
        float pWidth = panelSize.x;
        switch (pivot)
        {
            case UIWidget.Pivot.TopLeft:
                {
                    panel.clipOffset = new Vector2(pWidth / 2, -pHeight / 2);
                }
                break;
            case UIWidget.Pivot.TopRight:
                {
                    panel.clipOffset = new Vector2(-pWidth / 2, -pHeight / 2);
                }
                break;
            case UIWidget.Pivot.BottomLeft:
                {
                    panel.clipOffset = new Vector2(pWidth / 2, pHeight / 2);
                }
                break;
            case UIWidget.Pivot.BottomRight:
                {
                    panel.clipOffset = new Vector2(-pWidth / 2, pHeight / 2);
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
