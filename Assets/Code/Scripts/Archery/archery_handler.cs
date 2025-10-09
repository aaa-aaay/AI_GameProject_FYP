using UnityEngine;
using Unity.Cinemachine;
using UnityEditor.Toolbars;
using System.Collections;

public class archery_handler : MonoBehaviour
{
    public static archery_handler instance;

    [Header("Cinemachine Camera")]
    [SerializeField, Tooltip("Time in seconds for camera to stay at arrow position after hitting")] private float cameraStay = 3f;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera arrowCamera;
    [SerializeField] private GameObject playerObject;
    private archery_player player;
    [SerializeField] private GameObject agentObject;
    private archery_agent agent;
    [field: SerializeField] public GameObject targetObject { get; private set; }

    private int playerPoint;
    private int agentPoint;

    [Header("Arrow")]
    [SerializeField, Tooltip("Number of arrows in object pool. Set to 0 to disable.")] private int numArrows = 3;
    private int currentArrow;
    [SerializeField] private GameObject arrowPrefab;
    private arrow[] arrows;

    [Header("Settings")]
    [SerializeField] private archery_ui_handler uiHandler;
    [field: SerializeField] public archery_settings settings { get; private set; }

    private bool isPlayerTurn;
    private bool canShoot;

    public float windDirection{ get; private set; }
    public float windSpeed { get; private set; }

    private float minTargetDistance;
    private float maxTargetDistance;
    private float maxLateralDistance;

    private float targetDistance;
    private float lateralDistance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        if (!playerCamera) Debug.LogError("playerCamera not assigned.");
        if (playerObject)
            playerCamera.Target.TrackingTarget = playerObject.transform;
        else
            Debug.LogWarning("playerObject not assigned.");
        playerCamera.enabled = true;

        if (!arrowCamera) Debug.LogError("arrowCamera not assigned.");
        arrowCamera.enabled = false;

        player = playerObject.GetComponent<archery_player>();
        if (!player)
            Debug.LogError("archery_player has not been added.");

        agent = agentObject.GetComponent<archery_agent>();
        if (!agent)
            Debug.LogError("archery_agent has not been added.");

        playerPoint = 0;
        agentPoint = 0;

        arrows = new arrow[numArrows];
        for (int i = 0; i < numArrows; i++)
        {
            arrows[i] = Instantiate(arrowPrefab).GetComponent<arrow>();
            arrows[i].gameObject.SetActive(false);
        }
        currentArrow = 0;

        minTargetDistance = settings.minTargetDistance;
        maxTargetDistance = settings.maxTargetDistance;
        maxLateralDistance = settings.maxLateralDistance;

        if (maxTargetDistance < minTargetDistance)
        {
            Debug.LogWarning("maxTargetDistance (" + maxTargetDistance + ") less than minTargetDistance (" + minTargetDistance + "). maxTargetDistance set to minTargetDistance.");
            maxTargetDistance = minTargetDistance;
        }

        isPlayerTurn = true;
        canShoot = true;

        player.Initialize();
        agent.enabled = true;
        StartCoroutine(PlayerTurn());
    }

    public void Shoot(float force, float yaw, float pitch)
    {
        if (!canShoot) return;
        canShoot = false;

        playerCamera.enabled = false;

        if (isPlayerTurn)
            arrows[currentArrow].transform.position = playerObject.transform.position;
        else
            arrows[currentArrow].transform.position = agentObject.transform.position;

        arrows[currentArrow].transform.rotation = Quaternion.identity;
        arrows[currentArrow].gameObject.SetActive(true);

        arrows[currentArrow].Shoot(force, yaw, pitch, windDirection, windSpeed);

        arrowCamera.Target.TrackingTarget = arrows[currentArrow].transform;
        arrowCamera.enabled = true;

        Debug.Log($"Force: {force}\tYaw: {yaw}\tPitch: {pitch}");
    }

    private IEnumerator PlayerTurn()
    {
        playerCamera.Target.TrackingTarget = playerObject.transform;
        
        windDirection = Random.Range(0f, 360f);
        windSpeed = Random.Range(0f, settings.maxWindSpeed);

        targetDistance = Random.Range(minTargetDistance, maxTargetDistance);
        lateralDistance = Random.Range(-maxLateralDistance, maxLateralDistance);

        targetObject.transform.position = playerObject.transform.position + new Vector3(lateralDistance, 1, targetDistance);

        yield return new WaitForSeconds(3f);

        player.StartTurn();
        canShoot = true;
    }

    private IEnumerator AgentTurn()
    {
        playerCamera.Target.TrackingTarget = agentObject.transform;
        
        windDirection = Random.Range(0f, 360f);
        windSpeed = Random.Range(0f, settings.maxWindSpeed);

        targetDistance = Random.Range(minTargetDistance, maxTargetDistance);
        lateralDistance = Random.Range(-maxLateralDistance, maxLateralDistance);

        windDirection = 0;
        windSpeed = 0;

        targetObject.transform.position = agentObject.transform.position + new Vector3(lateralDistance, 1, targetDistance);

        yield return new WaitForSeconds(3f);

        agent.StartTurn();
        canShoot = true;
    }

    public void OnHit(int point)
    {
        currentArrow++;
        if (currentArrow == numArrows)
            currentArrow = 0;
        arrows[currentArrow].gameObject.SetActive(false);

        if (isPlayerTurn)
        {
            playerPoint += point;
            isPlayerTurn = false;
        }
        else
        {
            agentPoint += point;
            isPlayerTurn = true;

            if (point > 0) agent.OnHit(point);
            else
            {
                agent.OnHit(-Vector3.Distance(targetObject.transform.position, agentObject.transform.position));
            }
        }
        uiHandler.set_point(playerPoint, agentPoint);

        if (point > 0) Debug.Log($"Hit: {point}");

        StartCoroutine(ReturnCamera());
    }

    public void UpdateUI(float force, float yaw, float pitch)
    {
        uiHandler.set_value(force, yaw, pitch, windDirection, windSpeed, targetDistance, lateralDistance);
    }

    private IEnumerator ReturnCamera()
    {
        yield return new WaitForSeconds(cameraStay);

        if (isPlayerTurn) StartCoroutine(PlayerTurn()); else StartCoroutine(AgentTurn());

        arrowCamera.enabled = false;
        playerCamera.enabled = true;
    }
}
