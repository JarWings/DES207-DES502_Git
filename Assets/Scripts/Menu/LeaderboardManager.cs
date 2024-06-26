using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Score
{
    public string playerName = "Unnamed";
    public string playDate = "0/0/0000";
    public float time = 0f;
}

[System.Serializable]
public class SerializableList
{
    public List<Score> scores = new();
}

public class LeaderboardManager : MonoBehaviour
{
    public SerializableList scoreList = new();
    public static float currentTime = 0f;
	
	public static bool allowTimer =  false;

	void Start()
	{
		allowTimer = !(SceneManager.GetActiveScene().name == "MainMenu");
	}

    private void Update()
    {
        if (allowTimer) currentTime += Time.deltaTime;
    }

    public static void ResetTime()
    {
        currentTime = 0f;
    }

    public static void AddScore(string playerName)
    {
        LeaderboardManager leaderboardObj = GetLeaderboardObj();

        float time = currentTime;

        playerName = playerName.Length <= 0 ? "Player" + leaderboardObj.scoreList.scores.Count : playerName;

        Score newScore = new();
        newScore.playerName = playerName;
        newScore.playDate = System.DateTime.Now.ToString();
        newScore.time = time;

        for (int i = 0; i < leaderboardObj.scoreList.scores.Count; i++)
        {
            Score tempScore = leaderboardObj.scoreList.scores[i];

            if (tempScore.playerName == playerName)
            {
                if (time < tempScore.time)
                {
                    tempScore.time = time; // replaces previous best time with new time if it's better and the name matches
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        leaderboardObj.scoreList.scores.Add(newScore);
        SaveScores();
    }

    public static void LoadScores()
    {
        LeaderboardManager leaderboardObj = GetLeaderboardObj();

        string path = Application.persistentDataPath + "/scores.json";
        if (!File.Exists(path)) return;

        string jsonData = File.ReadAllText(path);
        leaderboardObj.scoreList = JsonUtility.FromJson<SerializableList>(jsonData);
        leaderboardObj.scoreList.scores.Sort((x, y) => y.time.CompareTo(x.time));
    }

    public static void SaveScores()
    {
        LeaderboardManager leaderboardObj = GetLeaderboardObj();

        string path = Application.persistentDataPath + "/scores.json";
        string jsonData = JsonUtility.ToJson(leaderboardObj.scoreList, true);

        File.WriteAllText(path, jsonData);
    }

    public static LeaderboardManager GetLeaderboardObj() // replace with instance in future!
    {
        return GameObject.FindWithTag("Global").GetComponent<LeaderboardManager>();
    }
}
