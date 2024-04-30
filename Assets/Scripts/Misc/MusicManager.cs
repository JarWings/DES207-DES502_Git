using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public AudioSource currentTrack;
    public AudioClip newTrack;
    public List<AudioClip> currentPlaylist;

    public bool changingTrack = false;
    public float curTime = 0f;
    public bool curLoop = false;
    public float curFadeSpeed = 2f;

    private void Update()
    {
        if (changingTrack)
        {
            if (currentTrack != null && currentTrack.volume > 0 && curFadeSpeed > 0f)
            {
                currentTrack.volume -= Time.unscaledDeltaTime * curFadeSpeed;
            }
            else
            {
                if (currentTrack != null)
                {
                    Destroy(currentTrack.gameObject);
                    currentTrack = null;
                }

                changingTrack = false;

                if (newTrack != null) ChangeTrack(newTrack, curLoop, curFadeSpeed);
            }
        }
        else
        {
            if (currentPlaylist != null && currentPlaylist.Count > 0)
            {
                curTime -= Time.unscaledDeltaTime;

                if (curTime <= 0f)
                {
                    List<AudioClip> tempList = currentPlaylist;
                    if (tempList.Contains(newTrack)) tempList.Remove(newTrack);

                    AudioClip track = tempList[Random.Range(0, tempList.Count)];
                    ChangeTrack(track, false);
                }
            }
        }
    }

    public static AudioClip GetCurrentTrack()
    {
        MusicManager globalMusicManager = GetMusicManager();

        return globalMusicManager.newTrack;
    }

    public static void StartPlaylist(List<AudioClip> playlist)
    {
        MusicManager globalMusicManager = GetMusicManager();

        globalMusicManager.currentPlaylist = playlist;
        globalMusicManager.curTime = 0f;
    }

    public static void ChangeTrack(AudioClip track, bool loop, float fadeSpeed = .6f)
    {
        MusicManager globalMusicManager = GetMusicManager();

        if (track == null && globalMusicManager.currentTrack == null) return;

        if (track == null) globalMusicManager.currentTrack.Stop();

        globalMusicManager.curFadeSpeed = fadeSpeed;
        globalMusicManager.curLoop = loop;

        if (globalMusicManager.currentPlaylist != null && !globalMusicManager.currentPlaylist.Contains(track))
        {
            globalMusicManager.currentPlaylist = null;
        }

        if (globalMusicManager.currentTrack != null)
        {
            globalMusicManager.newTrack = track;
            globalMusicManager.changingTrack = true;
            return;
        }


        globalMusicManager.currentTrack = AudioManager.PlayAudio(AudioType.music, track, null, Vector2.zero, globalMusicManager.transform, 1f, 1f, 0f, 0f, 0f, loop);
        globalMusicManager.curTime = track.length;
    }
	
	public static IEnumerator DelayTrackChange(AudioClip track, bool loop, float fadeSpeed, float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		ChangeTrack(track, loop, fadeSpeed);
	}

    public static void ChangeVolume(float vol, float speed = 1f)
    {
        MusicManager globalMusicManager = GetMusicManager();

        if (globalMusicManager.currentTrack == null) return;

        //globalMusicManager.StopAllCoroutines();
        globalMusicManager.StartCoroutine(AudioManager.FadeSource(globalMusicManager.currentTrack, vol, speed));
    }

    public static MusicManager GetMusicManager() // replace with instance in future!
    {
        return GameObject.FindWithTag("Global").GetComponent<MusicManager>();
    }
}