using System.Collections;
using UnityEngine;

public class RacketSwing : MonoBehaviour
{

    [SerializeField]
    private GameObject _racketGO;

    [SerializeField]
    private Transform _lerpTransform;

    private Vector3 _origPos;
    private Quaternion _origRot;

    private bool _racketSwinging = false;
    private bool _finishedSwiping = false;

    [SerializeField] private Racket _racket;



    private void Start()
    {
        _origRot = _racketGO.transform.localRotation;
        _origPos = _racketGO.transform.localPosition;
        _racket.DeactivateCollider();

        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();

        inputManager.OnClick += HandleClick;
    }

    private void HandleClick()
    {
        if(_racketSwinging) return;

        _racketSwinging = true;
        _racket.ActivateCollider();

        StartCoroutine(LerpRacket(_lerpTransform, 0.2f));
    }

    private IEnumerator LerpRacket(Transform targetTransform, float duration)
    {
        float time = 0;
        Vector3 startPosition = _origPos;
        Quaternion startRotation = _origRot;
        while (time < duration)
        {
            _racketGO.transform.localPosition = Vector3.Lerp(startPosition, targetTransform.localPosition, time / duration);
            _racketGO.transform.localRotation = Quaternion.Slerp(startRotation, targetTransform.localRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _racketGO.transform.localPosition = targetTransform.localPosition;
        _racketGO.transform.localRotation = targetTransform.localRotation;

        _racket.DeactivateCollider();

        yield return new WaitForSeconds(0.1f);


        time = 0;
        startPosition = _racketGO.transform.localPosition;
        startRotation = _racketGO.transform.localRotation;
        while (time < duration)
        {
            _racketGO.transform.localPosition = Vector3.Lerp(startPosition, _origPos, time / duration);
            _racketGO.transform.localRotation = Quaternion.Slerp(startRotation, _origRot, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _racketGO.transform.localPosition = _origPos;
        _racketGO.transform.localRotation = _origRot;
        _racketSwinging = false;
    }




}
