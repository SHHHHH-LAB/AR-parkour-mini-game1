using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    // 双指的坐标
    private Vector2 oldPos1;
    private Vector2 oldPos2;
    // 摄像机初始位置和旋转
    private Vector3 orgPos;
    private Quaternion orgRot;

    // 控制旋转和缩放速度
    public float rotateSpeed = 0.2f;
    public float zoomSpeed = 0.05f;
    public GameObject originPoint; // 旋转和缩放的中心点，即角色位置

    void Start()
    {
        // 记录摄像机初始位置和旋转
        orgPos = transform.position;
        orgRot = transform.rotation;
    }

    void Update()
    {
        int touchCount = Input.touchCount;
        if (touchCount == 0) return; //没有触摸时不处理

        // 单指触控：角色绕 y 轴旋转
        if (touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                //获取本次触摸与上次触摸位置的位移差（手指滑动的方向与距离）
                //touch.deltaPosition表示当前帧手指相对上一帧移动了多少像素，用来判断滑动方向与速度。
                Vector2 deltaPos = touch.deltaPosition;
                // 绕 originPoint 的 y 轴旋转
                transform.RotateAround(originPoint.transform.position, Vector3.up, deltaPos.x * rotateSpeed);
            }
        }
        // 双指缩放角色
        else if (touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 获取当前两指位置
            Vector2 newPos1 = touch1.position;
            Vector2 newPos2 = touch2.position;

            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                // 计算两指间距变化量
                float oldDistance = Vector2.Distance(oldPos1, oldPos2);
                float newDistance = Vector2.Distance(newPos1, newPos2);
                float diff = newDistance - oldDistance;

                // 根据手指间距变化缩放角色（沿自身 z 轴）
                transform.Translate(Vector3.forward * diff * zoomSpeed, Space.Self);
            }

            // 更新上一帧两指位置，用于下次计算间距变化
            oldPos1 = newPos1;
            oldPos2 = newPos2;
        }
    }
}
