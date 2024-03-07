using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera cineCam;
    void Start()
    {
        Transform player = GameObject.FindWithTag("Player").transform;
        cineCam.Follow = player.transform;
    }

}
