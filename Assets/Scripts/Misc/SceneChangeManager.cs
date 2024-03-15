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


    /// <summary>
    /// Will probably support scene transitions in the future, as well as handling music changes.
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        if(SceneManager.GetActiveScene().name == sceneName)
        {
            return;
        }

        PlayerCharacter.ResetPosition();

        if(PlayerCharacter.Instance != null)
        {
            PlayerCharacter.Instance.hp = PlayerCharacter.Instance.maxHp;
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }
}
