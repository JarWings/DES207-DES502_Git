using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    // AUDIO
    [Range(0f, 100f)] public static float masterVol = 0f, musicVol = 0f, fxVol = 0f, uiVol = 0f;
    public static float minVol = -80f, maxVol = 0f;

    // VIDEO
    public static int curResIndex = 0;
    public static Resolution[] resolutions;
    public static bool vsync = true;
    public static bool isFullscreen = true;

    public static bool simpleFont = false;
    public static bool menuHighlight = false;

    private void Update()
    {
        if (!Application.isEditor)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneChangeManager.LoadScene("MainMenu");
        }
    }

    public static void LoadResolutions()
    {
        resolutions = Screen.resolutions;

        for(int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                curResIndex = i;
            }
        }
    }

    public static void SetResolution(int index)
    {
        curResIndex += index;

        if(curResIndex > resolutions.Length - 1)
        {
            curResIndex = 0;
        }
        else if (curResIndex < 0)
        {
            curResIndex = resolutions.Length - 1;
        }

        Screen.SetResolution(resolutions[curResIndex].width, resolutions[curResIndex].height, isFullscreen, resolutions[curResIndex].refreshRate);

        int vsyncCount = 0;
        if (vsync)
        {
            vsyncCount = 1;
        }

        QualitySettings.vSyncCount = vsyncCount;
    }

    public static void UpdateVolume(AudioType type, float volume, bool addition)
    {
        AudioMixer mix = GameObject.FindWithTag("Global").GetComponent<AudioManager>().mainAudioMix;
        switch (type)
        {
            case AudioType.master:
                masterVol = AdditionCheck(masterVol, volume, addition);
                mix.SetFloat("Master", masterVol);
                break;
            case AudioType.soundFX:
                fxVol = AdditionCheck(fxVol, volume, addition);
                mix.SetFloat("Sound FX", fxVol);
                break;
            case AudioType.music:
                musicVol = AdditionCheck(musicVol, volume, addition);
                mix.SetFloat("Music", musicVol);
                break;
            case AudioType.ui:
                uiVol = AdditionCheck(uiVol, volume, addition);
                mix.SetFloat("Ui", uiVol);
                break;
        }
    }

    private static float AdditionCheck(float vol, float addition, bool isAdd)
    {
        float tempVol = vol;
        if (isAdd)
        {
            tempVol += addition;
        }

        if(tempVol < minVol)
        {
            tempVol = maxVol;
        }
        else if (tempVol > maxVol)
        {
            tempVol = minVol;
        }

        return tempVol;
    }
}
