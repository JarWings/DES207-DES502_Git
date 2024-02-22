using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.IO;
public enum SliderValue
{
    master, soundFX, music, ui, res
}


[System.Serializable]
public class MenuButton
{
    public string buttonName;
    public string buttonDesc;

    public UnityEvent SelectEvent;
    public UnityEvent LeftEvent;
    public UnityEvent RightEvent;

    public SliderValue sliderValueType;
    public float sliderMaxValue = 100f;
    public bool displaySlider = false;
    public bool playSounds = true;
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
    private int currentPageIndex = -1;

    public GameObject menuButtonPrefab, menuSliderPrefab;
    public GridLayoutGroup buttonParent;

    private List<GameObject> spawnedMenuButtons = new();

    public float pageFlipSpeed = 320f;
    public TMP_Text pageTitleText;
    public TMP_Text buttonDescText;

    public Color buttonColour, headerColour;

    public Sprite highlightSprite;
    private Image highlightImage;

    public RectTransform leftPageParent, rightPageParent;

    [Header("Audio")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip highlightSound;
    public AudioClip activateSound;
    public AudioClip sliderUseSound;
    public AudioClip errorSound;
    public AudioClip pageChangeSound;

    [Header("Video")]
    public TMP_FontAsset defaultButtonFont;
    public TMP_FontAsset simpleButtonFont;

    [HideInInspector] public float sliderValue;

    private Image sliderImage;
    private bool vertHeld = false, hozHeld = false, pageTurning = false;
    private RectTransform leftPageDupe, rightPageDupe;

    private void Start()
    {
        MusicManager.ChangeTrack(menuMusic, true);
        SettingsManager.LoadResolutions(false);

        UpdateMenuFonts();
        UpdateHighlightDisplay();

        ChangePage(0);
    }

    private void Update()
    {
        float vertInput = Input.GetAxisRaw("Vertical");
        float hozInput = Input.GetAxisRaw("Horizontal");

        if (vertInput > .5f)
        {
            UpdateSelection(-1);
            vertHeld = true;
        }
        else if (vertInput < -.5f)
        {
            UpdateSelection(1);
            vertHeld = true;
        }
        else
        {
            vertHeld = false;
        }

        if (hozInput > .5f)
        {
            SliderUse(1);
            hozHeld = true;
        }
        else if (hozInput < -.5f)
        {
            SliderUse(-1);
            hozHeld = true;
        }
        else
        {
            hozHeld = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            ActivateSelection();
        }
    }

    private void MenuHighlightMove(RectTransform targetRect)
    {
        if(targetRect == null || !SettingsManager.data.menuHighlight)
        {
            return;
        }

        if(highlightImage == null)
        {
            CreateHighlightSprite();
        }

        highlightImage.transform.SetParent(targetRect);
        highlightImage.transform.localRotation = Quaternion.Euler(Vector3.zero);

        highlightImage.color = Color.clear;
        highlightImage.transform.localPosition = Vector3.zero;
        highlightImage.rectTransform.sizeDelta = Vector2.zero;
        highlightImage.rectTransform.localScale = Vector2.one;

        highlightImage.rectTransform.anchorMin = Vector2.zero;
        highlightImage.rectTransform.anchorMax = Vector2.one;

        highlightImage.color = Color.yellow;
    }

    void RemoveButtons()
    {
        if(highlightImage != null && highlightImage.transform.parent != null)
        {
            highlightImage.transform.SetParent(null);
        }

        for (int i = 0; i < spawnedMenuButtons.Count; i++)
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
            buttonText.color = buttonColour;

            if ((curButton.SelectEvent == null || curButton.SelectEvent.GetPersistentEventCount() == 0) && (curButton.LeftEvent == null || curButton.LeftEvent.GetPersistentEventCount() == 0) && (curButton.RightEvent == null || curButton.RightEvent.GetPersistentEventCount() == 0)) // Button has no logic, probably being used as a header or "slider" (left/right events)
            {
                buttonText.color = headerColour;
            }

            int index = i;
            bool selected = curPage.currentButtonIndex == index;

            // audio labels
            buttonText.text = buttonText.text.Replace("<mastervol>", ReturnVolumeString(AudioType.master));
            buttonText.text = buttonText.text.Replace("<fxvol>", ReturnVolumeString(AudioType.soundFX));
            buttonText.text = buttonText.text.Replace("<musicvol>", ReturnVolumeString(AudioType.music));
            buttonText.text = buttonText.text.Replace("<uivol>", ReturnVolumeString(AudioType.ui));

            // video labels
            Resolution currentRes = SettingsManager.resolutions[SettingsManager.data.curResIndex];
            buttonText.text = buttonText.text.Replace("<resolution>", (currentRes.width + " x " + currentRes.height + ", " + currentRes.refreshRate + "hz").ToString());
            buttonText.text = buttonText.text.Replace("<fullscreen>", SettingsManager.data.isFullscreen.ToString());
            buttonText.text = buttonText.text.Replace("<vsync>", SettingsManager.data.vsync.ToString());
            buttonText.text = buttonText.text.Replace("<font>", SettingsManager.data.simpleFont.ToString());
            buttonText.text = buttonText.text.Replace("<highlight>", SettingsManager.data.menuHighlight.ToString());

            // gameplay labels
            buttonText.text = buttonText.text.Replace("<difficulty>", SettingsManager.data.difficulty.ToString());

            if (curButton.LeftEvent != null && curButton.LeftEvent.GetPersistentEventCount() > 0)
            {
                buttonText.text = "< " + buttonText.text;
            }

            if (curButton.LeftEvent != null && curButton.RightEvent.GetPersistentEventCount() > 0)
            {
                buttonText.text += " >";
            }

            if (selected)
            {
                if (SettingsManager.data.menuHighlight)
                {
                    if(highlightImage == null)
                    {
                        CreateHighlightSprite();
                    }

                    MenuHighlightMove(tempButton.GetComponent<RectTransform>());
                }

                if (curButton.displaySlider) // This solution sucks and i'll hopefully change it soon...
                {
                    sliderImage = Instantiate(menuSliderPrefab, tempButton.transform).GetComponent<Image>();

                    float sliderVal = 0f;
                    switch (curButton.sliderValueType)
                    {
                        case SliderValue.master:
                            sliderVal = (SettingsManager.data.masterVol + 80f) * 1.25f;
                            break;
                        case SliderValue.soundFX:
                            sliderVal = (SettingsManager.data.fxVol + 80f) * 1.25f;
                            break;
                        case SliderValue.music:
                            sliderVal = (SettingsManager.data.musicVol + 80f) * 1.25f;
                            break;
                        case SliderValue.ui:
                            sliderVal = (SettingsManager.data.uiVol + 80f) * 1.25f;
                            break;
                        case SliderValue.res:
                            sliderVal = SettingsManager.data.curResIndex + 1;
                            curButton.sliderMaxValue = SettingsManager.resolutions.Length;
                            break;
                    }

                    UpdateSlider(sliderVal, curButton.sliderMaxValue);
                }
            }
            else
            {
                buttonText.color /= 1.4f;
                buttonText.fontSize /= 1.6f;
            }

            spawnedMenuButtons.Add(tempButton);
        }
    }

