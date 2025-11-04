using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float holdMoveSpeed = 2.5f; // slower speed while holding a rabbit
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;
    public float gravity = 2f;

    [Header("References")]
    public Transform playerModel;

    [Header("Tag Settings")]
    public GameObject tagHitboxPrefab;
    public float tagCooldown = 1f;
    public float tagOffset = 1f;

    [Header("Health Settings")]
    public List<Image> heartsUI;
    public Sprite fullHeartSprite;
    public Sprite lostHeartSprite;
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Damage Settings")]
    public float invincibilityTime = 3f;
    private bool isInvincible = false;

    private Rigidbody rb;
    private bool canTag = true;
    private GameObject activeHitbox;
    private Coroutine tagCoroutine;
    public bool isGrounded;

    // Runner pickup
    private Runner heldRunner = null;
    public Transform holdPoint;
    public bool isHoldingRunner { get; private set; } = false;

    private Coroutine holdReleaseCoroutine;

    void Awake()
    {
        currentHealth = maxHealth;

        for (int i = 0; i < heartsUI.Count; i++)
        {
            if (i < maxHealth)
                heartsUI[i].sprite = fullHeartSprite;
            else
                heartsUI[i].gameObject.SetActive(false);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (playerModel == null) Debug.LogWarning("PlayerMovement: playerModel not assigned.");

        if (holdPoint == null)
        {
            holdPoint = new GameObject("HoldPoint").transform;
            holdPoint.SetParent(playerModel, true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isHoldingRunner)
            {
                DropRunner();
            }
            else
            {
                if (canTag)
                {
                    if (tagCoroutine != null) StopCoroutine(tagCoroutine);
                    tagCoroutine = StartCoroutine(DoTag());
                }
            }
        }
    }

    void FixedUpdate()
    {
        Move();

        if (!isGrounded)
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);

        if (isHoldingRunner && heldRunner != null)
        {
            heldRunner.transform.position = holdPoint.position;
            heldRunner.transform.rotation = holdPoint.rotation;
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;
        float speed = isHoldingRunner ? holdMoveSpeed : moveSpeed;
        Vector3 move = inputDir * speed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        if (inputDir != Vector3.zero && playerModel != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator DoTag()
    {
        if (!canTag) yield break;
        canTag = false;
        FindAnyObjectByType<GrabUI>()?.StartCooldownUI(tagCooldown);

        if (activeHitbox != null)
        {
            Destroy(activeHitbox);
            activeHitbox = null;
        }

        if (playerModel == null)
        {
            Debug.LogError("DoTag: playerModel is null, cannot spawn hitbox.");
            canTag = true;
            yield break;
        }

        Vector3 spawnPos = playerModel.position + playerModel.forward * tagOffset;
        Quaternion spawnRot = playerModel.rotation;

        activeHitbox = Instantiate(tagHitboxPrefab, spawnPos, spawnRot);
        TagHitbox th = activeHitbox.GetComponent<TagHitbox>();
        if (th != null) th.owner = this;

        activeHitbox.transform.SetParent(playerModel, true);

        yield return new WaitForSeconds(tagCooldown);

        if (activeHitbox != null)
        {
            Destroy(activeHitbox);
            activeHitbox = null;
        }

        canTag = true;
        tagCoroutine = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;

        if (collision.collider.CompareTag("Tagger"))
        {
            TakeDamage();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = false;
    }

    public void PickUpRunner(Runner runner)
    {
        if (runner == null || isHoldingRunner) return;

        holdPoint.rotation = playerModel.rotation;

        Collider runCol = runner.GetComponent<Collider>();
        Rigidbody runRb = runner.GetComponent<Rigidbody>();
        Runner runAgent = runner.GetComponent<Runner>();

        if (runRb != null) runRb.isKinematic = true;
        if (runCol != null) runCol.enabled = false;
        if (runAgent != null) runAgent.enabled = false;

        heldRunner = runner;
        isHoldingRunner = true;

        runner.transform.SetParent(holdPoint, true);
        runner.transform.localPosition = Vector3.zero;
        runner.transform.localRotation = Quaternion.identity;

        // Start auto-release timer
        if (holdReleaseCoroutine != null) StopCoroutine(holdReleaseCoroutine);
        holdReleaseCoroutine = StartCoroutine(AutoReleaseRunner());

        Debug.Log($"Picked up runner: {runner.name}");
    }

    private IEnumerator AutoReleaseRunner()
    {
        float duration = Random.Range(5f, 10f);
        yield return new WaitForSeconds(duration);

        if (isHoldingRunner)
        {
            Debug.Log("Runner broke free!");
            DropRunner();
        }
    }

    public void DropRunner()
    {
        if (!isHoldingRunner || heldRunner == null) return;

        Collider runCol = heldRunner.GetComponent<Collider>();
        Rigidbody runRb = heldRunner.GetComponent<Rigidbody>();
        Runner runAgent = heldRunner.GetComponent<Runner>();

        if (runCol != null) runCol.enabled = true;
        if (runRb != null) runRb.isKinematic = false;
        if (runAgent != null) runAgent.enabled = true;

        heldRunner.transform.SetParent(null, true);
        heldRunner.transform.position = transform.position + transform.forward * 1.5f + Vector3.up * 0.1f;

        heldRunner = null;
        isHoldingRunner = false;

        if (holdReleaseCoroutine != null)
        {
            StopCoroutine(holdReleaseCoroutine);
            holdReleaseCoroutine = null;
        }
    }

    public Runner GetHeldRunner()
    {
        return heldRunner != null ? heldRunner.GetComponent<Runner>() : null;
    }

    private void TakeDamage()
    {
        if (currentHealth <= 0 || isInvincible) return;

        currentHealth--;
        if (currentHealth < heartsUI.Count)
            heartsUI[currentHealth].sprite = lostHeartSprite;

        StartCoroutine(InvincibilityCoroutine());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        this.enabled = false;
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
}



