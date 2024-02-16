using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    private static AudioSource currentTrack;
    private static AudioClip newTrack;
    private static List<AudioClip> currentPlaylist;

    private static float curTime = 0f;

    private static bool changingTrack = false;
    private static bool curLoop = false;
    private static float curFadeSpeed = 2f;

    private static Transform globalTransform;

    private void Update()
    {
        if (changingTrack)
        {
            if (currentTrack != null && currentTrack.volume > 0)
            {
                currentTrack.volume -= Time.deltaTime * curFadeSpeed;
            }
            else
            {
                if (currentTrack != null)
                {
                    Destroy(currentTrack.gameObject);
                    currentTrack = null;
                }

                changingTrack = false;

                if (newTrack != null)
                {
                    ChangeTrack(newTrack, curLoop, curFadeSpeed);
                }
            }
        }
        else
        {
            if (currentPlaylist != null)
            {
                curTime -= Time.unscaledDeltaTime;

                if (curTime <= 0f)
                {
                    List<AudioClip> tempList = currentPlaylist;
                    if (tempList.Contains(newTrack))
                    {
                        tempList.Remove(newTrack);
                    }

                    AudioClip track = tempList[Random.Range(0, tempList.Count)];
                    ChangeTrack(track, false);
                }
            }
        }
    }

    public static void StartPlaylist(List<AudioClip> playlist)
    {
        currentPlaylist = playlist;
        curTime = 0f;
    }

    public static void ChangeTrack(AudioClip track, bool loop, float fadeSpeed = .6f)
    {
        curFadeSpeed = fadeSpeed;
        curLoop = loop;

        if (currentPlaylist != null && !currentPlaylist.Contains(track))
        {
            currentPlaylist = null;
        }

        if (currentTrack != null)
        {
            newTrack = track;
            changingTrack = true;
            return;
        }

        if (globalTransform == null)
        {
            globalTransform = GameObject.FindWithTag("Global").transform;
        }

        currentTrack = AudioManager.PlayAudio(AudioType.music, track, null, Vector2.zero, globalTransform, 1f, 1f, 0f, 0f, 0f, loop);
        curTime = track.length;
    }
}
