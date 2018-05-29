using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosFollow : MonoBehaviour
{
    public GameObject TargetObject;         //目标物体。这里是指Cube
    Camera worldcamera;         //世界相机。
    Camera guiCamera;           //UI相机
    // Use this for initialization
    void Start()
    {
        worldcamera = NGUITools.FindCameraForLayer(TargetObject.layer);     //这里是通过物体的层获得相应层上的相机
        guiCamera = NGUITools.FindCameraForLayer(this.gameObject.layer);   //通过脚本所在物体的层获得相应层上的相机
    }

    // Update is called once per frame
    void Update()
    {

    }
    void LateUpdate()
    {
        Vector3 pos = worldcamera.WorldToScreenPoint(TargetObject.transform.position);         //获取目标物体的屏幕坐标
        pos = guiCamera.ScreenToWorldPoint(pos);                              //将屏幕坐标转换为UI的世界坐标
        pos.z = 0;                                //由于NGUI 2D界面的Z轴都为0，这里我们将坐标修改为0.只取其X,Y坐标。
        transform.position = new Vector3(pos.x, pos.y, pos.z);      //将修改过的坐标赋给UI界面。这里指Panel_CharacterInfo
    }
}
