using UnityEngine;

public class RandomGlitchAnimation : MonoBehaviour
{
    [SerializeField] private float _minTime = 1.0f;
    [SerializeField] private float _maxTime = 5.0f;

    private float _time = 0.0f;
    private float _randomTime = 0.0f;
    private Animator _animator;


    private void Start()
    {
        _time = 0;
        _animator = GetComponent<Animator>();
        _randomTime = Random.Range(_minTime, _maxTime);
    }
    private void Update()
    {
        _time += Time.deltaTime;

        if( _time > _randomTime)
        {
            _animator.SetTrigger("PlayGlitch");
            _randomTime = Random.Range(_minTime, _maxTime);
            _time = 0.0f;
        }
    }
}
