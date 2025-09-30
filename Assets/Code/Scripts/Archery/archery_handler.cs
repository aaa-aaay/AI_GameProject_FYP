using UnityEngine;
using Unity.Cinemachine;

public class archery_handler : MonoBehaviour
{
    public static archery_handler instance;

    [Header("Cinemachine Camera")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera arrowCamera;
    [SerializeField] private GameObject playerObject;

    private archery_player player;
    [SerializeField] private archery_ui_handler uiHandler;

    [Header("Arrow")]
    [SerializeField, Tooltip("Number of arrows in object pool. Set to 0 to disable.")] private int numArrows = 3;
    private int currentArrow;
    [SerializeField] private GameObject arrowPrefab;
    private arrow[] arrows;

    private bool isPlayerTurn;
    private bool isFlying;
    private bool canShoot;

    private float windDirection;
    private float windSpeed;

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

        player = GetComponent<archery_player>();
        if (!player)
        {
            gameObject.AddComponent<archery_player>();
            Debug.LogWarning("archery_player has not been added.");
        }
        player.Init(0, 0);

        arrows = new arrow[numArrows];
        for (int i = 0; i < numArrows; i++)
        {
            arrows[i] = Instantiate(arrowPrefab).GetComponent<arrow>();
            arrows[i].gameObject.SetActive(false);
        }
        currentArrow = 0;

        isPlayerTurn = true;
        isFlying = false;
        canShoot = true;
    }

    public void Shoot(float force, float yaw, float pitch)
    {
        if (isFlying) return;
        isFlying = true;

        playerCamera.enabled = false;

        arrows[currentArrow].transform.position = playerObject.transform.position;
        arrows[currentArrow].transform.rotation = Quaternion.identity;
        arrows[currentArrow].gameObject.SetActive(true);

        arrows[currentArrow].Shoot(force, yaw, pitch, windDirection, windSpeed);

        arrowCamera.Target.TrackingTarget = arrows[currentArrow].transform;
        arrowCamera.enabled = true;

        currentArrow++;
        if (currentArrow == numArrows)
            currentArrow = 0;
        arrows[currentArrow].gameObject.SetActive(false);


        player.Init(0, 0);
        windDirection = Random.Range(0, 359);
        windSpeed = Random.Range(0, 10);
    }

    public void OnHit(int point)
    {
        isFlying = false;
        arrowCamera.enabled = false;
        playerCamera.enabled = true;

        Debug.Log($"Point: {point}");
    }

    public void UpdateUI(float force, float yaw, float pitch)
    {
        uiHandler.set_value(force, yaw, pitch, windDirection, windSpeed);
    }
}
