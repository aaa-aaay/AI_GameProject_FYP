using System;
using System.Collections;
using UnityEngine;

public class RacketSwing : MonoBehaviour
{

    [SerializeField]
    private GameObject _racketSwingGO;

    [SerializeField]
    private Transform _lerpTransform;

    private Vector3 _origPos;
    private Quaternion _origRot;


    [HideInInspector]
    public bool racketSwinging = false;

    [SerializeField] private float swingTime = 0.3f;


    [SerializeField] private Racket _racket;



    private void Start()
    {
        _origRot = _racketSwingGO.transform.localRotation;
        _origPos = _racketSwingGO.transform.localPosition;
        _racket.DeactivateCollider();

    }


    public void StartSwing(Racket.ShotType shotType,int direction = 3)
    {
        if(racketSwinging) return;
        racketSwinging = true;
        _racket.AssignShot(shotType, direction);
        _racket.ActivateCollider();

        StartCoroutine(LerpRacket(_lerpTransform, swingTime));
    }

    private IEnumerator LerpRacket(Transform targetTransform, float duration)
    {
        float time = 0;
        Vector3 startPosition = _origPos;
        Quaternion startRotation = _origRot;
        while (time < duration)
        {
            _racketSwingGO.transform.localPosition = Vector3.Lerp(startPosition, targetTransform.localPosition, time / duration);
            _racketSwingGO.transform.localRotation = Quaternion.Slerp(startRotation, targetTransform.localRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _racketSwingGO.transform.localPosition = targetTransform.localPosition;
        _racketSwingGO.transform.localRotation = targetTransform.localRotation;

        _racket.DeactivateCollider();

        yield return new WaitForSeconds(0.1f);


        time = 0;
        startPosition = _racketSwingGO.transform.localPosition;
        startRotation = _racketSwingGO.transform.localRotation;
        while (time < duration)
        {
            _racketSwingGO.transform.localPosition = Vector3.Lerp(startPosition, _origPos, time / duration);
            _racketSwingGO.transform.localRotation = Quaternion.Slerp(startRotation, _origRot, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _racketSwingGO.transform.localPosition = _origPos;
        _racketSwingGO.transform.localRotation = _origRot;
        racketSwinging = false;
    }




}