    private void CreateHighlightSprite()
    {
        highlightImage = new GameObject().AddComponent<Image>();
        highlightImage.gameObject.name = "HighlightSprite";

        highlightImage.rectTransform.rotation = Quaternion.Euler(Vector3.zero);

        highlightImage.raycastTarget = false;
        highlightImage.sprite = highlightSprite;
        highlightImage.type = Image.Type.Sliced;
    }

    private string ReturnVolumeString(AudioType type)
    {
        float vol = 0f;

        switch (type)
        {
            case AudioType.master:
                vol = SettingsManager.data.masterVol;
                break;
            case AudioType.soundFX:
                vol = SettingsManager.data.fxVol;
                break;
            case AudioType.music:
                vol = SettingsManager.data.musicVol;
                break;
            case AudioType.ui:
                vol = SettingsManager.data.uiVol;
                break;
        }

        float tempVol = Mathf.RoundToInt((vol + 80f) * 1.25f);
        string volString = tempVol.ToString() + "%";

        return volString;
    }

    private void UpdateSlider(float fillValue, float max = 100f)
    {
        if (sliderImage == null)
        {
            return;
        }

        sliderImage.fillAmount = Mathf.Clamp(fillValue / max, 0f, 1f);
        sliderImage.color = Color.Lerp(Color.red, Color.green, sliderImage.fillAmount / 1f);
    }

