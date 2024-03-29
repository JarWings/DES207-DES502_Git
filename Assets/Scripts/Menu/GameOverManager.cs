using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameOverManager : MonoBehaviour
{
	public static GameOverManager Instance;
	
	public static bool isOver = false;
	
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
		
		if(Input.GetButtonDown("Jump")) SwitchCharacter(1);
		if(Input.GetKeyDown(KeyCode.Return)) SubmitScore();
	}
	
	void SwitchCharacter(int input)
	{
		selectedIndex = (selectedIndex + input) % character.Length;
		
		for(int i = 0; i < characterText.Length; i++) characterText[i].color = i == selectedIndex ? Color.green : Color.white;
		FlipCharacter(0);
	}

	void FlipCharacter(int input)
	{
		if(inputHeld) return;
				
		character[selectedIndex] = (character[selectedIndex] + input) % characters.Length;
		character[selectedIndex] = character[selectedIndex] < 0 ? characters.Length - 1 : character[selectedIndex];
		characterText[selectedIndex].text = characters[character[selectedIndex]].ToString();
	}
	
	public static void GameOver()
	{
		if(isOver) return;
		Instance.uiGroup.alpha = 1f;
		Instance.bgPanel.color = Color.black;
		
		LeaderboardManager.allowTimer = false;
		Instance.gameOverText.text = "Game Over! Your time: " + TimeSpan.FromSeconds(LeaderboardManager.currentTime).ToString(@"hh\:mm\:ss\:ff") + "\nEnter your name for the leaderboard.";
	
		isOver = true;
	}
	
	public void SubmitScore()
	{
		isOver = false;

		Instance.uiGroup.alpha = 0f;
		Instance.bgPanel.color = Color.clear;
		
		LeaderboardManager.AddScore(characters[character[0]].ToString() + characters[character[1]].ToString() + characters[character[2]].ToString());
		SceneChangeManager.LoadScene("MainMenu");
	}
}