using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public float talkRange = 5f;
    public List<Line> Dialogue = new();
    public int priority = 0;

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
        MusicManager.ChangeTrack(music, true);
    }
}