    void SliderUse(int index)
    {
        if (hozHeld || pageTurning)
        {
            return;
        }

        MenuPage curPage = Pages[currentPageIndex];
        MenuButton curButton = curPage.menuButtons[curPage.currentButtonIndex];

        if (((curButton.LeftEvent == null || curButton.LeftEvent.GetPersistentEventCount() == 0) && index == -1) || ((curButton.RightEvent == null || curButton.RightEvent.GetPersistentEventCount() == 0) && index == 1))
        {
            return;
        }

        if (index == 1) // right arrow
        {
            curButton.RightEvent.Invoke();
        }
        else if (index == -1) // left arrow
        {
            curButton.LeftEvent.Invoke();
        }

        if (curButton.playSounds)
        {
            AudioManager.PlayAudio(AudioType.ui, sliderUseSound, null, Vector2.zero, null, 1, 1 + (.05f * index), 0);
        }

        SpawnButtons();
    }

    public void PlaySoundFX(AudioClip fx)
    {
        AudioManager.PlayAudio(AudioType.soundFX, fx, null, Vector2.zero, null, 1, 1, 0, 0);
    }

    public void UpdateVolumePositive(int type)
    {
        SettingsManager.UpdateVolume((AudioType)type, 10f, true);
    }

    public void UpdateVolumeNegative(int type)
    {
        SettingsManager.UpdateVolume((AudioType)type, -10f, true);
    }

    public void ChangeResolution(int index)
    {
        SettingsManager.SetResolution(index);
    }

    public void ChangeDifficulty(int index)
    {
        SettingsManager.SetDifficulty(index);
    }

    public void ToggleFullscreen()
    {
        SettingsManager.data.isFullscreen = !SettingsManager.data.isFullscreen;
        SettingsManager.SetResolution(0);
    }

    public void ToggleVsync()
    {
        SettingsManager.data.vsync = !SettingsManager.data.vsync;
        SettingsManager.SetResolution(0);
    }

    public void SaveSettings()
    {
        SettingsManager.SaveSettings();
    }

    public void ResetSettings()
    {
        string path = Application.persistentDataPath + "/gamesettings.json";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        SettingsManager.data = new();
        SettingsManager.LoadSettings();

        UpdateMenuFonts();
        UpdateHighlightDisplay();
        SpawnButtons();
    }

    public void ToggleSimpleFont()
    {
        SettingsManager.data.simpleFont = !SettingsManager.data.simpleFont;
        UpdateMenuFonts();
    }

    private void UpdateMenuFonts()
    {
        menuButtonPrefab.GetComponent<TMP_Text>().font = defaultButtonFont;
        pageTitleText.font = defaultButtonFont;
        buttonDescText.font = defaultButtonFont;

        if (SettingsManager.data.simpleFont)
        {
            menuButtonPrefab.GetComponent<TMP_Text>().font = simpleButtonFont;
            pageTitleText.font = simpleButtonFont;
            buttonDescText.font = simpleButtonFont;
        }
    }

    public void ToggleHighlight()
    {
        SettingsManager.data.menuHighlight = !SettingsManager.data.menuHighlight;

        UpdateHighlightDisplay();
    }

