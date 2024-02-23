using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public Animator anim;
    public bool oneTimeEvent = true;
    public UnityEvent openEvent;
    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Player")
        {
            return;
        }

        if(!triggered || !oneTimeEvent)
        {
            openEvent.Invoke();
            triggered = true;
        }

        anim.SetTrigger("Open");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }
        anim.SetTrigger("Close");
    }
}
