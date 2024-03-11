using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    master, soundFX, music, ui
}

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainAudioMix;
    public AudioMixerGroup fxGroup, musicGroup, uiGroup;
    private static Transform cameraTransform;

    private static void FindCamera()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    /// <summary>
    /// For randomisation use Random.Range(min, max). For 2D sounds set spatial to 0f. Volume, pitch, spatial, stereo are have a 0 to 1 range. Clip will be ignored if randomClipSet is not populated.
    /// </summary>
    public static AudioSource PlayAudio(AudioType type, AudioClip clip, AudioClip[] randomClipSet, Vector2 origin, Transform parent = null, float vol = 1f, float pitch = 1f, float spatial = 1f, float stereoPan = 0, float distance = 2600f, bool loop = false, float delay = 0f)
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

        if (listenerDistance > distance && spatial > 0f && !loop) // too far from the camera to hear, ignored if the sound is 2D
        {
            return null;
        }

        GameObject audioObj = new("SoundFX (" + clip.name + ")", typeof(AudioSource));
        AudioSource audioSource = audioObj.GetComponent<AudioSource>();

        AudioManager audioMan = GameObject.FindGameObjectWithTag("Global").GetComponent<AudioManager>();

        switch (type)
        {
            case AudioType.soundFX:
                audioSource.outputAudioMixerGroup = audioMan.fxGroup;
                break;
            case AudioType.music:
                audioSource.outputAudioMixerGroup = audioMan.musicGroup;
                break;
            case AudioType.ui:
                audioSource.outputAudioMixerGroup = audioMan.uiGroup;
                break;
        }

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

        audioSource.PlayDelayed(delay);

        if (!loop)
        {
            Destroy(audioObj, (clip.length / audioSource.pitch) + delay);
        }

        return audioSource;
    }
}