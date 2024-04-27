using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class Intro : MonoBehaviour
{
    public VideoPlayer vidPlayer;
    public AudioClip introMusic, loopMusic;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MusicManager.ChangeTrack(null, false, 9999f);

        StartCoroutine(VideoPlay());
    }

    IEnumerator VideoPlay() 
    {
        if(SceneManager.GetActiveScene().buildIndex == 0) SettingsManager.firstBoot = true;

        if(SettingsManager.firstBoot)  yield return new WaitForSeconds(1f);

        vidPlayer.Play();

        MusicManager.ChangeTrack(introMusic, false, 999999f);
        StartCoroutine(DelaySceneLoad((float)vidPlayer.clip.length));
    }

    IEnumerator DelaySceneLoad(float delay) 
    {
        yield return new WaitForSeconds(delay);

        MusicManager.ChangeTrack(loopMusic, true, 999999f);
        SceneChangeManager.LoadScene("MainMenu");
    }
}
