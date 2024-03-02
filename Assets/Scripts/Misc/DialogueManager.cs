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
    public Sprite talkSprite;
    public bool isPlayer = false;
    public UnityEvent talkEvent;
}

public class DialogueManager : MonoBehaviour
{
    public int lineIndex = 0;
    public float characterDelay = .06f;
    public float openSpeed = 800f;

    private List<Line> currentDialogueLines = new();

    private bool inDialogue = false;
    private int currentPriority;

    private AudioSource dialogueSoundSource;

    [Header("Ui")]
    public RectTransform dialogueBox;
    public TMP_Text nameText;
    public TMP_Text lineText;

    public Image fadePanel;
    public Image playerAvatar, npcAvatar;

    private void Start()
    {
        dialogueBox.anchoredPosition = new Vector2(0, -180f);
        fadePanel.color = Color.clear;
        playerAvatar.color = Color.clear;
        npcAvatar.color = Color.clear;
    }

    private void Update()
    {
        if (!inDialogue)
        {
            return;
        }

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
                    if(currentLine.talkSprite != null)
                    {
                        playerAvatar.color = Color.white;
                    }

                    if(npcAvatar.sprite != null)
                    {
                        npcAvatar.color = Color.grey;
                    }
                }
                else
                {
                    if (currentLine.talkSprite != null)
                    {
                        playerAvatar.color = Color.grey;
                    }

                    if (npcAvatar.sprite != null)
                    {
                        npcAvatar.color = Color.white;
                    }
                }

                dialogueBox.anchoredPosition = new Vector2(0, 150f);
                return;
            }

            lineIndex++;
            DisplayLine();
        }
    }

    public void DisplayDialogue(List<Line> lines, int priority)
    {
        if((inDialogue && priority < currentPriority) || lines == currentDialogueLines)
        {
            return;
        }

        playerAvatar.sprite = null;
        npcAvatar.sprite = null;

        lineIndex = 0;
        currentDialogueLines = lines;
        DisplayLine();
        inDialogue = true;
    }

    private void DisplayLine()
    {
        if (lineIndex < 0 || lineIndex >= currentDialogueLines.Count || currentDialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (dialogueSoundSource != null)
        {
            Destroy(dialogueSoundSource.gameObject);
        }

        Line currentLine = currentDialogueLines[lineIndex];

        if(currentLine.talkEvent != null && currentLine.talkEvent.GetPersistentEventCount() > 0)
        {
            currentLine.talkEvent.Invoke();
        }

        if (currentLine.isPlayer)
        {
            currentLine.senderName = "Stuart Mitchell";
        }

        StopAllCoroutines();

        if(nameText.text != currentLine.senderName)
        {
            StartCoroutine(FillText(nameText, currentLine.senderName));
        }
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

            if(npcAvatar.sprite != null)
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

        if (dialogueSoundSource != null)
        {
            Destroy(dialogueSoundSource.gameObject);
        }

        currentDialogueLines = null;

        inDialogue = false;
    }

    IEnumerator ImageFade(Image img, Color colour, float fadeSpeed)
    {
        while(img.color != colour)
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
            yield return new WaitForSeconds(characterDelay);
        }
    }

    IEnumerator ToggleDialogueUi(bool open)
    {
        float posY = -180f;
        if (open)
        {
            posY = 150f;
        }

        while(dialogueBox.anchoredPosition != new Vector2(0, posY))
        {
            dialogueBox.anchoredPosition = new Vector2(0, Mathf.MoveTowards(dialogueBox.anchoredPosition.y, posY, openSpeed * Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
    }
}
