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
        SceneManager.LoadScene(sceneName);
    }
}
