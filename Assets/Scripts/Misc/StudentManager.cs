using UnityEngine;
using TMPro;

public class StudentManager : MonoBehaviour
{
    public static StudentManager Instance;

    public GameObject blockObject;

    public TMP_Text studentCounterText;
    public int foundStudents = 0, totalStudents = 0;

    private void Awake()
    {
        Instance = this;
    }

    public static void AddToTotalStudents()
    {
        Instance.totalStudents++;
        Instance.UpdateCounterUi();
    }

    public static void FindStudent()
    {
        Instance.foundStudents++;
        Instance.UpdateCounterUi();

        if (Instance.foundStudents >= Instance.totalStudents) Instance.blockObject.SetActive(false);
    }

    public void UpdateCounterUi()
    {
        studentCounterText.text = foundStudents + "\n  /\n   " + totalStudents;
    }
}
