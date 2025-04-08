using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public enum TimeSpeed { Paused, Normal, Fast, VeryFast }

    public float dayDuration = 5f;
    public UnityEvent OnWeekPassed;

    private float timer;
    private int dayCount = 1;
    private int weekCount = 1;
    private TimeSpeed currentSpeed = TimeSpeed.Normal;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        float speedMultiplier = GetSpeedMultiplier();
        if (speedMultiplier == 0f) return;

        timer += Time.deltaTime * speedMultiplier;

        if (timer >= dayDuration)
        {
            timer = 0f;
            dayCount++;

            if (dayCount > 7)
            {
                dayCount = 1;
                weekCount++;
                OnWeekPassed?.Invoke();
                Debug.Log("Nouvelle semaine commencée : semaine " + weekCount);
            }

            UISystem.Instance.UpdateDate(dayCount, weekCount);
        }

        UISystem.Instance.UpdateDayProgress(timer / dayDuration);
    }

    float GetSpeedMultiplier()
    {
        return currentSpeed switch
        {
            TimeSpeed.Paused => 0f,
            TimeSpeed.Normal => 1f,
            TimeSpeed.Fast => 2f,
            TimeSpeed.VeryFast => 4f,
            _ => 1f
        };
    }

    public void SetSpeed(int speedIndex)
    {
        currentSpeed = (TimeSpeed)speedIndex;
    }

    public int GetDay() => dayCount;
    public int GetWeek() => weekCount;
}
