using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRain : MonoBehaviour
{
    public float speed = 7;
    Animator anim;
    bool explode = false;

    private void Start() 
    { 
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (!explode)
        {
            transform.position += speed * Time.deltaTime * Vector3.down;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetTrigger("Explode");
        explode = true;
        Invoke("DestroyThis", 1);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
