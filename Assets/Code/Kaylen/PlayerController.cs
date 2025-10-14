using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;
    public float gravity = 2f; 

    [Header("References")]
    public Transform playerModel;

    [Header("Tag Settings")]
    public GameObject tagHitboxPrefab;
    public float tagCooldown = 1f;
    public float tagOffset = 1f;

    private Rigidbody rb;
    private bool canTag = true;
    private GameObject activeHitbox;
    private Coroutine tagCoroutine;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

    
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

 
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;
        Vector3 move = inputDir * moveSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

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

        Debug.Log("Tag disabled");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;

        if (collision.collider.CompareTag("Tagger"))
        {
            Debug.Log("You Lose");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = false;
    }
}
