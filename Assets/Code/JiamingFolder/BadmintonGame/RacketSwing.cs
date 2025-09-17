using System;
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

    public bool racketSwinging = false;




    [SerializeField] private Racket _racket;



    private void Start()
    {
        _origRot = _racketGO.transform.localRotation;
        _origPos = _racketGO.transform.localPosition;
        _racket.DeactivateCollider();

    }


    public void StartSwing(Racket.ShotType shotType,int direction = 3)
    {
        if(racketSwinging) return;
        racketSwinging = true;
        _racket.AssignShot(shotType, direction);
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
        racketSwinging = false;
    }




}
