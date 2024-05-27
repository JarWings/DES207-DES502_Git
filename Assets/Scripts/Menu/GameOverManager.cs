using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
	public static GameOverManager Instance;
	
	public static bool isOver = false;

	public AudioClip gameOverMusic, victoryMusic;
	public GameObject diePrefab;

	[Header("UI")]
	public CanvasGroup uiGroup;
	public Image bgPanel, heldBarFill;
	public Sprite winsprite, losesprite;
	public TMP_Text gameOverText;
	public TMP_Text[] characterText;
	public int[] character;
	private int selectedIndex;
	
	private string characterString = "abcdefghijklmnopqrstuvwxyz";
	private char[] characters;
	
	private bool inputHeld = false;
	public bool win = false;

	private float attackHeldTime = 0f;
	
	void Awake()
	{
		characters = characterString.ToCharArray();
		Instance = this;
	}
	
	void Start()
	{
		SwitchCharacter(0);
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.C)) 
		{
			Cursor.visible = !Cursor.visible;
			Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
		}

		if(!isOver) return;
		
		float vert = Input.GetAxisRaw("Vertical");
		if(Mathf.Abs(vert) == 1f) 
		{
			FlipCharacter((int)vert);
			inputHeld = true;
		}else
		{
			inputHeld = false;
		}
		
		if(Input.GetButtonDown("Jump") && win) SwitchCharacter(1);

		attackHeldTime = Input.GetButton("Attack") ? attackHeldTime + Time.unscaledDeltaTime / 2f : 0f;
		heldBarFill.fillAmount = attackHeldTime / 1f;

		if (attackHeldTime >= 1f) 
		{
			attackHeldTime = 0f;

			if (win) 
			{
				SubmitScore();
				return;
			}

			SceneChangeManager.LoadScene("MainMenu");
		}
	}
	
	void SwitchCharacter(int input)
	{
		selectedIndex = (selectedIndex + input) % character.Length; // increments the selected character, flips back to 0 when it reaches the character entry length
		
		for(int i = 0; i < characterText.Length; i++) characterText[i].color = i == selectedIndex ? Color.green : Color.white;
		FlipCharacter(0);
	}

	void FlipCharacter(int input)
	{
		if(inputHeld) return;
				
		character[selectedIndex] = (character[selectedIndex] + input) % characters.Length; // increments the selected character
		character[selectedIndex] = character[selectedIndex] < 0 ? characters.Length - 1 : character[selectedIndex]; // decrements the character 
		characterText[selectedIndex].text = characters[character[selectedIndex]].ToString();
	}
	
	public static void GameOver(bool winState = false)
	{
		if(isOver) return;

		PlayerCharacter.Instance.enabled = false;
		PlayerCharacter.Instance.GetComponent<Collider2D>().enabled = false;
		PlayerCharacter.Instance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

		if (!winState) 
		{
			PlayerCharacter.Instance.transform.GetComponent<SpriteRenderer>().enabled = true;
			Instantiate(Instance.diePrefab, PlayerCharacter.Instance.transform.position, Instance.transform.rotation);
		}

		Instance.win = winState;

		Instance.StartCoroutine(Instance.FadeCanvas(Instance.uiGroup, 1f));

		Instance.bgPanel.sprite = winState ? Instance.winsprite : Instance.losesprite;
		Instance.bgPanel.color = Color.white;
		Instance.heldBarFill.enabled = true;


		LeaderboardManager.allowTimer = false;
		Instance.gameOverText.text = (Instance.win ? "Congratulations! Your time: " + TimeSpan.FromSeconds(LeaderboardManager.currentTime).ToString(@"hh\:mm\:ss\:ff") + "\nEnter your name for the leaderboard. Press the DASH button to change character, and hold ATTACK to submit." : "Hold the ATTACK button to continue.");

		MusicManager.ChangeTrack(winState == false ? Instance.gameOverMusic : Instance.victoryMusic, false, 2f);

		for (int i = 0; i < Instance.characterText.Length; i++) 
		{
			Instance.characterText[i].enabled = Instance.win;
		}

		isOver = true;
	}

	IEnumerator FadeCanvas(CanvasGroup group, float level) 
	{
		while (group != null && group.alpha != level) 
		{
			group.alpha = Mathf.MoveTowards(group.alpha, level, Time.unscaledDeltaTime);
			yield return new WaitForEndOfFrame();
		}
	}
	
	public void SubmitScore()
	{
		isOver = false;

		Instance.StartCoroutine(Instance.FadeCanvas(Instance.uiGroup, 0f));
		Instance.bgPanel.color = Color.clear;
		Instance.heldBarFill.enabled = false;

		if (Instance.win) LeaderboardManager.AddScore(characters[character[0]].ToString() + characters[character[1]].ToString() + characters[character[2]].ToString());
		SceneChangeManager.LoadScene("Outro");
	}
}