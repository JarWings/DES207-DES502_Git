using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector3 spawnPosition;
    public string journalEntryName;

    public virtual void GetHit(int damage)
    {
        JournalManager.FindEntry(journalEntryName); // adds journal entry for this enemy, leave journalEntryName blank if there's no entry
    }

    public virtual void ResetPosition()
    {
        transform.position = spawnPosition;
    }
}
