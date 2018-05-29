using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
public enum FlipMode
{
    RightToLeft,
    LeftToRight
}
/// <summary>
/// 大概过程是这样
/// 准备  容器四个  一个用来裁剪翻页  一个用来裁剪下一页  还有两个分别用来裁剪阴影
///       页面四个  想象一下翻书  手里拿着一张纸有两页  左右各一页
///       阴影两个  图片应该是对称的两个图片
///       拿在手上的两个页面暂时叫   翻页左  翻页右
///       另外两边的页面叫          静止左  静止右
///  开始
///      先设置好两个裁剪页面的容器  要设置的和页面一样宽  高要设置大一点  不然翻得角度大会剪掉页面
///      然后设置裁剪页面容器的锚点  在panel上 我用clipoffset来设置的  设置在左下或者右下偏上一点
///      设置好两个裁剪阴影的容器  我设置的和页面一个size的
///      设置好阴影的对齐方式  左下或者右下  因为阴影要跟着大容器走
///      
///  运动的过程就跟翻书一样，假设把书翻一点点，折一个角落
///  先假设从左往右折，折好之后，有两个点注意下
///  一个是被折的那张纸角落那个点   就是翻页时手粘住的点叫t0
///  还有一个就是折痕点，折痕点会有两个，取下面一个点t1
///  t1是跟着t0动的 整个翻书的过程就可以理解为t0的运动带动了其他物体的移动
///  t0点就是翻页右面的右下角点
///  t1就是两个裁剪页面容器的点  两个裁剪容器一个裁剪静止左 一个裁剪翻页右  旋转的角度是一样的
///  旋转的角度alpha可以根据t0和t1来计算(这个里面计算小小复杂)
///  当然t1也是t0算出来的，t0是已知点
///  整理过程如下：
///  向右翻
///  设置两个裁剪页面的容器锚点
///  设置翻页右的位置  设置为静止左的位置（对齐方式也是一样）
///  设置翻页左的对齐方式为右下  因为翻书的时候  是从右下翻的  从右上翻还没有开发出来
///  已知t0为动态已知点
///  计算出折痕下点t1，翻页右需要旋转的角度和裁剪容器需要旋转的角度（自己研究去）
///  设置翻页右的位置和旋转角度
///  设置两个裁剪容器的位置和旋转角度（值相同）
///  设置翻页右为裁剪容器的儿子，保持自己的transform
///  先设置阴影为裁剪容器的子物体，初始化（保持旋转角度和锚点重合），然后再设置给阴影裁剪容器当儿子，剪掉
///      超出页面的部分
///  刷新物体，保证裁剪成功
///  有两点需要注意
///  1. 需要需要裁剪的两个页面不能跟着裁剪容器动，要等容器动完了，再给他当儿子
///  2. 阴影要跟着裁剪页面的容器动，不能跟着阴影的裁剪容器动
/// </summary>
[ExecuteInEditMode]
public class Book : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField]
    UIPanel BookPanel;
    public Texture background;
    public Texture[] bookPages;
    public bool interactable = true;
    public bool enableShadowEffect = true;
    //represent the index of the sprite shown in the right page
    public int currentPage = 0;
    public int TotalPageCount
    {
        get { return bookPages.Length; }
    }
    public Vector3 EndBottomLeft
    {
        get { return ebl; }
    }
    public Vector3 EndBottomRight
    {
        get { return ebr; }
    }
    public UIPanel ClippingPlane;
    public UIPanel NextPageClip;
    public UITexture Shadow;
    public UITexture ShadowLTR;
    public UITexture Left;
    public UITexture LeftNext;
    public UITexture Right;
    public UITexture RightNext;
    public UnityEvent OnFlip;
    /// <summary>
    /// 书半径
    /// </summary>
    float radius1, radius2;
    //Spine Bottom
    /// <summary>
    /// 书缝底部
    /// </summary>
    Vector3 sb;
    //Spine Top
    /// <summary>
    /// 书脊上部
    /// </summary>
    Vector3 st;
    //corner of the page
    /// <summary>
    /// 被翻的页的左下角
    /// </summary>
    Vector3 c;
    //Edge Bottom Right
    /// <summary>
    /// 页面右下侧
    /// </summary>
    Vector3 ebr;
    //Edge Bottom Left
    /// <summary>
    /// 页面左下侧
    /// </summary>
    Vector3 ebl;
    //follow point 
    /// <summary>
    /// 跟随点
    /// </summary>
    Vector3 f;
    bool pageDragging = false;
    //current flip mode
    FlipMode mode;

    public Vector2 ClipPlaneRTLPivot;
    public Vector2 NextClipPlaneRTLPivot;
    void Start()
    {
        float pageWidth = Left.width;
        float pageHeight = Left.height;
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        UpdateSprites();
        //书缝隙中间的下点
        Vector3 globalsb = BookPanel.transform.position + new Vector3(0, -pageHeight / 2);
        sb = transformPoint(globalsb);
        //书右下角
        Vector3 globalebr = BookPanel.transform.position + new Vector3(pageWidth, -pageHeight / 2);
        ebr = transformPoint(globalebr);
        //书左下角
        Vector3 globalebl = BookPanel.transform.position + new Vector3(-pageWidth, -pageHeight / 2);
        ebl = transformPoint(globalebl);
        //书缝隙中间的下点
        Vector3 globalst = BookPanel.transform.position + new Vector3(0, pageHeight / 2);
        st = transformPoint(globalst);
        radius1 = Vector2.Distance(sb, ebr);
        float scaledPageWidth = pageWidth;
        float scaledPageHeight = pageHeight;
        radius2 = Mathf.Sqrt(scaledPageWidth * scaledPageWidth + scaledPageHeight * scaledPageHeight);
        //ClippingPlane.rectTransform.sizeDelta = new Vector2(scaledPageWidth*2, scaledPageHeight + scaledPageWidth * 2);
        //Shadow.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);
        //ShadowLTR.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);
        //NextPageClip.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);

        ClipPlaneRTLPivot = ClippingPlane.clipOffset;
        NextClipPlaneRTLPivot = NextPageClip.clipOffset;
    }
    public Vector3 transformPoint(Vector3 global)
    {
        //Vector2 localPos = BookPanel.InverseTransformPoint(global);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(BookPanel, global, null, out localPos);
        return global;
    }
    void Update()
    {
        if (pageDragging && interactable)
        {
            UpdateBook();
        }
        //Debug.Log("mouse local pos:" + transformPoint(Input.mousePosition));
        //Debug.Log("mouse  pos:" + Input.mousePosition);
    }
    public void UpdateBook()
    {
        f = Vector3.Lerp(f, transformPoint(Input.mousePosition), Time.deltaTime * 10);
        if (mode == FlipMode.RightToLeft)
            UpdateBookRTLToPoint(f);
        else
            UpdateBookLTRToPoint(f);
    }
    public void UpdateBookLTRToPoint(Vector3 followLocation)
    {
        mode = FlipMode.LeftToRight;
        f = followLocation;
        //初始化阴影，左图当前页移动到翻页的当页 下
        ShadowLTR.transform.SetParent(ClippingPlane.transform, true);
        ShadowLTR.transform.localPosition = new Vector3(0, 0, 0);
        ShadowLTR.transform.localEulerAngles = new Vector3(0, 0, 0);

        Left.transform.SetParent(ClippingPlane.transform, true);
        Fresh(Left.transform);
        //右图当前页不动
        Right.transform.SetParent(BookPanel.transform, true);

        //左图当前页不动
        LeftNext.transform.SetParent(BookPanel.transform, true);

        c = Calc_C_Position(followLocation);
        Vector3 t1;
        float T0_T1_Angle = Calc_T0_T1_Angle(c, ebl, out t1);
        if (T0_T1_Angle < 0) T0_T1_Angle += 180;

        //转动翻页遮罩
        ClippingPlane.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
        ClippingPlane.transform.position = BookPanel.transform.TransformPoint(t1);

        //page position and angle
        //移动新页的位置
        Left.transform.position = BookPanel.transform.TransformPoint(c);
        float C_T1_dy = t1.y - c.y;
        float C_T1_dx = t1.x - c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        //转动新一页的角度
        Left.transform.eulerAngles = new Vector3(0, 0, C_T1_Angle - 180);

        //转动下页遮罩
        NextPageClip.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
        NextPageClip.transform.position = BookPanel.transform.TransformPoint(t1);
        //静止下页接受裁剪
        LeftNext.transform.SetParent(NextPageClip.transform, true);
        Fresh(LeftNext.transform);

        Right.transform.SetParent(ClippingPlane.transform, true);
        Fresh(Left.transform);
        Right.transform.SetAsFirstSibling();
        //阴影裁剪
        ShadowLTR.transform.SetParent(Left.GetComponentInChildren<UIPanel>().transform, true);
        Fresh(ShadowLTR.transform);

    }
    public void UpdateBookRTLToPoint(Vector3 followLocation)
    {

        mode = FlipMode.RightToLeft;
        f = followLocation;
        //设置阴影位置， 阴影要跟着裁剪翻页的容器走，因为他们的旋转角度和位置是一样的
        Shadow.transform.SetParent(ClippingPlane.transform, true);
        Shadow.transform.localPosition = new Vector3(0, 0, 0);
        Shadow.transform.localEulerAngles = new Vector3(0, 0, 0);

        Right.transform.SetParent(ClippingPlane.transform, true);
        //隐藏翻页右面  因为阴影需要裁剪  阴影设进容器时  会显示不出裁剪效果  需要刷新一下物体
        Fresh(Right.transform);
        //翻页左不需要跟着旋转
        Left.transform.SetParent(BookPanel.transform, true);

        RightNext.transform.SetParent(BookPanel.transform, true);
        c = Calc_C_Position(followLocation);
        Vector3 t1;
        float T0_T1_Angle = Calc_T0_T1_Angle(c, ebr, out t1);
        if (T0_T1_Angle >= -90) T0_T1_Angle -= 180;

        //ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        //设置翻页裁剪容器的角度和位置
        ClippingPlane.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
        ClippingPlane.transform.position = BookPanel.transform.TransformPoint(t1);

        //page position and angle
        Right.transform.position = BookPanel.transform.TransformPoint(c);
        float C_T1_dy = t1.y - c.y;
        float C_T1_dx = t1.x - c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        Right.transform.eulerAngles = new Vector3(0, 0, C_T1_Angle);
        //设置静止页裁剪容器的角度和位置
        NextPageClip.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
        NextPageClip.transform.position = BookPanel.transform.TransformPoint(t1);

        print("t0 : " + BookPanel.transform.TransformPoint(c) + ", t1 : " + BookPanel.transform.TransformPoint(t1));

        RightNext.transform.SetParent(NextPageClip.transform, true);
        Fresh(RightNext.transform);

        //翻页左接受裁剪
        Left.transform.SetParent(ClippingPlane.transform, true);
        Fresh(Left.transform);

        Left.transform.SetAsFirstSibling();
        //阴影接受裁剪
        Shadow.transform.SetParent(Right.GetComponentInChildren<UIPanel>().transform, true);
        Fresh(Shadow.transform);
    }

    void Fresh(Transform trans)
    {
        trans.gameObject.SetActive(false);
        trans.localScale = Vector3.one;
        trans.gameObject.SetActive(true);
    }

    /// <summary>
    ///计算t0 t1的夹角，t0为翻起页脚和原页脚的中心点, t1为翻页时页面下方的交点
    /// </summary>
    /// <param name="c">翻起点</param>
    /// <param name="bookCorner">页脚点</param>
    /// <param name="t1">翻页时页面下方的交点</param>
    /// <returns></returns>
    private float Calc_T0_T1_Angle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
    {
        Vector3 t0 = (c + bookCorner) / 2;
        float T0_CORNER_dy = bookCorner.y - t0.y;
        float T0_CORNER_dx = bookCorner.x - t0.x;
        float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
        float T0_T1_Angle = 90 - T0_CORNER_Angle;

        float T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
        T1_X = normalizeT1X(T1_X, bookCorner, sb);
        t1 = new Vector3(T1_X, sb.y, 0);
        ////////////////////////////////////////////////
        //clipping plane angle=T0_T1_Angle
        float T0_T1_dy = t1.y - t0.y;
        float T0_T1_dx = t1.x - t0.x;
        T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
        return T0_T1_Angle;
    }
    private float normalizeT1X(float t1, Vector3 corner, Vector3 sb)
    {
        if (t1 > sb.x && sb.x > corner.x)
            return sb.x;
        if (t1 < sb.x && sb.x < corner.x)
            return sb.x;
        return t1;
    }
    /// <summary>
    /// 限制跟随点的范围  不能把书页撕破
    /// </summary>
    /// <param name="followLocation"></param>
    /// <returns></returns>
    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        Vector3 c;
        f = followLocation;
        //限制翻页的点不能超过书的半径  不能把页面撕破
        float F_SB_dy = f.y - sb.y;
        float F_SB_dx = f.x - sb.x;
        float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
        Vector3 r1 = new Vector3(radius1 * Mathf.Cos(F_SB_Angle), radius1 * Mathf.Sin(F_SB_Angle), 0) + sb;

        float F_SB_distance = Vector2.Distance(f, sb);
        if (F_SB_distance < radius1)
            c = f;
        else
            c = r1;

        //限制点不能往左撕破  点要在左侧书页的范围内
        float F_ST_dy = c.y - st.y;
        float F_ST_dx = c.x - st.x;
        float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
        Vector3 r2 = new Vector3(radius2 * Mathf.Cos(F_ST_Angle),
           radius2 * Mathf.Sin(F_ST_Angle), 0) + st;
        float C_ST_distance = Vector2.Distance(c, st);
        if (C_ST_distance > radius2)
            c = r2;
        return c;
    }
    public void DragRightPageToPoint(Vector3 point)
    {
        if (currentPage >= bookPages.Length) return;
        pageDragging = true;
        mode = FlipMode.RightToLeft;
        f = point;

        DepthRighLeftChange(true);

        NextPageClip.clipOffset = NextClipPlaneRTLPivot;
        ClippingPlane.clipOffset = ClipPlaneRTLPivot;
        //NextPageClip.transform.pivot = new Vector2(0, 0.12f);
        //ClippingPlane.transform.pivot = new Vector2(1, 0.35f);

        //Left.rectTransform.pivot = new Vector2(0, 0);
        Left.gameObject.SetActive(true);
        //翻页的左页面不动  保持在静止页面的右页面的位置
        //先要设置翻页左的对方方式和子容器的对齐方式
        var stillPivot = RightNext.pivot;
        PanelAnchorsAdjust(Left.GetComponentInChildren<UIPanel>(), stillPivot);
        Left.GetComponent<UIWidget>().pivot = stillPivot;
        //设置静止右页的位置给翻页左
        Left.transform.position = RightNext.transform.position;
        Left.transform.eulerAngles = new Vector3(0, 0, 0);
        //左页的内容变为翻页右的内容
        Left.mainTexture = (currentPage < bookPages.Length) ? bookPages[currentPage] : background;
        Left.transform.SetAsFirstSibling();

        var movePivot = UIWidget.Pivot.BottomLeft;
        PanelAnchorsAdjust(Right.GetComponentInChildren<UIPanel>(), movePivot);
        Right.pivot = movePivot;

        //翻页右显示翻页左的背面
        Right.mainTexture = (currentPage < bookPages.Length - 1) ? bookPages[currentPage + 1] : background;
        //静止右显示翻页右的下页
        RightNext.mainTexture = (currentPage < bookPages.Length - 2) ? bookPages[currentPage + 2] : background;
        //静止左页升为太子位
        LeftNext.transform.SetAsFirstSibling();
        //显示阴影
        if (enableShadowEffect) Shadow.gameObject.SetActive(true);

        UpdateBookRTLToPoint(f);
    }
    public void OnMouseDragRightPage()
    {
        if (interactable)
            DragRightPageToPoint(transformPoint(Input.mousePosition));

    }

    void DepthRighLeftChange(bool rOverl)
    {
        int max = Left.depth;
        int min = Left.depth;
        if (Left.depth > Right.depth)
        {
            max = Left.depth;
            min = Right.depth;
        }
        else if (Left.depth == Right.depth)
        {
            min = Left.depth;
            max = min + 1;
        }
        else
        {
            min = Left.depth;
            max = Right.depth;
        }

        if (rOverl)
        {
            Right.depth = max;
            Left.depth = min;
        }
        else
        {
            Right.depth = min;
            Left.depth = max;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="point"></param>
    public void DragLeftPageToPoint(Vector3 point)
    {
        if (currentPage <= 0) return;
        pageDragging = true;
        mode = FlipMode.LeftToRight;
        f = point;

        DepthRighLeftChange(false);


        NextPageClip.clipOffset = new Vector2(-NextClipPlaneRTLPivot.x, NextClipPlaneRTLPivot.y);
        ClippingPlane.clipOffset = new Vector2(-ClipPlaneRTLPivot.x, ClipPlaneRTLPivot.y);
        //NextPageClip.rectTransform.pivot = new Vector2(1, 0.12f);
        //ClippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

        Right.gameObject.SetActive(true);
        //先设置对齐方式相同 再设置位置
        var stillPivot = LeftNext.GetComponent<UIWidget>().pivot;
        PanelAnchorsAdjust(Right.GetComponentInChildren<UIPanel>(), stillPivot);
        Right.GetComponent<UIWidget>().pivot = stillPivot;
        Right.transform.position = LeftNext.transform.position;
        //设置翻页右的图片
        Right.mainTexture = bookPages[currentPage - 1];
        Right.transform.eulerAngles = new Vector3(0, 0, 0);
        Right.transform.SetAsFirstSibling();

        Left.gameObject.SetActive(true);
        //Left.rectTransform.pivot = new Vector2(1, 0);
        var movePivot = UIWidget.Pivot.BottomRight;
        PanelAnchorsAdjust(Left.GetComponentInChildren<UIPanel>(), movePivot);
        Left.pivot = movePivot;


        //Left.transform.position = LeftNext.transform.position;
        //Left.transform.eulerAngles = new Vector3(0, 0, 0);
        //翻页左换页
        Left.mainTexture = (currentPage >= 2) ? bookPages[currentPage - 2] : background;
        LeftNext.mainTexture = (currentPage >= 3) ? bookPages[currentPage - 3] : background;

        RightNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) ShadowLTR.gameObject.SetActive(true);
        UpdateBookLTRToPoint(f);
    }
    public void OnMouseDragLeftPage()
    {
        if (interactable)
            DragLeftPageToPoint(transformPoint(Input.mousePosition));

    }
    public void OnMouseRelease()
    {
        if (interactable)
            ReleasePage();
    }
    public void ReleasePage()
    {
        if (pageDragging)
        {
            pageDragging = false;
            float distanceToLeft = Vector2.Distance(c, ebl);
            float distanceToRight = Vector2.Distance(c, ebr);
            if (distanceToRight < distanceToLeft && mode == FlipMode.RightToLeft)
                TweenBack();
            else if (distanceToRight > distanceToLeft && mode == FlipMode.LeftToRight)
                TweenBack();
            else
                TweenForward();
        }
    }
    Coroutine currentCoroutine;
    void UpdateSprites()
    {
        LeftNext.mainTexture = (currentPage > 0 && currentPage <= bookPages.Length) ? bookPages[currentPage - 1] : background;
        RightNext.mainTexture = (currentPage >= 0 && currentPage < bookPages.Length) ? bookPages[currentPage] : background;
    }
    public void TweenForward()
    {
        if (mode == FlipMode.RightToLeft)
            currentCoroutine = StartCoroutine(TweenTo(ebl, 0.15f, () => { Flip(); }));
        else
            currentCoroutine = StartCoroutine(TweenTo(ebr, 0.15f, () => { Flip(); }));
    }
    void Flip()
    {
        if (mode == FlipMode.RightToLeft)
            currentPage += 2;
        else
            currentPage -= 2;
        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.transform.SetParent(BookPanel.transform, true);
        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        Right.transform.SetParent(BookPanel.transform, true);
        RightNext.transform.SetParent(BookPanel.transform, true);
        UpdateSprites();
        Shadow.gameObject.SetActive(false);
        ShadowLTR.gameObject.SetActive(false);
        if (OnFlip != null)
            OnFlip.Invoke();
    }
    public void TweenBack()
    {
        if (mode == FlipMode.RightToLeft)
        {
            currentCoroutine = StartCoroutine(TweenTo(ebr, 0.15f,
                () =>
                {
                    UpdateSprites();
                    RightNext.transform.SetParent(BookPanel.transform);
                    Right.transform.SetParent(BookPanel.transform);

                    Left.gameObject.SetActive(false);
                    Right.gameObject.SetActive(false);
                    pageDragging = false;
                }
                ));
        }
        else
        {
            currentCoroutine = StartCoroutine(TweenTo(ebl, 0.15f,
                () =>
                {
                    UpdateSprites();

                    LeftNext.transform.SetParent(BookPanel.transform);
                    Left.transform.SetParent(BookPanel.transform);

                    Left.gameObject.SetActive(false);
                    Right.gameObject.SetActive(false);
                    pageDragging = false;
                }
                ));
        }
    }
    public IEnumerator TweenTo(Vector3 to, float duration, System.Action onFinish)
    {
        int steps = (int)(duration / 0.025f);
        Vector3 displacement = (to - f) / steps;
        for (int i = 0; i < steps - 1; i++)
        {
            if (mode == FlipMode.RightToLeft)
                UpdateBookRTLToPoint(f + displacement);
            else
                UpdateBookLTRToPoint(f + displacement);

            yield return new WaitForSeconds(0.025f);
        }
        if (onFinish != null)
            onFinish();
    }

    void PanelAnchorsAdjust(UIPanel panel,  UIWidget.Pivot pivot)
    {
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
}
