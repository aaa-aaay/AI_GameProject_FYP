using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("References")]
    public Transform playerModel;
    public TimerUI timerUI;   // reference to timer UI in scene

    [Header("Tag Settings")]
    public GameObject tagHitboxPrefab;
    public float tagCooldown = 1f;
    public float tagOffset = 1f;

    private Rigidbody rb;
    private bool canTag = true;
    private GameObject activeHitbox;
    private Coroutine tagCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // Input for tagging
        if (Input.GetMouseButtonDown(0) && canTag)
        {
            if (tagCoroutine != null)
                StopCoroutine(tagCoroutine);

            tagCoroutine = StartCoroutine(DoTag());
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;
        Vector3 move = inputDir * moveSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // Rotate toward movement direction
        if (inputDir != Vector3.zero && playerModel != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            playerModel.rotation = Quaternion.Slerp(
                playerModel.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    private IEnumerator DoTag()
    {
        if (!canTag) yield break;
        canTag = false;

        if (activeHitbox != null)
        {
            Destroy(activeHitbox);
            activeHitbox = null;
        }

        Vector3 spawnPos = playerModel.position + playerModel.forward * tagOffset;
        activeHitbox = Instantiate(tagHitboxPrefab, spawnPos, playerModel.rotation, playerModel);

        yield return new WaitForSeconds(0.1f);

        if (activeHitbox != null)
        {
            Destroy(activeHitbox);
            activeHitbox = null;
        }

        yield return new WaitForSeconds(tagCooldown);
        canTag = true;
        tagCoroutine = null;
    }

    // Called by CaptureManager to immediately disable tagging
    public void DisableTagging()
    {
        canTag = false;

        if (tagCoroutine != null)
        {
            StopCoroutine(tagCoroutine);
            tagCoroutine = null;
        }

        if (activeHitbox != null)
        {
            Destroy(activeHitbox);
            activeHitbox = null;
        }

        Debug.Log("Player tagging disabled by CaptureManager.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Tagger"))
        {
            Debug.Log("You Lose");
            // optional: handle defeat here (restart, end episode, etc.)
        }
    }
}
