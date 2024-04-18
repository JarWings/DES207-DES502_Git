using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public enum DifficultySettings { easy, medium, hard, insane }

[System.Serializable]
public class SettingsData
{
    // AUDIO
    [Range(0f, 100f)] public float masterVol = 0f, musicVol = 0f, fxVol = 0f, uiVol = 0f;

    // VIDEO
    public int curResIndex = 0;
    public bool vsync = true;
    public bool isFullscreen = true;

    public bool simpleFont = false;
    public bool menuHighlight = false;
    public int curLanguage = 0;

    // GAMEPLAY
    public DifficultySettings difficulty = DifficultySettings.easy;
}

public class SettingsManager : MonoBehaviour
{
    public static string[] Languages = new string[] {"English"};
    public static SettingsData data = new();
    public static Resolution[] resolutions;
    public static float minVol = -80f, maxVol = 0f;

    public static bool firstBoot = true;

    private void Update()
    {
        if (Input.GetButtonDown("Pause")) SceneChangeManager.LoadScene("MainMenu");

        if (Input.GetKey(KeyCode.Alpha6) && Input.GetKey(KeyCode.Alpha9))
        {
            DestructionCheat();
        }
		
		if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.D))
        {
            GodCheat();
        }
    }

    private void Start()
    {
        LoadSettings();
    }

    public static void LoadResolutions(bool selectCurrentRes)
    {
        resolutions = Screen.resolutions;

        if (!selectCurrentRes) return;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                data.curResIndex = i;
            }
        }
    }

    public static void SetResolution(int index)
    {
        if (resolutions == null || resolutions.Length <= 0) LoadResolutions(false);

        data.curResIndex += index;

        if (data.curResIndex > resolutions.Length - 1)
        {
            data.curResIndex = 0;
        }
        else if (data.curResIndex < 0)
        {
            data.curResIndex = resolutions.Length - 1;
        }

        Screen.SetResolution(resolutions[data.curResIndex].width, resolutions[data.curResIndex].height, data.isFullscreen, resolutions[data.curResIndex].refreshRate);
        int vsyncCount = data.vsync ? 1 : 0;
        QualitySettings.vSyncCount = vsyncCount;
    }

    public static void SetDifficulty(int index)
    {
        int curDifficulty = (int)data.difficulty;
		curDifficulty = (curDifficulty + 1) % 3;
        data.difficulty = (DifficultySettings)curDifficulty;
    }

    public static void UpdateVolume(AudioType type, float volume, bool addition)
    {
        AudioMixer mix = GameObject.FindWithTag("Global").GetComponent<AudioManager>().mainAudioMix;
        switch (type)
        {
            case AudioType.master:
                data.masterVol = AdditionCheck(data.masterVol, volume, addition);
                mix.SetFloat("Master", data.masterVol);
                break;
            case AudioType.soundFX:
                data.fxVol = AdditionCheck(data.fxVol, volume, addition);
                mix.SetFloat("Sound FX", data.fxVol);
                break;
            case AudioType.music:
                data.musicVol = AdditionCheck(data.musicVol, volume, addition);
                mix.SetFloat("Music", data.musicVol);
                break;
            case AudioType.ui:
                data.uiVol = AdditionCheck(data.uiVol, volume, addition);
                mix.SetFloat("Ui", data.uiVol);
                break;
        }
    }

    private static float AdditionCheck(float vol, float addition, bool isAdd)
    {
        float tempVol = vol;
        if (isAdd) tempVol += addition;

        tempVol = (tempVol < minVol) ? maxVol : (tempVol > maxVol) ? minVol : tempVol;

        return tempVol;
    }

    public static void SaveSettings()
    {
        string path = Application.persistentDataPath + "/gamesettings.json";

        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, jsonData);
    }

    public static void LoadSettings()
    {
        string path = Application.persistentDataPath + "/gamesettings.json";

        if (!File.Exists(path))
        {
            LoadResolutions(true);
            UpdateSettings();
            return;
        }

        string jsonData = File.ReadAllText(path);
        data = JsonUtility.FromJson<SettingsData>(jsonData);

        UpdateSettings();
    }

    public static void UpdateSettings()
    {
        SetResolution(0);

        UpdateVolume(AudioType.master, 0, true);
        UpdateVolume(AudioType.soundFX, 0, true);
        UpdateVolume(AudioType.music, 0, true);
        UpdateVolume(AudioType.ui, 0, true);
    }
	
    private void DestructionCheat()
    {
        GameObject[] objs = FindObjectsOfType<GameObject>();

        for (int i = 0; i < objs.Length; i++)
        {
            GameObject obj = objs[i];
            if (!obj.GetComponent<Collider2D>() && obj.gameObject.tag == "Untagged" && obj.GetComponent<SpriteRenderer>() && !obj.GetComponent<DestructableObject>())
            {
                obj.tag = "Destructible";
                obj.layer = 11;

                obj.AddComponent<BoxCollider2D>();

                DestructableObject destructable = obj.AddComponent<DestructableObject>();
                destructable.health = 50;
            }
        }
    }
	
	private void GodCheat()
	{
		if(PlayerCharacter.Instance == null) return;
		
		PlayerCharacter.Instance.hp = 99999;
		PlayerCharacter.Instance.maxHp = 99999;
	}
}