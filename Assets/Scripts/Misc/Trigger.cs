using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public bool oneTimeEvent = true;
    public UnityEvent enterEvent, exitEvent;
    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }

        if (!triggered || !oneTimeEvent)
        {
            enterEvent.Invoke();
            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }

        if (!triggered || !oneTimeEvent)
        {
            exitEvent.Invoke();
            triggered = true;
        }
    }

    public void EndLevel()
    {
        LeaderboardManager.AddScore("");
        SceneChangeManager.LoadScene("MainMenu");
    }

    public void ChangeMusic(AudioClip track)
    {
        MusicManager.ChangeTrack(track, true);
    }
}
