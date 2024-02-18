using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 主角的Transform
    public float smoothing = 5f; // 相机跟随的平滑度

    private float initialY; // 相机初始的Y轴坐标

    void Start()
    {
        // 存储相机初始的Y轴坐标
        initialY = transform.position.y;
    }

    void LateUpdate()
    {
        // 如果目标（主角）存在
        if (target != null)
        {
            // 创建一个新的Vector3，X轴来自主角，Y轴保持相机的初始值，Z轴为相机当前的Z轴坐标
            Vector3 targetCamPos = new Vector3(target.position.x, initialY, transform.position.z);

            // 平滑地插值相机位置到目标位置
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }
}