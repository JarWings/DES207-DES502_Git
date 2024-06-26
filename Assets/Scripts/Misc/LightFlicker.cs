using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public float flickerSpeed = 1f;
    public float spinSpeed = 0f;
    private float startSpeed;

    public AudioClip buzzSound;
    private AudioSource buzzingSource;

    private MeshRenderer meshRenderer;
    private Color startColour, targetColour;
    void Start()
    {
        TryGetComponent(out meshRenderer);

        buzzingSource = AudioManager.PlayAudio(AudioType.soundFX, buzzSound, null, transform.position, null, 20f, Random.Range(.8f, 1.2f), 1, 0, 40, true);

        startSpeed = flickerSpeed;
        startColour = meshRenderer.material.GetColor("_Color");
        targetColour = NewColour();
    }

    void Update()
    {
        meshRenderer.transform.parent.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
        Flicker();
    }

    private void Flicker()
    {
        if(meshRenderer.material.GetColor("_Color") != targetColour)
        {
            meshRenderer.material.SetColor("_Color", Color.Lerp(meshRenderer.material.GetColor("_Color"), targetColour, Time.deltaTime * flickerSpeed));
            buzzingSource.volume = targetColour.a / meshRenderer.material.GetColor("_Color").a; // changes the volume of the buzzing based on how bright the light is
        }
        else
        {
            targetColour = NewColour();
        }
    }

    private Color NewColour()
    {
        flickerSpeed = Random.Range(startSpeed / 2f, startSpeed * 2f);

        Color newCol = startColour;
        newCol.a = Random.Range(0f, .7f);

        return newCol;
    }
}
