using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class MenuButton
{
    public string buttonName;
    public string buttonDesc;

    public UnityEvent SelectEvent;
    public UnityEvent LeftEvent;
    public UnityEvent RightEvent;
}

[System.Serializable]
public class MenuPage
{
    public string MenuName;
    public List<MenuButton> menuButtons = new();

    [HideInInspector] public int currentButtonIndex = 0;
}


public class MainMenuManager : MonoBehaviour
{
    public List<MenuPage> Pages = new();
    private int currentPageIndex = 0;

    public GameObject menuButtonPrefab;
    public GridLayoutGroup buttonParent;

    private List<GameObject> spawnedMenuButtons = new();

    public TMP_Text pageTitleText;
    public TMP_Text buttonDescText;

    [Header("Audio")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip highlightSound;
    public AudioClip activateSound;
    public AudioClip sliderUseSound;

    [Header("Video")]
    public TMP_FontAsset defaultButtonFont;
    public TMP_FontAsset simpleButtonFont;

    private void Start()
    {
        MusicManager.ChangeTrack(menuMusic, true);
        SettingsManager.LoadResolutions();

        ChangePage(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpdateSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpdateSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SliderUse(-1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SliderUse(1);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ActivateSelection();
        }
    }

    void RemoveButtons()
    {
        for(int i = 0; i < spawnedMenuButtons.Count; i++)
        {
            Destroy(spawnedMenuButtons[i]);
        }

        spawnedMenuButtons.Clear();
    }

    void SpawnButtons()
    {
        RemoveButtons();

        MenuPage curPage = Pages[currentPageIndex];

        for (int i = 0; i < curPage.menuButtons.Count; i++)
        {
            MenuButton curButton = curPage.menuButtons[i];

            GameObject tempButton = Instantiate(menuButtonPrefab, buttonParent.transform);
            TMP_Text buttonText = tempButton.GetComponent<TMP_Text>();

            buttonText.text = curButton.buttonName;

            if (curButton.SelectEvent.GetPersistentEventCount() == 0) // Button has no logic, probably being used as a header or "slider" (left/right events)
            {
                buttonText.color = Color.white;
            }

            // audio labels
            buttonText.text = buttonText.text.Replace("<mastervol>", ReturnVolumeString(AudioType.master));
            buttonText.text = buttonText.text.Replace("<fxvol>", ReturnVolumeString(AudioType.soundFX));
            buttonText.text = buttonText.text.Replace("<musicvol>", ReturnVolumeString(AudioType.music));
            buttonText.text = buttonText.text.Replace("<uivol>", ReturnVolumeString(AudioType.ui));

            // video labels
            buttonText.text = buttonText.text.Replace("<resolution>", SettingsManager.resolutions[SettingsManager.curResIndex].ToString());
            buttonText.text = buttonText.text.Replace("<fullscreen>", SettingsManager.isFullscreen.ToString());
            buttonText.text = buttonText.text.Replace("<vsync>", SettingsManager.vsync.ToString());
            buttonText.text = buttonText.text.Replace("<font>", SettingsManager.simpleFont.ToString());

            if (curButton.LeftEvent.GetPersistentEventCount() > 0)
            {
                buttonText.text = "< " + buttonText.text;
            }

            if (curButton.RightEvent.GetPersistentEventCount() > 0)
            {
                buttonText.text = buttonText.text + " >";
            }

            if (curPage.currentButtonIndex != i)
            {
                buttonText.color /= 1.6f;
                buttonText.fontSize /= 1.4f;
            }

            spawnedMenuButtons.Add(tempButton);
        }
    }

    private string ReturnVolumeString(AudioType type)
    {
        string volString;
        float vol = 0f;

        switch (type)
        {
            case AudioType.master:
                vol = SettingsManager.masterVol;
                break;
            case AudioType.soundFX:
                vol = SettingsManager.fxVol;
                break;
            case AudioType.music:
                vol = SettingsManager.musicVol;
                break;
            case AudioType.ui:
                vol = SettingsManager.uiVol;
                break;
        }

        volString = Mathf.RoundToInt((vol + 80f) * 1.25f).ToString() + "%";

        return volString;
    }

    void SliderUse(int index)
    {
        MenuPage curPage = Pages[currentPageIndex];
        MenuButton curButton = curPage.menuButtons[curPage.currentButtonIndex];

        if ((curButton.LeftEvent.GetPersistentEventCount() == 0 && index == -1) || (curButton.RightEvent.GetPersistentEventCount() == 0 && index == 1))
        {
            return; 
        }

        if(index == 1) // right arrow
        {
            curButton.RightEvent.Invoke();
        }
        else if (index == -1) // left arrow
        {
            curButton.LeftEvent.Invoke();
        }

        AudioManager.PlayAudio(AudioType.ui, sliderUseSound, null, Vector2.zero, null, 1, 1 + (.05f * index), 0);

        SpawnButtons();
    }

    public void UpdateVolumePositive(int type)
    {
        SettingsManager.UpdateVolume((AudioType)type, 10f, true);
    }

    public void UpdateVolumeNegative(int type)
    {
        SettingsManager.UpdateVolume((AudioType)type, - 10f, true);
    }

    public void ChangeResolution(int index)
    {
        SettingsManager.SetResolution(index);
    }

    public void ToggleFullscreen()
    {
        SettingsManager.isFullscreen = !SettingsManager.isFullscreen;
        SettingsManager.SetResolution(0);
    }

    public void ToggleVsync()
    {
        SettingsManager.vsync = !SettingsManager.vsync;
        SettingsManager.SetResolution(0);
    }

    public void ToggleSimpleFont()
    {
        SettingsManager.simpleFont = !SettingsManager.simpleFont;

        menuButtonPrefab.GetComponent<TMP_Text>().font = defaultButtonFont;
        pageTitleText.font = defaultButtonFont;
        buttonDescText.font = defaultButtonFont;

        if (SettingsManager.simpleFont)
        {
            menuButtonPrefab.GetComponent<TMP_Text>().font = simpleButtonFont;
            pageTitleText.font = simpleButtonFont;
            buttonDescText.font = simpleButtonFont;
        }
    }

    void UpdateSelection(int index)
    {
        MenuPage curPage = Pages[currentPageIndex];

        curPage.currentButtonIndex += index;

        if(curPage.currentButtonIndex > curPage.menuButtons.Count - 1)
        {
            curPage.currentButtonIndex = 0;
        }
        else if(curPage.currentButtonIndex < 0)
        {
            curPage.currentButtonIndex = curPage.menuButtons.Count - 1;
        }

        buttonDescText.text = curPage.menuButtons[curPage.currentButtonIndex].buttonDesc;

        // Highlight sound pitches up as you move down the menu
        AudioManager.PlayAudio(AudioType.ui, highlightSound, null, Vector2.zero, null, 1, 1 + (.05f * curPage.currentButtonIndex), 0);

        SpawnButtons();
    }

    public void ChangePage(int page)
    {
        if(page > Pages.Count || page < 0)
        {
            return;
        }

        currentPageIndex = page;
        SpawnButtons();

        MenuPage curPage = Pages[currentPageIndex];

        pageTitleText.text = curPage.MenuName;
        buttonDescText.text = curPage.menuButtons[curPage.currentButtonIndex].buttonDesc;
    }

    void ActivateSelection()
    {
        MenuPage curMenu = Pages[currentPageIndex];
        MenuButton button = curMenu.menuButtons[curMenu.currentButtonIndex];

        button.SelectEvent.Invoke();
        AudioManager.PlayAudio(AudioType.ui, activateSound, null, Vector2.zero, null, 1, 1, 0);
    }

    public void LoadScene(string scene) 
    {
        MusicManager.ChangeTrack(levelMusic, true, .3f);
        SceneChangeManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}