using UnityEngine;
using TMPro;

public class StudentManager : MonoBehaviour
{
    public static StudentManager instance { get; private set; }

    public TMP_Text studentCounterText;
    public int foundStudents = 0, totalStudents = 0;

    private void Awake()
    {
        instance = this;
    }

    public static void AddToTotalStudents()
    {
        instance.totalStudents++;
        instance.UpdateCounterUi();
    }

    public static void FindStudent()
    {
        instance.foundStudents++;
        instance.UpdateCounterUi();
    }

    public void UpdateCounterUi()
    {
        studentCounterText.text = foundStudents + "/" + totalStudents;
    }
}
