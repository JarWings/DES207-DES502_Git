using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum EntryType { enemy, item, misc }

[System.Serializable]
public class JournalEntry
{
    public string Title;
    public bool Owned = false;

    public EntryType Type;
    [TextArea(5, 34)]
    public string Description;
    public Sprite Picture, missingImage;
}

[System.Serializable]
public class JournalListContainer
{
    public List<JournalEntry> Entries = new();
}

public class JournalManager : MonoBehaviour
{
    public JournalListContainer EntryContainer;

    private void Start()
    {
        LoadEntries();
    }
	
	private void Update()
	{
		if(Input.GetKey(KeyCode.Alpha4) && Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.Alpha0)) // cheat code to unlock all entries
		{
			for(int i = 0; i < EntryContainer.Entries.Count; i++) FindEntry(EntryContainer.Entries[i].Title);
		}
	}

    public static JournalEntry ReturnEntry(int index)
    {
        JournalManager manager = GetJournalObj();

        return manager.EntryContainer.Entries[index];
    }

    public static void FindEntry(string entry)
    {
        JournalManager manager = GetJournalObj();

        int index = -1;

        if (entry == "" || entry == null) return;

        for (int i = 0; i < manager.EntryContainer.Entries.Count; i++) 
        {
            if (manager.EntryContainer.Entries[i].Title.ToLower() == entry.ToLower()) index = i;
        }

        if (index < 0 || index > manager.EntryContainer.Entries.Count) return;

        manager.EntryContainer.Entries[index].Owned = true;

        SaveEntries();
    }

    public static void LoadEntries()
    {
        JournalManager manager = GetJournalObj();
        string path = Application.persistentDataPath + "/journal.json";

        for (int i = 3; i < manager.EntryContainer.Entries.Count; i++)
        {
            manager.EntryContainer.Entries[i].Owned = false;
        }

        if (!File.Exists(path)) return;

        string jsonData = File.ReadAllText(path);
        JournalListContainer savedList = JsonUtility.FromJson<JournalListContainer>(jsonData);

        for (int i = 0; i < savedList.Entries.Count; i++)
        {
            manager.EntryContainer.Entries[i].Owned = savedList.Entries[i].Owned;
        }
    }

    public static void SaveEntries()
    {
        JournalManager manager = GetJournalObj();

        string path = Application.persistentDataPath + "/journal.json";
        string jsonData = JsonUtility.ToJson(manager.EntryContainer, true);

        File.WriteAllText(path, jsonData);
    }

    public static JournalManager GetJournalObj() // replace with instance in future!
    {
        return GameObject.FindWithTag("Global").GetComponent<JournalManager>();
    }
}