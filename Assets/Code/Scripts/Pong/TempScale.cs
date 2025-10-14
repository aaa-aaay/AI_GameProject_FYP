using UnityEngine;

public class TempScale : MonoBehaviour
{
    private float scale_multiplier;
    private float time_passed;
    private float scale_time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time_passed = 0;
        transform.localScale *= scale_multiplier;
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;
        if (time_passed >= scale_time)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        transform.localScale /= scale_multiplier;
    }

    public void SetScaleMultiplier(float multiplier)
    {
        scale_multiplier = multiplier;
    }

    public void SetScaleTime(float time)
    {
        scale_time = time;
    }

    public void ResetTimer()
    {
        time_passed = 0;
    }
}
