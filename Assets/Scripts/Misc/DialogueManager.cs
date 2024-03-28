using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class Line
{
    public string senderName = "";
    [TextArea(4, 20)]
    public string message = "";
    public AudioClip voiceClip;
    public Sprite talkSprite, recieveSprite;
    public bool isPlayer = false;
    public UnityEvent talkEvent;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public int lineIndex = 0;
    public float characterDelay = .06f;
    public float openSpeed = 800f;

    private List<Line> currentDialogueLines = new();

    public static bool inDialogue = false;
    private int currentPriority;

    private AudioSource dialogueSoundSource;

    [Header("Ui")]
    public RectTransform dialogueBox;
    public TMP_Text nameText;
    public TMP_Text lineText;

    public Image fadePanel;
    public Image playerAvatar, npcAvatar;


    private void Awake()
    {
        Instance = this;

        dialogueBox.anchoredPosition = new Vector2(0, -180f);
        fadePanel.color = Color.clear;
        playerAvatar.color = Color.clear;
        npcAvatar.color = Color.clear;
    }

    private void Update()
    {
        if (!inDialogue) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Line currentLine = currentDialogueLines[lineIndex];
            if (lineText.text != currentLine.message)
            {
                StopAllCoroutines();

                nameText.text = currentLine.senderName;
                lineText.text = currentLine.message;

                fadePanel.color = Color.black / 1.4f;

                if (currentLine.isPlayer)
                {
                    playerAvatar.color = currentLine.talkSprite != null ? Color.white : Color.clear;
                    npcAvatar.color = npcAvatar.sprite != null ? Color.grey : Color.clear;
                }
                else
                {
                    npcAvatar.color = npcAvatar.sprite != null ? Color.white : Color.clear;
                    playerAvatar.color = playerAvatar.sprite != null ? Color.grey : Color.clear;
                }

                dialogueBox.anchoredPosition = new Vector2(0, 150f);
                return;
            }

            lineIndex++;
            DisplayLine();
        }
    }

    public static void DisplayDialogue(List<Line> lines, int priority)
    {
        if ((inDialogue && priority < Instance.currentPriority) || lines == Instance.currentDialogueLines) return;

        Instance.playerAvatar.sprite = null;
        Instance.npcAvatar.sprite = null;

        Instance.lineIndex = 0;
        Instance.currentDialogueLines = lines;
        Instance.DisplayLine();
        inDialogue = true;

        Time.timeScale = .0001f;
    }

    private void DisplayLine()
    {
        if (lineIndex < 0 || lineIndex >= currentDialogueLines.Count || currentDialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (dialogueSoundSource != null) Destroy(dialogueSoundSource.gameObject);

        Line currentLine = currentDialogueLines[lineIndex];

        if (currentLine.talkEvent != null && currentLine.talkEvent.GetPersistentEventCount() > 0) currentLine.talkEvent.Invoke();

        if (currentLine.isPlayer) currentLine.senderName = "Stuart Mitchell";

        StopAllCoroutines();

        nameText.text = currentLine.senderName;
        StartCoroutine(FillText(lineText, currentLine.message));

        StartCoroutine(ToggleDialogueUi(true));
        StartCoroutine(ImageFade(fadePanel, Color.black / 1.4f, 2f));

        if (currentLine.isPlayer)
        {
            if (currentLine.talkSprite != null)
            {
                playerAvatar.rectTransform.anchoredPosition = new Vector2(500f, 40f);
                playerAvatar.sprite = currentLine.talkSprite;
                StartCoroutine(ImageFade(playerAvatar, Color.white, 4f));
            }

            if (npcAvatar.sprite != null)
            {
                npcAvatar.rectTransform.anchoredPosition = new Vector2(-500f, 20f);
                StartCoroutine(ImageFade(npcAvatar, Color.grey, 4f));
            }
        }
        else
        {
            if (currentLine.talkSprite != null)
            {
                npcAvatar.rectTransform.anchoredPosition = new Vector2(-500f, 40f);
                npcAvatar.sprite = currentLine.talkSprite;
                StartCoroutine(ImageFade(npcAvatar, Color.white, 4f));
            }

            if (currentLine.recieveSprite != null)
            {
                playerAvatar.sprite = currentLine.recieveSprite;
            }

            if (playerAvatar.sprite != null)
            {
                playerAvatar.rectTransform.anchoredPosition = new Vector2(500f, 20f);
                StartCoroutine(ImageFade(playerAvatar, Color.grey, 4f));
            }
        }

        dialogueSoundSource = AudioManager.PlayAudio(AudioType.soundFX, currentLine.voiceClip, null, Vector2.zero, null, 1, 1, 0);
    }

    private void EndDialogue()
    {
        StopAllCoroutines();

        StartCoroutine(ToggleDialogueUi(false));

        StartCoroutine(ImageFade(fadePanel, Color.clear, 2f));

        StartCoroutine(ImageFade(playerAvatar, Color.clear, 12f));
        StartCoroutine(ImageFade(npcAvatar, Color.clear, 12f));

        nameText.text = "";
        lineText.text = "";

        if (dialogueSoundSource != null) Destroy(dialogueSoundSource.gameObject);

        currentDialogueLines = null;
        Time.timeScale = 1f;

        inDialogue = false;
    }

    IEnumerator ImageFade(Image img, Color colour, float fadeSpeed)
    {
        while (img.color != colour)
        {
            img.color = Color.Lerp(img.color, colour, Time.unscaledDeltaTime * fadeSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FillText(TMP_Text textRef, string text)
    {
        textRef.text = "";
        char[] characters = text.ToCharArray();

        for (int i = 0; i < characters.Length; i++)
        {
            textRef.text += characters[i];
            yield return new WaitForSecondsRealtime(characterDelay);
        }
    }

    IEnumerator ToggleDialogueUi(bool open)
    {
        float posY = open ? 150f : -180f;
        while (dialogueBox.anchoredPosition != new Vector2(0, posY))
        {
            dialogueBox.anchoredPosition = new Vector2(0, Mathf.MoveTowards(dialogueBox.anchoredPosition.y, posY, openSpeed * Time.unscaledDeltaTime));
            yield return new WaitForEndOfFrame();
        }
    }
}
