using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class archery_agent : Agent
{
    private bool isTurn;

    private float minForce;
    private float maxForce;
    private float maxYaw;
    private float minPitch;
    private float maxPitch;

    private archery_handler handler;
    private archery_settings settings;

    private float force;
    private float yaw;
    private float pitch;
    private bool shoot;

    public override void Initialize()
    {
        isTurn = false;
        handler = archery_handler.instance;
        settings = handler.settings;

        minForce = settings.minForce;
        maxForce = settings.maxForce;
        maxYaw = settings.maxYaw;
        minPitch = settings.minPitch;
        maxPitch = settings.maxPitch;

        StartTurn();
    }

    public void StartTurn()
    {
        isTurn = true;
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        if (minForce > 10f) force = minForce; else force = 10f;
        yaw = 0f;
        if (minPitch > 0f) pitch = minPitch; else pitch = 0f;
        shoot = false;
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

        sensor.AddObservation(force);
        sensor.AddObservation(yaw);
        sensor.AddObservation(pitch);
        sensor.AddObservation(shoot);
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
            isTurn = false;
            shoot = true;
            handler.Shoot(force, yaw, pitch);
        }

        handler.UpdateUI(force, yaw, pitch);
    }

    public void OnHit(int point)
    {
        AddReward(point * 0.01f);
    }
}
