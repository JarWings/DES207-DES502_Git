using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public Door destinationDoor;
    public Animator anim;
    public bool oneTimeEvent = true, inputHeld = false;
    public UnityEvent openEvent;
    private bool triggered = false, atDoor = false;
    public Rigidbody2D playerRbody;
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

        if(playerRbody == null)
        {
            playerRbody = collision.GetComponent<Rigidbody2D>();
        }

        atDoor = true;
        anim.SetTrigger("Open");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }
        atDoor = false;
        anim.SetTrigger("Close");
    }

    private void Update()
    {
        float vert = Input.GetAxisRaw("Vertical");
        if (vert == 1)
        {
            UseDoor();
            inputHeld = true;
        }
        else if (inputHeld)
        {
            inputHeld = false;
        }
    }

    private void UseDoor()
    {
        if(destinationDoor == null || !atDoor || playerRbody == null || inputHeld)
        {
            return;
        }

        playerRbody.position = destinationDoor.transform.position;
        destinationDoor.inputHeld = true;
    }

    private void OnDrawGizmosSelected()
    {
        if(destinationDoor == null)
        {
            return;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, destinationDoor.transform.position);
    }

    public void SpawnObject(GameObject obj)
    {
        GameObject spawnedobj = Instantiate(obj, transform.position, obj.transform.rotation);

        Rigidbody2D[] rigids = spawnedobj.GetComponentsInChildren<Rigidbody2D>();

        for(int i = 0; i < rigids.Length; i++)
        {
            rigids[i].transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            rigids[i].AddForce(Random.insideUnitCircle.normalized * 5f, ForceMode2D.Impulse);
        }
    }
}
