using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static Transform cameraTransform;

    private void Start()
    {
        AliveBetweenScenes();
    }

    void AliveBetweenScenes()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Global");

        if (obj != null)
        {
            Destroy(obj);
        }

        DontDestroyOnLoad(gameObject);
    }

    private static void FindCamera()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    /// <summary>
    /// For randomisation use Random.Range(min, max). For 2D sounds set spatial to 0f. Volume, pitch, spatial, stereo are have a 0 to 1 range. Clip will be ignored if randomClipSet is not populated.
    /// </summary>
    public static AudioSource PlayAudio(AudioClip clip, AudioClip[] randomClipSet, Vector2 origin, Transform parent = null, float vol = 1f, float pitch = 1f, float spatial = 1f, float stereoPan = 0, float distance = 2600f, bool loop = false)
    {
        if (clip == null && randomClipSet == null)
        {
            return null;
        }

        if(randomClipSet != null)
        {
            clip = randomClipSet[Random.Range(0, randomClipSet.Length)];
        }

        if (cameraTransform == null)
        {
            FindCamera();
        }

        float listenerDistance = (origin - (Vector2)cameraTransform.position).magnitude;

        if (listenerDistance > distance && spatial > 0f) // too far from the camera to hear, ignored if the sound is 2D
        {
            return null;
        }

        GameObject audioObj = new("SoundFX (" + clip.name + ")", typeof(AudioSource));
        AudioSource audioSource = audioObj.GetComponent<AudioSource>();

        Debug.DrawRay(origin, Vector3.up, Color.magenta, 2f);

        audioSource.transform.position = origin;

        audioSource.playOnAwake = false;

        audioSource.clip = clip;

        audioSource.pitch = pitch;
        audioSource.loop = loop;

        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = distance;
        audioSource.minDistance = distance / 100f;

        audioSource.spatialBlend = spatial;
        audioSource.maxDistance = distance;

        audioSource.dopplerLevel = .5f;
        audioSource.panStereo = stereoPan;

        audioSource.volume = vol;

        if (parent != null)
        {
            audioObj.transform.parent = parent;
        }

        audioSource.Play();

        if (!loop)
        {
            Destroy(audioObj, clip.length / audioSource.pitch);
        }

        return audioSource;
    }
}