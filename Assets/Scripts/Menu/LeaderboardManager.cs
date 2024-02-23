using UnityEngine;
using System.IO;
using System.Collections.Generic;

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
    public float currentTime = 0f;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F5)) // temp
        {
            AddScore("Test" + Random.Range(1, 9999), currentTime);
            currentTime = 0f;
            SaveScores();
        }
    }

    public static void AddScore(string playerName, float time)
    {
        LeaderboardManager leaderboardObj = GetLeaderboardObj();

        if (playerName.Length <= 0)
        {
            return;
        }
        
        Score newScore = new();
        newScore.playerName = playerName;
        newScore.playDate = System.DateTime.Now.ToString();
        newScore.time = time;

        for(int i = 0; i < leaderboardObj.scoreList.scores.Count; i++)
        {
            Score tempScore = leaderboardObj.scoreList.scores[i];

            if(tempScore.playerName == playerName)
            {
                if(time < tempScore.time)
                {
                    // better time, replace current score with new time
                    tempScore.time = time;
                    return;
                }
                else
                {
                    // worse time, don't add to leaderboard
                    return;
                }
            }
        }

        leaderboardObj.scoreList.scores.Add(newScore);
    }

    public static void LoadScores()
    {
        LeaderboardManager leaderboardObj = GetLeaderboardObj();

        string path = Application.persistentDataPath + "/scores.json";
        if (!File.Exists(path))
        {
            return;
        }

        string jsonData = File.ReadAllText(path);
        leaderboardObj.scoreList = JsonUtility.FromJson<SerializableList>(jsonData);
        leaderboardObj.scoreList.scores.Sort((y, x) => x.time.CompareTo(y.time));
    }

    public static void SaveScores()
    {
        LeaderboardManager leaderboardObj = GetLeaderboardObj();

        string path = Application.persistentDataPath + "/scores.json";
        string jsonData = JsonUtility.ToJson(leaderboardObj.scoreList, true);

        File.WriteAllText(path, jsonData);
    }

    public static LeaderboardManager GetLeaderboardObj()
    {
        return GameObject.FindWithTag("Global").GetComponent<LeaderboardManager>();
    }
}
