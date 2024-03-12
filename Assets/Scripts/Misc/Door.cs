using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public Sprite interactSprite;
    public Door destinationDoor;
    public Animator anim;
    public bool oneTimeEvent = true, inputHeld = false, keyTriggersEvent = false;
    public UnityEvent openEvent;
    private bool triggered = false, atDoor = false;
    public Rigidbody2D playerRbody;

    public AudioClip openSound;
    public AudioClip closeSound;

    private SpriteRenderer spriteRenderer, interactIcon;

    private void Start()
    {
        TryGetComponent(out spriteRenderer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Player")
        {
            return;
        }

        if((!triggered || !oneTimeEvent) && !keyTriggersEvent)
        {
            openEvent.Invoke();
            triggered = true;
        }

        if(playerRbody == null)
        {
            playerRbody = collision.GetComponent<Rigidbody2D>();
        }

        atDoor = true;

        if(!triggered || destinationDoor != null)
        {
            if(interactIcon == null)
            {
                interactIcon = IconManager.CreateSprite();
            }

            IconManager.UpdateInteractIcon(interactIcon, transform.position + new Vector3(0f, 2f), interactSprite);
        }

        if (!keyTriggersEvent)
        {
            AudioManager.PlayAudio(AudioType.soundFX, openSound, null, transform.position, null, 1, Random.Range(.9f, 1.1f), 1, 0, 40);
            anim.SetTrigger("Open");
            spriteRenderer.sortingOrder = -2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }
        atDoor = false;

        if(!keyTriggersEvent)
        {
            AudioManager.PlayAudio(AudioType.soundFX, closeSound, null, transform.position, null, 1, Random.Range(.9f, 1.1f), 1, 0, 40);
            anim.SetTrigger("Close");
            spriteRenderer.sortingOrder = -3;
        }

        IconManager.UpdateInteractIcon(interactIcon, Vector2.zero, null);
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
        if(!atDoor || playerRbody == null || inputHeld)
        {
            return;
        }

        if(keyTriggersEvent && !triggered)
        {
            AudioManager.PlayAudio(AudioType.soundFX, openSound, null, transform.position, null, 1, Random.Range(.9f, 1.1f), 1, 0, 40);
            anim.SetTrigger("Open");
            spriteRenderer.sortingOrder = -2;

            openEvent.Invoke();
            triggered = true;

            IconManager.UpdateInteractIcon(interactIcon, Vector2.zero, null);
        }

        if (destinationDoor == null)
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
        spawnedobj.transform.position = new Vector3(spawnedobj.transform.position.x, spawnedobj.transform.position.y, 0f);

        Rigidbody2D[] rigids = spawnedobj.GetComponentsInChildren<Rigidbody2D>();

        for(int i = 0; i < rigids.Length; i++)
        {
            rigids[i].transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            rigids[i].AddForce(Random.insideUnitCircle.normalized * 5f, ForceMode2D.Impulse);
        }
    }
}
