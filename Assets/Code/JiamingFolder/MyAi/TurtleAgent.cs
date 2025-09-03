using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using System.Collections;

public class TurtleAgent : Agent
{
    [SerializeField] private Transform _goal;
    [SerializeField] private Renderer _groundRenderer;
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _rotationSpeed = 180;

    private Renderer _renderer;

    [HideInInspector]public int currentEpisode = 0;
    [HideInInspector] public float cumulativeReward = 0f;

    private Color _defaultGroundColor;
    private Coroutine  _flashGroundCoroutine;

    public override void Initialize()
    {
        Debug.Log("Ai inited");
        _renderer = GetComponent<Renderer>();
        currentEpisode = 0;
        cumulativeReward = 0f;


        if(_groundRenderer != null)
        {
            _defaultGroundColor = _groundRenderer.material.color;
        }
            
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode begun");



        if (_groundRenderer != null && cumulativeReward != 0f)
        {
            Color flashColor = (cumulativeReward > 0f) ? Color.green : Color.red;

            // Stop any existing FlashGround coroutine before starting a new one
            if (_flashGroundCoroutine != null)
            {
                StopCoroutine(_flashGroundCoroutine);
            }

            _flashGroundCoroutine = StartCoroutine(FlashGround(flashColor, 3.0f));
        }




            currentEpisode++;
        cumulativeReward = 0f;
        _renderer.material.color = Color.blue;

        SpawnObjects();
    }

    private IEnumerator FlashGround(Color targetColor, float duration)
    {
        float elapsedTime = 0f;

        _groundRenderer.material.color = targetColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _groundRenderer.material.color = Color.Lerp(targetColor, _defaultGroundColor, elapsedTime / duration);
            yield return null;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // The Goal's position
        float goalPosX_normalized = _goal.localPosition.x / 5f;
        float goalPosZ_normalized = _goal.localPosition.z / 5f;

        // The Turtle's position
        float turtlePosX_normalized = transform.localPosition.x / 5f;
        float turtlePosZ_normalized = transform.localPosition.z / 5f;

        // The Turtle's direction (on the Y Axis)
        float turtleRotation_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        sensor.AddObservation(goalPosX_normalized);
        sensor.AddObservation(goalPosZ_normalized);
        sensor.AddObservation(turtlePosX_normalized);
        sensor.AddObservation(turtlePosZ_normalized);
        sensor.AddObservation(turtleRotation_normalized);

    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[0] = 0; // don't move - do nothing!

        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = 3;
        }
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions);

        AddReward(-10f / MaxStep);

        cumulativeReward += GetCumulativeReward();
    }

    private void SpawnObjects()
    {
        transform.localPosition = new Vector3(0f, 0.15f, 0f);
        transform.localRotation = Quaternion.identity;

        //randomi
        float randomAngle = Random.Range(0f, 360f);
        Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;

        float randomDistance = Random.Range(1f, 2.5f);

        Vector3 goalPosition = transform.localPosition + randomDirection * randomDistance;

        _goal.localPosition = new Vector3(goalPosition.x, 0.3f, goalPosition.z);
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];

        switch (action)
        {
            case 1: // Move forward
                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
                break;

            case 2: // Rotate left
                transform.Rotate(0f, -_rotationSpeed * Time.deltaTime, 0f);
                break;

            case 3: // Rotate right
                transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            GoalReached();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            SetReward(-2.0f);
            _renderer.material.color = Color.red;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            SetReward(-0.1f * Time.fixedDeltaTime);
        }


    }


    private void GoalReached()
    {
        AddReward(20.0f);
        cumulativeReward = GetCumulativeReward();

        EndEpisode();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _renderer.material.color = Color.blue;
        }
    }
}
