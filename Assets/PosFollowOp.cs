using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosFollowOp : MonoBehaviour
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
        Vector3 pos = guiCamera.WorldToScreenPoint(this.transform.position);         //获取UI界面的屏幕坐标
        pos.z = 1f;//设置为零时转换后的pos全为0,屏幕空间的原因，被坑过的我提醒大家，切记要改！
        pos = worldcamera.ScreenToWorldPoint(pos);                              //将屏幕坐标转换为世界坐标
        //pos.y = 0f;
        TargetObject.transform.position = new Vector3(pos.x, pos.y, pos.z);      //将修改过的坐标赋给目标物体坐标
    }
}
