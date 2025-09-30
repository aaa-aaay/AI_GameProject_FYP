using UnityEngine;
using Unity.Cinemachine;
using UnityEditor.Toolbars;

public class archery_handler : MonoBehaviour
{
    public static archery_handler instance;

    [Header("Cinemachine Camera")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera arrowCamera;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject targetObject;

    [Header("Arrow")]
    [SerializeField, Tooltip("Number of arrows in object pool. Set to 0 to disable.")] private int numArrows = 3;
    private int currentArrow;
    [SerializeField] private GameObject arrowPrefab;
    private arrow[] arrows;

    [Header("Settings")]
    private archery_player player;
    [SerializeField] private archery_ui_handler uiHandler;
    [field: SerializeField] public archery_settings settings { get; private set; }

    private bool isPlayerTurn;
    private bool isFlying;
    private bool canShoot;

    private float windDirection;
    private float windSpeed;

    private float minTargetDistance;
    private float maxTargetDistance;
    private float maxLateralDistance;

    private float targetDistance;
    private float lateralDistance;

    public void Awake()
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
        isFlying = false;
        canShoot = true;

        InitPlayer();
    }

    public void Shoot(float force, float yaw, float pitch)
    {
        if (!canShoot) return;
        isFlying = true;
        canShoot = false;

        playerCamera.enabled = false;

        arrows[currentArrow].transform.position = playerObject.transform.position;
        arrows[currentArrow].transform.rotation = Quaternion.identity;
        arrows[currentArrow].gameObject.SetActive(true);

        arrows[currentArrow].Shoot(force, yaw, pitch, windDirection, windSpeed);

        arrowCamera.Target.TrackingTarget = arrows[currentArrow].transform;
        arrowCamera.enabled = true;
    }

    private void InitPlayer()
    {
        windDirection = Random.Range(0f, 360f);
        windSpeed = Random.Range(0f, settings.maxWindSpeed);

        targetDistance = Random.Range(minTargetDistance, maxTargetDistance);
        lateralDistance = Random.Range(-maxLateralDistance, maxLateralDistance);

        targetObject.transform.position = playerObject.transform.position + new Vector3(lateralDistance, 1, targetDistance);

        player.Init();
    }

    public void OnHit(int point)
    {
        isFlying = false;
        //arrowCamera.enabled = false;
        //playerCamera.enabled = true;

        canShoot = true;

        currentArrow++;
        if (currentArrow == numArrows)
            currentArrow = 0;
        arrows[currentArrow].gameObject.SetActive(false);

        InitPlayer();
    }

    public void UpdateUI(float force, float yaw, float pitch)
    {
        uiHandler.set_value(force, yaw, pitch, windDirection, windSpeed, targetDistance, lateralDistance);
    }
}
