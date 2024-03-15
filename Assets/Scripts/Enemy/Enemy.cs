using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector3 spawnPosition;

    public virtual void GetHit(int damage)
    {

    }

    public virtual void ResetPosition()
    {
        transform.position = spawnPosition;
    }
}
