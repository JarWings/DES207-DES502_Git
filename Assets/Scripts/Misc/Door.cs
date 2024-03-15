using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Door : MonoBehaviour
{
    public Sprite liftSprite;
    public Sprite interactSprite;
    public Door destinationDoor;
    public Animator anim;
    public bool oneTimeEvent = true, inputHeld = false, keyTriggersEvent = false, transition = false;
    public float transitionSpeed = 10f;
    public UnityEvent openEvent;
    private bool triggered = false, atDoor = false;
    public Rigidbody2D playerRbody;

    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip transitionSound;

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

        MoveEnemies();

        if (transition)
        {
            StartCoroutine(Transition(playerRbody));
        }
        else
        {
            playerRbody.position = destinationDoor.transform.position;
        }

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

    private void MoveEnemies()
    {
        if(destinationDoor == null)
        {
            return;
        }

        Vector2 exitPos = destinationDoor.transform.position;
        RaycastHit2D[] enemies = Physics2D.CircleCastAll(exitPos, 5f, Vector2.up, 5f, LayerMask.GetMask("Enemy"));

        for(int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i].transform.GetComponent<Enemy>();
            enemy.ResetPosition();
        }
    }

    IEnumerator Transition(Rigidbody2D playerRbody)
    {
        AudioSource transitionSource = AudioManager.PlayAudio(AudioType.soundFX, transitionSound, null, Vector2.zero, null, 1, 1, 0, 0, 2600f, true);

        SpriteRenderer playerSprite = playerRbody.GetComponent<SpriteRenderer>();
        Collider2D playerCol = playerRbody.GetComponent<Collider2D>();

        GameObject elevatorObj = new();
        SpriteRenderer elevatorSprite = elevatorObj.AddComponent<SpriteRenderer>();
        elevatorSprite.sprite = liftSprite;
        elevatorSprite.sortingOrder = -100;
        elevatorObj.transform.position = playerRbody.position;
        elevatorObj.transform.localScale = Vector3.one * .8f;

        playerCol.enabled = false;
        playerSprite.enabled = false;
        playerRbody.isKinematic = true;

        playerRbody.constraints = RigidbodyConstraints2D.FreezePosition;

        while (playerRbody.position != (Vector2)destinationDoor.transform.position)
        {
            Vector2 movePos = Vector2.MoveTowards(playerRbody.position, destinationDoor.transform.position, Time.deltaTime * transitionSpeed);
            elevatorObj.transform.position = movePos;
            playerRbody.position = movePos;
            yield return new WaitForFixedUpdate();
        }

        Destroy(elevatorObj);

        playerCol.enabled = true;
        playerSprite.enabled = true;
        playerRbody.isKinematic = false;

        playerRbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (transitionSource != null)
        {
            Destroy(transitionSource.gameObject);
        }
    }
}
