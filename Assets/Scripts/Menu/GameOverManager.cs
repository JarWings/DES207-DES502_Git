using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameOverManager : MonoBehaviour
{
	public static GameOverManager Instance;
	
	public static bool isOver = false;

	public AudioClip gameOverMusic;

	[Header("UI")]
	public CanvasGroup uiGroup;
	public Image bgPanel;
	public TMP_Text gameOverText;
	public TMP_Text[] characterText;
	public int[] character;
	private int selectedIndex;
	
	private string characterString = "abcdefghijklmnopqrstuvwxyz";
	private char[] characters;
	
	private bool inputHeld = false;
	public bool win = false;
	
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

		if (Input.GetButtonDown("Attack")) 
		{
			if (win) 
			{
				SubmitScore();
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

		Instance.win = winState;

		Instance.uiGroup.alpha = 1f;
		Instance.bgPanel.color = Color.black;
		
		LeaderboardManager.allowTimer = false;
		Instance.gameOverText.text = "Game Over! " + (Instance.win ? "Your time: " + TimeSpan.FromSeconds(LeaderboardManager.currentTime).ToString(@"hh\:mm\:ss\:ff") + "\nEnter your name for the leaderboard. Press the JUMP button to change character, and ATTACK to submit." : "Press the ATTACK button to continue.");

		MusicManager.ChangeTrack(Instance.gameOverMusic, false, 2f);

		for (int i = 0; i < Instance.characterText.Length; i++) 
		{
			Instance.characterText[i].enabled = Instance.win;
		}


		isOver = true;
	}
	
	public void SubmitScore()
	{
		isOver = false;

		Instance.uiGroup.alpha = 0f;
		Instance.bgPanel.color = Color.clear;
		
		if(Instance.win) LeaderboardManager.AddScore(characters[character[0]].ToString() + characters[character[1]].ToString() + characters[character[2]].ToString());
		SceneChangeManager.LoadScene("MainMenu");
	}
}