using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public List<string> Tips = new();

    [Header("UI")]
    public Image fadePanel;
    public TMP_Text tipText;
    public Image loadIcon;

    public static LoadingManager Instance;

    private void Start()
    {
        Instance = this;
    }

    public static void StartLoad()
    {
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.FadeImage(Instance.fadePanel, Color.black, 2f));
        Instance.StartCoroutine(Instance.FadeImage(Instance.loadIcon, Color.white, 1f));

        Instance.tipText.enabled = true;
        Instance.tipText.text = Instance.Tips[Random.Range(0, Instance.Tips.Count)];
    }

    public static void EndLoad()
    {
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.FadeImage(Instance.fadePanel, Color.clear, 1f));
        Instance.StartCoroutine(Instance.FadeImage(Instance.loadIcon, Color.clear, 4f));

        Instance.tipText.enabled = false;
    }

    IEnumerator FadeImage(Image img, Color target, float speed)
    {
        while(img.color != target)
        {
            img.color = Color.Lerp(img.color, target, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
    }
}
