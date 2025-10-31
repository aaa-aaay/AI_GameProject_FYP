using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections;
using UnityEngine.Animations;

public class archery_agent : Agent
{
    [Header("AI")]
    [SerializeField] private float minAimTime = 2f;
    private bool isTurn;

    private float minForce;
    private float maxForce;
    private float maxYaw;
    private float minPitch;
    private float maxPitch;

    private archery_handler handler;
    private archery_settings settings;
    private DecisionRequester decisionRequester;

    private bool canShoot;
    
    private float force;
    private float yaw;
    private float pitch;
    private bool shoot;

    private float lastDistance;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip aimAnim;
    [SerializeField] private PositionConstraint stringConstraint;
    [SerializeField] private Transform spine;
    [SerializeField] private Transform bow;
    [SerializeField] private Transform arrow;

    private Vector3 spineRotation;
    private Vector3 stringOrigin;

    public override void Initialize()
    {
        isTurn = false;
        handler = archery_handler.instance;
        settings = handler.settings;
        decisionRequester = GetComponent<DecisionRequester>();

        minForce = settings.minForce;
        maxForce = settings.maxForce;
        maxYaw = settings.maxYaw;
        minPitch = settings.minPitch;
        maxPitch = settings.maxPitch;

        spineRotation = spine.rotation.eulerAngles;
        stringOrigin = stringConstraint.transform.localPosition;
    }

    public void StartTurn()
    {
        isTurn = true;
        lastDistance = Vector3.Distance(handler.targetObject.transform.position, handler.estimateLanding);
        stringConstraint.constraintActive = true;
        arrow.gameObject.SetActive(true);
        decisionRequester.enabled = true;
    }

    public override void OnEpisodeBegin()
    {
        StopAllCoroutines();
        if (minForce > 10f) force = minForce; else force = 10f;
        yaw = 0f;
        if (minPitch > 0f) pitch = minPitch; else pitch = 0f;
        canShoot = false;
        StartCoroutine(ShootTimer());
        shoot = false;
    }

    private IEnumerator ShootTimer()
    {
        yield return new WaitForSeconds(minAimTime);

        canShoot = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 toTarget = handler.targetObject.transform.position - transform.position;
        Vector3 toTargetLocal = transform.InverseTransformDirection(toTarget);
        
        sensor.AddObservation(toTargetLocal.normalized);
        sensor.AddObservation(toTargetLocal.magnitude);

        Vector3 windWorld = Quaternion.Euler(0f, handler.windDirection, 0f) * Vector3.forward * handler.windSpeed;
        Vector3 windLocal = transform.InverseTransformDirection(windWorld);
        sensor.AddObservation(windLocal.normalized);
        sensor.AddObservation(windLocal.magnitude);

        Vector3 toEstimateLanding = handler.estimateLanding - transform.position;
        Vector3 toEstimateLandingLocal = transform.InverseTransformDirection(toEstimateLanding);
        sensor.AddObservation(toEstimateLandingLocal.normalized);
        sensor.AddObservation(toEstimateLandingLocal.magnitude);
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (!canShoot && Vector3.Distance(handler.targetObject.transform.position, handler.estimateLanding) > 1f) actionMask.SetActionEnabled(branch: 3, actionIndex: 1, isEnabled: false);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isTurn) return;

        AddReward(-0.001f * Time.deltaTime);

        var da = actions.DiscreteActions;

        // Force
        switch (da[0])
        {
            case 1:
                force = Mathf.Min(force + 10f * Time.deltaTime, maxForce);
                break;
            case 2:
                force = Mathf.Max(force - 10f * Time.deltaTime, minForce);
                break;
        }

        // Yaw
        switch (da[1])
        {
            case 1:
                yaw = Mathf.Max(yaw - 10f * Time.deltaTime, -maxYaw);
                break;
            case 2:
                yaw = Mathf.Min(yaw + 10f * Time.deltaTime, maxYaw);
                break;
        }

        // Pitch
        switch (da[2])
        {
            case 1:
                pitch = Mathf.Min(pitch + 10f * Time.deltaTime, maxPitch);
                break;
            case 2:
                pitch = Mathf.Max(pitch - 10f * Time.deltaTime, minPitch);
                break;
        }

        if (da[3] == 1)
        {
            if (Vector3.Distance(handler.targetObject.transform.position, handler.estimateLanding) > 3f)
            {
                AddReward(-10);
            }

            decisionRequester.enabled = false;
            isTurn = false;
            stringConstraint.constraintActive = false;
            stringConstraint.transform.localPosition = stringOrigin;
            arrow.gameObject.SetActive(false);
            shoot = true;
            handler.Shoot(bow.position, force, yaw, pitch);
            return;
        }

        float newDistance = Vector3.Distance(handler.targetObject.transform.position, handler.estimateLanding);
        AddReward((lastDistance - newDistance));
        lastDistance = newDistance;

        handler.UpdateUI(bow.position, force, yaw, pitch);

        animator.enabled = true;
        float normalizedForce = Mathf.InverseLerp(minForce, maxForce, force);
        animator.speed = 0f;
        animator.Play(aimAnim.name, 0, normalizedForce);
        animator.Update(0f);
        animator.enabled = false;

        spine.localRotation = Quaternion.Euler(spineRotation.x, spineRotation.y + 20f + yaw, spineRotation.z - pitch);
    }

    public void OnHit(float point)
    {
        AddReward(point);
        EndEpisode();
    }
}
