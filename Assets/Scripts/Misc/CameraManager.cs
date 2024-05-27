using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public CinemachineVirtualCamera cineCam;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Transform player = GameObject.FindWithTag("Player").transform;
        cineCam.Follow = player.transform;
    }

    public static void Shake(float time) 
    {
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.ShakeEnum(time));
    }

    public IEnumerator ShakeEnum(float time) 
    {
        CinemachineBasicMultiChannelPerlin cinemachinePerlin = Instance.cineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachinePerlin.m_AmplitudeGain = time;

        while (cinemachinePerlin.m_AmplitudeGain > 0f) 
        {
            cinemachinePerlin.m_AmplitudeGain -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public static void SetFOV(float fov)
    {
        Instance.cineCam.m_Lens.FieldOfView = fov;
        Instance.cineCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = fov < 65 ? 0.62f : .5f;
    }

}
