using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ���ǵ�Transform
    public float smoothing = 5f; // ��������ƽ����

    private float initialY; // �����ʼ��Y������

    void Start()
    {
        // �洢�����ʼ��Y������
        initialY = transform.position.y;
    }

    void LateUpdate()
    {
        // ���Ŀ�꣨���ǣ�����
        if (target != null)
        {
            // ����һ���µ�Vector3��X���������ǣ�Y�ᱣ������ĳ�ʼֵ��Z��Ϊ�����ǰ��Z������
            Vector3 targetCamPos = new Vector3(target.position.x, initialY, transform.position.z);

            // ƽ���ز�ֵ���λ�õ�Ŀ��λ��
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }
}