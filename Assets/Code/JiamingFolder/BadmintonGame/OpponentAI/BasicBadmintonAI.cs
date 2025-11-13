using System.Collections;
using UnityEngine;

public class BasicBadmintonAI : MonoBehaviour
{

    [Header("AI Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float offset = 1.0f;

    [Header("Controled Mistakes")]
    [SerializeField, Range(0f, 1f)] private float _missChance = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _smashMissChance = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _dropMissChance = 0.1f;
    [SerializeField] private float _positionErrorRange = 0.5f;
    [SerializeField] private float _minimumMovementSpeed = 0.3f;

    [Header("AI Swing")]
    [SerializeField] private float hitRange = 0.5f;
    [SerializeField] private float minHeight = 0.5f;
    [SerializeField] private float maxHeight = 3.0f;

    [Header("Racket References")]
    [SerializeField] private RacketSwing _racketSwing;
    [SerializeField] private GameObject _mainRacketGO;

    [Header("map References")]
    [SerializeField] private Transform _shuttle;
    [SerializeField] private Transform _net;
    [SerializeField] private Transform _courtCenterPoint;
    [SerializeField] private Transform _shutterLandingMarker;

    [Header("Managers")]
    [SerializeField] private BadmintionGameManager _gameManager;
    [SerializeField] private LastHitChecker _lastHitChecker;


    private ShotTypeTracker _shotTracker;
    private BadmintonStamina _stamina;

    private Vector3 _startPos;
    private bool attacking = false;
    private bool serving = false;
    private bool isWaitingForSwing = false;

    private Vector3 _lastShuttlePos;
    private Vector3 _shuttleVelocity;
    private void Start()
    {
        _startPos = transform.position;
        _gameManager.OnGameOver += HandleGameOver;
        _gameManager.OnPlayer2Score += HandleScoring;
        _gameManager.OnPlayer1Score += HandleMissing;
        _lastHitChecker.OnHitByRacker += CheckWhoHit;

        attacking = false;
        serving = true;

        _shotTracker = _shuttle.gameObject.GetComponent<ShotTypeTracker>();
        _stamina = GetComponent<BadmintonStamina>();
    }

    void Update()
    {
        if (_shuttle == null || _net == null) return;


        // --- Track shuttle linearVelocity ---
        _shuttleVelocity = (_shuttle.position - _lastShuttlePos) / Time.deltaTime;
        _lastShuttlePos = _shuttle.position;

        MoveTo(attacking);

        if (attacking || serving)
        {
            if (isWaitingForSwing || _racketSwing.racketSwinging) return;
            // --- Step 2: Check if AI can hit ---
            bool inRange = Vector3.Distance(transform.position, _shuttle.position) < hitRange;
            bool inHeight = _shuttle.position.y > minHeight && _shuttle.position.y < maxHeight;

            if (inRange && inHeight)
            {
                PerformShot();
            }

            FaceIncomingShuttle();
        }

    }


    private void MoveTo(bool attackMode)
    {
        // --- Step 1: Move behind shuttle ---

        Vector3 dirToNet = (_net.position - _shuttle.position).normalized;
        Vector3 desiredPos = Vector3.zero;

        if (attackMode) {

            desiredPos = _shutterLandingMarker.position;
            desiredPos += new Vector3(Random.Range(-_positionErrorRange, _positionErrorRange), 0f,
            Random.Range(-_positionErrorRange, _positionErrorRange));

        } 

        else desiredPos = _courtCenterPoint.position;

        if (serving) desiredPos = _shuttle.position - dirToNet * offset;

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

        float finalMoveSpeed = moveSpeed * Random.Range(_minimumMovementSpeed, 1.0f);

        Vector3 vectorToTarget = transform.position - desiredPos;
        vectorToTarget.y = 0;
        if (vectorToTarget.magnitude < 0.2f)
        {
            finalMoveSpeed = 0;
            _stamina.UseStamina(BadmintonStamina.actions.Rest);
        }
        else
        {
            _stamina.UseStamina(BadmintonStamina.actions.Running);
        }


            // --- Move toward corrected position ---
        transform.position = Vector3.MoveTowards(transform.position, desiredPos, finalMoveSpeed * Time.deltaTime);


    }

    private void FaceIncomingShuttle()
    {
        if (_shuttleVelocity.sqrMagnitude < 0.01f) return; // shuttle barely moving

        // Opposite of shuttle's linearVelocity (where it's coming from)
        Vector3 incomingDir = -_shuttleVelocity;
        incomingDir.y = 0f; // ignore tilt so AI only rotates on ground plane

        if (incomingDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(incomingDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }
    private void PerformShot()
    {

        if (_racketSwing.racketSwinging) return;




        if (_shotTracker.getShotType() == Racket.ShotType.Smash)
        {
            if (Random.value < _smashMissChance) return;
        }
        else if (_shotTracker.getShotType() == Racket.ShotType.Drop)
        {
            if (Random.value < _dropMissChance) return;
        }
        else
        {
            if (Random.value < _missChance) return;
        }


        int choice = Random.Range(0, 4);


        if(choice == 0)
            _racketSwing.StartSwing(Racket.ShotType.Smash);
        else if(choice == 1)
            _racketSwing.StartSwing(Racket.ShotType.Drop);
        else
            _racketSwing.StartSwing(Racket.ShotType.Lob);


        StartCoroutine(SwingLock());
    }

    private IEnumerator SwingLock()
    {
        isWaitingForSwing = true;
        while (_racketSwing.racketSwinging)
            yield return null;
        isWaitingForSwing = false;
    }

    private void HandleGameOver()
    {
        attacking = false;
        serving = false;
        transform.position = _startPos;
    }

    private void HandleScoring()
    {
        attacking = false;
        serving = true;
    }

    private void HandleMissing()
    {
        attacking = false;
        serving = false;
    }

    private void CheckWhoHit(GameObject lastHitRacket)
    {
        if (lastHitRacket == _mainRacketGO) {

            //Ai just hit the shutter, go to defence

            attacking = false;
            serving = false;
        }
        else
        {
            //opponent just hit the shutter, go to attack
            attacking = true;
        }
    }
}
