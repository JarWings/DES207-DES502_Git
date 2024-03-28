using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public enum AudioType { master, soundFX, music, ui }

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainAudioMix;
    public AudioMixerGroup fxGroup, musicGroup, uiGroup;
    private static Transform cameraTransform;

    private static void FindCamera()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    #region function summary
    /// <param name="type">The type of audio effect this is, used for individual volume control.</param>
    /// <param name="clip">The sound to be played.</param>
    /// <param name="randomClipSet">Plays a random sound from this array, clip will be ignored.</param>
    /// <param name="origin">Where the sound originates from in the scene, pass vector.zero if using a 2D sound.</param>
    /// <param name="parent">Transform this sound is attached to, pass as null if no parent is needed.</param>
    /// <param name="vol">How loud the volume is, use values 0f to 1f. Use Random.Range(min, max) for randomisation.</param>
    /// <param name="pitch">The pitch of the sound, use values 0f to 1f. Use Random.Range(min, max) for randomisation.</param>
    /// <param name="spatial">For 2D sounds set to 0f, for 3D sounds set to 1f.</param>
    /// <param name="stereoPan">0f for default, -1f for left, 1f for right pan.</param>
    /// <param name="distance">How far the sound can be heard from.</param>
    /// <param name="loop">Set to true for sound looping, false for one-time. Assign this function to an AudioSource variable in order to stop the loop later.</param>
    /// <param name="delay">How long before the sound plays, leave at 0f for instant playback.</param>
    /// <param name="randomStart">Set to true if you want the sound to start from a random point within the clip.</param>
    #endregion
    public static AudioSource PlayAudio(AudioType type, AudioClip clip, AudioClip[] randomClipSet, Vector2 origin, Transform parent = null, float vol = 1f, float pitch = 1f, float spatial = 1f, float stereoPan = 0, float distance = 2600f, bool loop = false, float delay = 0f, bool randomStart = false)
    {
        if (clip == null && randomClipSet == null) return null;

        if (randomClipSet != null) clip = randomClipSet[Random.Range(0, randomClipSet.Length)];

        if (cameraTransform == null) FindCamera();

        float listenerDistance = (origin - (Vector2)cameraTransform.position).magnitude;

        if (listenerDistance > distance && spatial > 0f && !loop) return null;

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

        if (parent != null) audioObj.transform.parent = parent;

        if (randomStart) audioSource.time = Random.Range(0f, clip.length);

        audioSource.PlayDelayed(delay);

        if (!loop) Destroy(audioObj, (clip.length / audioSource.pitch) + delay);

        return audioSource;
    }

    public static IEnumerator FadeSource(AudioSource source, float targetVolume, float speed = 2f, bool destroyOnEnd = false)
    {
        while (source.volume != targetVolume && source != null)
        {
            source.volume = Mathf.MoveTowards(source.volume, targetVolume, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }

        if (destroyOnEnd && source != null) Destroy(source.gameObject);
    }
}