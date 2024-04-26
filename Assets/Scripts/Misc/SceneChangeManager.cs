using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static Transform globalManager;

    private void Awake()
    {
        AliveBetweenScenes();
    }

    void AliveBetweenScenes()
    {
        if(globalManager == null)
        {
            DontDestroyOnLoad(gameObject);
            globalManager = transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadScene(string sceneName, bool resetPlayer = true)
    {
        if(SceneManager.GetActiveScene().name == sceneName) return; // prevents loading the active scene

        PlayerCharacter.ResetPosition();
		
		bool isMenu = sceneName == "MainMenu";
		LeaderboardManager.allowTimer = !isMenu;

        if (GameOverManager.Instance != null) GameOverManager.isOver = false;

        if (DialogueManager.Instance != null) DialogueManager.Instance.EndDialogue();

        if (PlayerCharacter.Instance != null && resetPlayer) // resets player on scene changes
        {
			PlayerCharacter.Instance.enabled = true;

            PlayerCharacter.Instance.hp = PlayerCharacter.Instance.maxHp;
            PlayerCharacter.Instance.transform.GetComponent<Collider2D>().enabled = true;
            PlayerCharacter.Instance.transform.GetComponent<SpriteRenderer>().enabled = true;
            PlayerCharacter.Instance.transform.GetComponent<PlayerController>().enabled = (sceneName != "MainMenu");

            PlayerCharacter.Instance.dashFrames.Clear();

            Rigidbody2D rbody = PlayerCharacter.Instance.transform.GetComponent<Rigidbody2D>();
			
            rbody.constraints = isMenu ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;
            rbody.velocity = isMenu ? Vector2.zero : rbody.velocity;
            rbody.isKinematic = isMenu;
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }
}
