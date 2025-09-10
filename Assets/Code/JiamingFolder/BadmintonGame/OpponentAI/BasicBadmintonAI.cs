using UnityEngine;

public class BasicBadmintonAI : MonoBehaviour
{

    [Header("AI Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float offset = 1.0f;

    [Header("AI Swing")]
    [SerializeField] private float hitRange = 0.5f;
    [SerializeField] private float minHeight = 0.5f;
    [SerializeField] private float maxHeight = 3.0f;

    [Header("References")]
    [SerializeField] private Transform _shuttle;
    [SerializeField] private Transform _net;
    [SerializeField] private RacketSwing _racketSwing;


    void Update()
    {
        if (_shuttle == null || _net == null) return;

        // --- Step 1: Move behind shuttle ---
        Vector3 dirToNet = (_net.position - _shuttle.position).normalized;
        Vector3 desiredPos = _shuttle.position - dirToNet * offset;
        desiredPos.y = transform.position.y;

        // --- Prevent crossing the net (respect offset) ---
        if (transform.position.z > _net.position.z) // AI on "top" side
        {
            float minZ = _net.position.z + offset;   // stay at least 'offset' away from net
            desiredPos.z = Mathf.Max(desiredPos.z, minZ);
        }
        else // AI on "bottom" side
        {
            float maxZ = _net.position.z - offset;   // stay at least 'offset' away from net
            desiredPos.z = Mathf.Min(desiredPos.z, maxZ);
        }

        // --- Move toward corrected position ---
        transform.position = Vector3.MoveTowards(transform.position, desiredPos, moveSpeed * Time.deltaTime);



        // --- Step 2: Check if AI can hit ---
        bool inRange = Vector3.Distance(transform.position, _shuttle.position) < hitRange;
        bool inHeight = _shuttle.position.y > minHeight && _shuttle.position.y < maxHeight;

        if (inRange && inHeight)
        {
            PerformShot();
        }
    }

    private void PerformShot()
    {

        if (_racketSwing.racketSwinging) return;


        int choice = Random.Range(2, 3);


        if(choice == 0)
            _racketSwing.StartSwing(Racket.ShotType.Smash);
        else if(choice == 1)
            _racketSwing.StartSwing(Racket.ShotType.Drop);
        else
            _racketSwing.StartSwing(Racket.ShotType.Lob);
    }
}
