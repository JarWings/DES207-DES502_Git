using UnityEngine;
using TMPro;

public class StudentManager : MonoBehaviour
{
    public static StudentManager Instance;

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
    }

    public void UpdateCounterUi()
    {
        studentCounterText.text = foundStudents + "/" + totalStudents;
    }
}
