using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class archery_handler : MonoBehaviour
{
    public static archery_handler instance;

    [Header("Cinemachine Camera")]
    [SerializeField, Tooltip("Time in seconds for camera to stay at arrow position after hitting")] private float cameraStay = 3f;
    [SerializeField] private CinemachineCamera playerLeftCamera;
    [SerializeField] private CinemachineCamera playerRightCamera;
    [SerializeField] private CinemachineCamera turnCamera;
    [SerializeField] private CinemachineCamera arrowCamera;
    [SerializeField] private GameObject playerObject;
    private archery_player player;
    [SerializeField] private GameObject agentObject;
    private archery_agent agent;
    [field: SerializeField] public GameObject targetObject { get; private set; }
    [SerializeField] private archery_windFX windFX;

    private int playerPoint;
    private int agentPoint;
    private int winCond;
    private int twoStarCond;
    private int threeStarCond;

    [Header("Arrow")]
    [SerializeField, Tooltip("Number of arrows in object pool. Set to 0 to disable.")] private int numArrows = 3;
    private int currentArrow;
    [SerializeField] private GameObject arrowPrefab;
    private arrow[] arrows;

    [Header("Settings")]
    [SerializeField] private trajectory_preview preview;
    [SerializeField] private archery_ui_handler uiHandler;
    [field: SerializeField] public archery_settings settings { get; private set; }

    [Header("Positions")]
    [SerializeField] private Transform shootPosition;
    [SerializeField] private Transform playerIdlePosition;
    [SerializeField] private Transform agentIdlePosition;

    private bool isPlayerTurn;
    private bool canShoot;
    private bool hasHit;

    public float windDirection{ get; private set; }
    public float windSpeed { get; private set; }

    private float minTargetDistance;
    private float maxTargetDistance;
    private float maxLateralDistance;

    private float targetDistance;
    private float lateralDistance;

    public Vector3 estimateLanding { get; private set; }

    [Header("AI Training")]
    [SerializeField] private bool isAiTraining = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        if (!playerLeftCamera) Debug.LogError("playerLeftCamera not assigned.");
        if (!playerRightCamera) Debug.LogError("playerRightCamera not assigned.");
        if (!turnCamera) Debug.LogError("turnCamera not assigned.");
        if (playerObject)
        {
            playerLeftCamera.Target.TrackingTarget = playerObject.transform;
            playerRightCamera.Target.TrackingTarget = playerObject.transform;
            turnCamera.Target.TrackingTarget = playerObject.transform;
        }
        else
            Debug.LogWarning("playerObject not assigned.");
        playerRightCamera.enabled = true;

        if (!arrowCamera) Debug.LogError("arrowCamera not assigned.");
        arrowCamera.enabled = false;

        player = playerObject.GetComponent<archery_player>();
        if (!player)
            Debug.LogError("archery_player has not been added.");

        agent = agentObject.GetComponent<archery_agent>();
        if (!agent)
            Debug.LogError("archery_agent has not been added.");

        ServiceLocator.Instance.GetService<AudioManager>().PlayBackgroundMusic("BGM_Archery");

        windFX.Initialize();

        playerPoint = 0;
        agentPoint = 0;
        winCond = settings.winningPoint;
        twoStarCond = settings.twoStarAhead;
        threeStarCond = settings.threeStarAhead;

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

        canShoot = true;

        player.Initialize();
        agent.enabled = true;
        uiHandler.init_value(settings.maxForce, settings.minForce, settings.winningPoint);

        if (!isAiTraining)
        {
            isPlayerTurn = true;
            StartCoroutine(PlayerTurn());
        }
        else
        {
            isPlayerTurn = false;
            StartCoroutine(AgentTurn());
        }
    }

    public void Shoot(Vector3 position, float force, float yaw, float pitch)
    {
        if (!canShoot) return;
        canShoot = false;
        hasHit = false;

        playerLeftCamera.enabled = false;
        playerRightCamera.enabled = false;

        arrows[currentArrow].transform.position = position;

        arrows[currentArrow].transform.rotation = Quaternion.identity;
        arrows[currentArrow].gameObject.SetActive(true);

        arrows[currentArrow].Shoot(force, yaw, pitch, windDirection, windSpeed);
        ServiceLocator.Instance.GetService<AudioManager>().PlaySFX("LaserShot", position);

        arrowCamera.Target.TrackingTarget = arrows[currentArrow].transform;
        arrowCamera.enabled = true;

        preview.Hide();
        Debug.Log($"Force: {force}\tYaw: {yaw}\tPitch: {pitch}");
    }

    private IEnumerator PlayerTurn()
    {
        playerObject.transform.position = shootPosition.position;
        agentObject.transform.position = agentIdlePosition.position;

        playerLeftCamera.Target.TrackingTarget = playerObject.transform;
        playerRightCamera.Target.TrackingTarget = playerObject.transform;
        turnCamera.Target.TrackingTarget = playerObject.transform;

        windSpeed = Random.Range(0f, settings.maxWindSpeed);
        windDirection = Random.Range(0, 360);
        if (windDirection <= 180)
        {
            windDirection = 90f;
            windFX.UpdateWind(0, windSpeed);
            uiHandler.UpdateWind(0, windSpeed);
        }
        else
        {
            windDirection = 270f;
            windFX.UpdateWind(1, windSpeed);
            uiHandler.UpdateWind(1, windSpeed);
        }

        targetDistance = Random.Range(minTargetDistance, maxTargetDistance);
        lateralDistance = Random.Range(-maxLateralDistance, maxLateralDistance);

        targetObject.transform.position = playerObject.transform.position + new Vector3(lateralDistance, 2.5f, targetDistance);

        yield return new WaitForSeconds(cameraStay);

        player.Ready();

        yield return new WaitForSeconds(3f + cameraStay);

        player.StartTurn();
        canShoot = true;
    }

    private IEnumerator AgentTurn()
    {
        playerObject.transform.position = playerIdlePosition.position;
        agentObject.transform.position = shootPosition.position;

        playerLeftCamera.Target.TrackingTarget = agentObject.transform;
        playerRightCamera.Target.TrackingTarget = agentObject.transform;
        turnCamera.Target.TrackingTarget = agentObject.transform;

        windSpeed = Random.Range(0f, settings.maxWindSpeed);
        windDirection = Random.Range(0, 360);
        if (windDirection <= 180)
        {
            windDirection = 90f;
            windFX.UpdateWind(0, windSpeed);
            uiHandler.UpdateWind(0, windSpeed);
        }
        else
        {
            windDirection = 270f;
            windFX.UpdateWind(1, windSpeed);
            uiHandler.UpdateWind(1, windSpeed);
        }

        targetDistance = Random.Range(minTargetDistance, maxTargetDistance);
        lateralDistance = Random.Range(-maxLateralDistance, maxLateralDistance);

        targetObject.transform.position = agentObject.transform.position + new Vector3(lateralDistance, 2.5f, targetDistance);

        yield return new WaitForSeconds(cameraStay);

        agent.Ready();

        yield return new WaitForSeconds(3f + cameraStay);

        agent.StartTurn();
        canShoot = true;
    }

    public void OnHit(int point)
    {
        if (hasHit) return;
        hasHit = true;

        if (isPlayerTurn)
        {
            playerPoint += point;
            isPlayerTurn = false;
        }
        else
        {
            agentPoint += point;

            if (!isAiTraining) isPlayerTurn = true;

            if (point > 0) agent.OnHit(point);
            else
            {
                agent.OnHit(0);
            }
        }
        uiHandler.set_point(playerPoint, agentPoint);

        if (point > 0) Debug.Log($"Hit: {point}");

        MiniGameOverHandler gameOverHandler = GetComponent<MiniGameOverHandler>();
        
        if (playerPoint >= winCond)
        {
            ServiceLocator.Instance.GetService<InputManager>().EnableActions(); //renable actions

            if ((playerPoint - agentPoint) < twoStarCond)
                gameOverHandler.HandleGameOver(true, 4, 1);
            else if ((playerPoint - agentPoint) < threeStarCond)
                gameOverHandler.HandleGameOver(true, 4, 2);
            else
                gameOverHandler.HandleGameOver(true, 4, 3);
        }
        else if (agentPoint >= winCond && !isAiTraining)
        {
            gameOverHandler.HandleGameOver(false);
        }

        StopAllCoroutines();
        StartCoroutine(ReturnCamera(currentArrow));

        currentArrow++;
        if (currentArrow == numArrows)
            currentArrow = 0;
        arrows[currentArrow].gameObject.SetActive(false);
    }

    public void UpdateUI(Vector3 position, float force, float yaw, float pitch)
    {
        uiHandler.set_value(force);

        if (yaw >= 0)
        {
            playerLeftCamera.enabled = false;
            playerRightCamera.enabled = true;
        }
        else
        {
            playerRightCamera.enabled = false;
            playerLeftCamera.enabled = true;
        }

        estimateLanding = preview.ShowPath(position, force, yaw, pitch, windDirection, windSpeed);
    }

    private IEnumerator ReturnCamera(int arrowIndex)
    {
        Transform position = new GameObject().transform;
        position.position = arrows[arrowIndex].transform.position;
        arrowCamera.Target.TrackingTarget = position;

        yield return new WaitForSeconds(cameraStay);

        if (isPlayerTurn) StartCoroutine(PlayerTurn()); else StartCoroutine(AgentTurn());

        arrowCamera.enabled = false;
        turnCamera.enabled = true;
        Destroy(position.gameObject);

        yield return new WaitForSeconds(cameraStay * 2f);

        turnCamera.enabled = false;
        playerRightCamera.enabled = true;
    }
}
