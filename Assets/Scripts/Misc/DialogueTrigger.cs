using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public float talkRange = 5f;
    public List<Line> Dialogue = new();
    public int priority = 0;

    public AudioClip introMusic;
    public AudioClip mainMusic;

    private void Update()
    {
        if(Physics2D.CircleCast(transform.position, talkRange, Vector2.up, talkRange, LayerMask.GetMask("Player")))
        {
            DialogueManager.DisplayDialogue(Dialogue, priority);
            enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, talkRange);
    }

    public void TriggerMusic(AudioClip music) 
    {
        MusicManager.ChangeTrack(music, true, 1.2f);
    }

    public void FindJournal(string journal) 
    {
        JournalManager.FindEntry(journal);
    }

    public void IntroMainMusic()
    {
        MusicManager.ChangeTrack(introMusic, false, 0f);
        MusicManager.GetMusicManager().StartCoroutine(MusicManager.DelayTrackChange(mainMusic, true, 0f, introMusic.length));
    }
}