    private void UpdateHighlightDisplay()
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = SettingsManager.data.menuHighlight;
        }
    }

    void UpdateSelection(int index)
    {
        if (vertHeld)
        {
            return;
        }

        MenuPage curPage = Pages[currentPageIndex];

        curPage.currentButtonIndex += index;

        if (curPage.currentButtonIndex > curPage.menuButtons.Count - 1)
        {
            curPage.currentButtonIndex = 0;
        }
        else if (curPage.currentButtonIndex < 0)
        {
            curPage.currentButtonIndex = curPage.menuButtons.Count - 1;
        }

        buttonDescText.text = curPage.menuButtons[curPage.currentButtonIndex].buttonDesc;

        // Highlight sound pitches up as you move down the menu
        AudioManager.PlayAudio(AudioType.ui, highlightSound, null, Vector2.zero, null, 1, 1 + (.05f * curPage.currentButtonIndex), 0);

        SpawnButtons();
    }

    public void DisplayScores()
    {
        ChangePage(7);

        List<MenuButton> scoreButtons = Pages[7].menuButtons;
        scoreButtons.Clear();

        LeaderboardManager.LoadScores();

        LeaderboardManager leaderboardObj = LeaderboardManager.GetLeaderboardObj();

        for (int i = 0; i < leaderboardObj.scoreList.scores.Count; i++)
        {
            Score currentScore = leaderboardObj.scoreList.scores[i];

            MenuButton newScore = new();
            newScore.buttonName = currentScore.playerName + "   -  " + TimeSpan.FromSeconds(currentScore.time).ToString(@"hh\:mm\:ss\:ff");
            newScore.buttonDesc = currentScore.playDate;

            scoreButtons.Insert(0, newScore);
        }

        scoreButtons.Add(Pages[1].menuButtons[Pages[1].menuButtons.Count - 1]);

        SpawnButtons();
        buttonDescText.text = Pages[7].menuButtons[Pages[7].currentButtonIndex].buttonDesc;
    }
    public void DisplayJournalEntries()
    {
        ChangePage(8);

        List<MenuButton> entryButtons = Pages[8].menuButtons;
        entryButtons.Clear();

        JournalManager.LoadEntries();

        JournalManager journalObj = JournalManager.GetJournalObj();

        for (int i = 0; i < journalObj.EntryContainer.Entries.Count; i++)
        {
            JournalEntry currentEntry = journalObj.EntryContainer.Entries[i];
            MenuButton newEntry = new();

            string buttonName =  new string('?', currentEntry.Title.Length);
            string buttonDesc = new string('?', currentEntry.Description.Length);

            if (currentEntry.Owned)
            {
                buttonName = currentEntry.Title;
                buttonDesc = currentEntry.Description;
            }

            newEntry.buttonName = buttonName;
            newEntry.buttonDesc = buttonDesc;

            entryButtons.Insert(0, newEntry);
        }

        entryButtons.Add(Pages[1].menuButtons[Pages[1].menuButtons.Count - 1]);

        SpawnButtons();
        buttonDescText.text = Pages[8].menuButtons[Pages[8].currentButtonIndex].buttonDesc;
    }

    public void ChangePage(int page)
    {
        if (page > Pages.Count || page < 0)
        {
            return;
        }

        if (leftPageDupe != null)
        {
            Destroy(leftPageDupe.gameObject);
        }
        if (rightPageDupe != null)
        {
            Destroy(rightPageDupe.gameObject);
        }

        if (highlightImage != null && highlightImage.transform.parent != null)
        {
            highlightImage.transform.SetParent(null);
        }

        StopAllCoroutines();
        StartCoroutine(PageAnimation(page));

        AudioManager.PlayAudio(AudioType.soundFX, pageChangeSound, null, Vector2.zero, null, UnityEngine.Random.Range(.9f, 1.1f), UnityEngine.Random.Range(.9f, 1.1f), 0);

        currentPageIndex = page;
        SpawnButtons();

        MenuPage curPage = Pages[currentPageIndex];

        pageTitleText.text = curPage.MenuName;
        buttonDescText.text = curPage.menuButtons[curPage.currentButtonIndex].buttonDesc;
    }

    IEnumerator PageAnimation(int page)
    {
        pageTurning = true;

        if (page > currentPageIndex) // turn page right
        {
            leftPageDupe = Instantiate(leftPageParent, leftPageParent.parent);
            leftPageDupe.transform.SetAsFirstSibling();

            leftPageParent.localRotation = Quaternion.Euler(0f, -90f, 0f);
            rightPageDupe = Instantiate(rightPageParent, rightPageParent.parent);
            while (rightPageDupe.localRotation != Quaternion.Euler(0, -90f, 0) || leftPageDupe.localRotation != Quaternion.Euler(0f, 0f, 0f))
            {
                rightPageDupe.localRotation = Quaternion.Euler(0f, Mathf.MoveTowardsAngle(rightPageDupe.localRotation.eulerAngles.y, -90f, Time.deltaTime * pageFlipSpeed), 0f);
                leftPageDupe.localRotation = Quaternion.Euler(0f, Mathf.MoveTowardsAngle(leftPageDupe.localRotation.eulerAngles.y, 0f, Time.deltaTime * pageFlipSpeed), 0f);

                yield return new WaitForEndOfFrame();
            }
            Destroy(rightPageDupe.gameObject);


            while (leftPageParent.localRotation != Quaternion.Euler(0f, 0f, 0f) || rightPageParent.localRotation != Quaternion.Euler(0f, 0f, 0f))
            {
                leftPageParent.localRotation = Quaternion.Euler(0f, Mathf.MoveTowardsAngle(leftPageParent.localRotation.eulerAngles.y, 0f, Time.deltaTime * pageFlipSpeed), 0f);
                rightPageParent.localRotation = Quaternion.Euler(0f, Mathf.MoveTowardsAngle(rightPageParent.localRotation.eulerAngles.y, 0f, Time.deltaTime * pageFlipSpeed), 0f);
                yield return new WaitForEndOfFrame();
            }
            Destroy(leftPageDupe.gameObject);
        }
        else // turn page left
        {
            rightPageDupe = Instantiate(rightPageParent, rightPageParent.parent);
            rightPageDupe.transform.SetAsFirstSibling();

            rightPageParent.localRotation = Quaternion.Euler(0f, -90f, 0f);

            leftPageDupe = Instantiate(leftPageParent, leftPageParent.parent);
            while (leftPageDupe.localRotation != Quaternion.Euler(0f, -90f, 0f) || rightPageDupe.localRotation != Quaternion.Euler(0f, 0f, 0f))
            {
                leftPageDupe.localRotation = Quaternion.Euler(0f, Mathf.MoveTowardsAngle(leftPageDupe.localRotation.eulerAngles.y, -90f, Time.deltaTime * pageFlipSpeed), 0f);
                rightPageDupe.localRotation = Quaternion.Euler(0f, Mathf.MoveTowardsAngle(rightPageDupe.localRotation.eulerAngles.y, 0f, Time.deltaTime * pageFlipSpeed), 0f);

                yield return new WaitForEndOfFrame();
            }
            Destroy(leftPageDupe.gameObject);

            while (rightPageParent.localRotation != Quaternion.Euler(0f, 0f, 0f) || leftPageParent.localRotation != Quaternion.Euler(0f, 0f, 0f))
            {
                rightPageParent.localRotation = Quaternion.Euler(0f, Mathf.MoveTowardsAngle(rightPageParent.localRotation.eulerAngles.y, 0f, Time.deltaTime * pageFlipSpeed), 0f);
                leftPageParent.localRotation = Quaternion.Euler(0f, Mathf.MoveTowardsAngle(leftPageParent.localRotation.eulerAngles.y, 0f, Time.deltaTime * pageFlipSpeed), 0f);
                yield return new WaitForEndOfFrame();
            }
            Destroy(rightPageDupe.gameObject);
        }

        MenuHighlightMove(spawnedMenuButtons[Pages[currentPageIndex].currentButtonIndex].GetComponent<RectTransform>());
        pageTurning = false;
    }

    void ActivateSelection()
    {
        if (pageTurning)
        {
            return;
        }

        MenuPage curMenu = Pages[currentPageIndex];
        MenuButton button = curMenu.menuButtons[curMenu.currentButtonIndex];

        AudioClip tempButtonSound = errorSound;
        if (button.SelectEvent != null && button.SelectEvent.GetPersistentEventCount() > 0)
        {
            button.SelectEvent.Invoke();
            tempButtonSound = activateSound;
        }
        if (button.playSounds)
        {
            AudioManager.PlayAudio(AudioType.ui, tempButtonSound, null, Vector2.zero, null, 1, 1, 0);
        }
    }

    public void LoadScene(string scene)
    {
        MusicManager.ChangeTrack(levelMusic, true);
        SceneChangeManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DeleteAllData()
    {
        string path = Application.persistentDataPath;

        if(File.Exists(path + "/journal.json"))
        {
            File.Delete(path + "/journal.json");
        }

        if (File.Exists(path + "/scores.json"))
        {
            File.Delete(path + "/scores.json");
        }

        ChangePage(0);
    }
}