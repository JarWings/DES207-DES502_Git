using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            collision.transform.GetComponent<Boss>().GetHit(1);
        }
        else if(collision.CompareTag("Mimic"))
        {
            collision.transform.GetComponent<Mimic>().GetHit(1);
        }
        else if (collision.CompareTag("Maths Teacher"))
        {
            collision.transform.GetComponent<MathsTeacher>().GetHit(1);
        }
    }
}
