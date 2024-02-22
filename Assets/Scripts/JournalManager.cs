using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum EntryType
{
    enemy, misc
}

[System.Serializable]
public class JournalEntry
{
    public string Title;
    public bool Owned = false;

    public EntryType Type;
    [TextArea(5, 20)]
    public string Description;
    public Sprite Picture;
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

    public static void FindEntry(int index)
    {
        JournalManager manager = GetJournalObj();

        if (index < 0 || index > manager.EntryContainer.Entries.Count)
        {
            return;
        }

        manager.EntryContainer.Entries[index].Owned = !manager.EntryContainer.Entries[index].Owned;

        SaveEntries();
    }

    public static void LoadEntries()
    {
        JournalManager manager = GetJournalObj();
        string path = Application.persistentDataPath + "/journal.json";

        for (int i = 0; i < manager.EntryContainer.Entries.Count; i++)
        {
            manager.EntryContainer.Entries[i].Owned = false;
        }

        if (!File.Exists(path))
        {
            return;
        }

        string jsonData = File.ReadAllText(path);
        JournalListContainer savedList = JsonUtility.FromJson<JournalListContainer>(jsonData);

        for(int i = 0; i < savedList.Entries.Count; i++)
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

    public static JournalManager GetJournalObj()
    {
        return GameObject.FindWithTag("Global").GetComponent<JournalManager>();
    }
}
