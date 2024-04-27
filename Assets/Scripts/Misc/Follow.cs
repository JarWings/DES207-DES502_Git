using UnityEngine;

public class Follow : MonoBehaviour
{
    void Update()
    {
        if (PlayerCharacter.Instance == null) return;
        transform.position = Vector2.Lerp(transform.position, PlayerCharacter.Instance.transform.position, 6.4f * Time.unscaledDeltaTime);
    }
}
