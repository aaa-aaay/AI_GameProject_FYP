using UnityEngine;

public class Timer : MonoBehaviour
{

    private bool timerStarted = false;      
    [HideInInspector] public float elapsedTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elapsedTime = 0f;
        timerStarted = false;
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        timerStarted = true;

    }

   public float StopTimer()
   {
        timerStarted = false;
        return elapsedTime;
   }

    // Update is called once per frame
    void Update()
    {
        if(timerStarted) elapsedTime += Time.deltaTime;


    }
}
