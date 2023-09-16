
using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class patrol : MonoBehaviour {

    // Different states for the guards
    public enum State {
        Patrolling,          
        Looking,
        Engaged,
        Searching
    }

    public Transform[] points; // Walk points
    private int destPoint = 0; // What point in array
    private NavMeshAgent agent;

    public Transform target;                // Player transform
    public Vector3 knownTargetPosition;     // Known player position 
    public float currentSeeingPercentage;   // How much the guard currently sees the guard

    private Animator anim;                  // Animator

    public State currentState = State.Patrolling; // The current state of the guard

    [SerializeField] float engagedRunSpeed = 11f;
    [SerializeField] private float turnSpeed = 70f;
    [SerializeField] private float timeToDetect = 1f;
    [SerializeField] private float haveSeenPlayerFor = 0f;

    private float patrollWalkSpeed;

    [Header("Guard finding player")]
    [SerializeField] private LayerMask guardMask;
    [SerializeField] private float guardCallingGuardDistance = 50f;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject indicatorPrefab;
    private GameObject indicator = null;
    

    // Variables when the guard has engaged
    [Header ("Guard searching")]
    private bool hasSeenPlayer = false;                     // Remove? Can right now
    [SerializeField] private float searchTime = 50f;
    [SerializeField] private float searchArea = 40f;
    [SerializeField] [Range(0, 1)] private float searchMovmentSpeed = 0.7f; // Multiplyer of run speed
    private float haveSearchedFor = 0f;

    [Header("Audio")]
    public float baseStepSpeed = 0.5f;
    public float sprintStepSpeed = 0.6f;

    [SerializeField] private AudioClip[] alertedClips;
    [SerializeField] private AudioClip[] lostIntrestClips;
    [SerializeField] private AudioClip[] seeingPlayerClips;
    [SerializeField] private AudioClip[] failedToSeeClips;
    [SerializeField] private AudioClip[] tauntingPlayerClips;
    [SerializeField] private AudioClip[] idleClips;

    private AudioSource audioSource;
    private float footstepTimer = 0;
    private float timerBetweenIdleClips;
    private float GetCurrentOffset => currentState == State.Engaged ? baseStepSpeed * sprintStepSpeed : baseStepSpeed; // This is for footstep sound I asume

    void Start () {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        timerBetweenIdleClips = Random.Range(15f, 40f);

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = true;
        agent.autoRepath = true;
        patrollWalkSpeed = agent.speed;
        GotoNextPoint();
    }

    // Sets the next target to be the next waypoint 
    void GotoNextPoint() {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }


    void Update () {
        // Choose the next destination point when the agent gets
        // close to the current one.

        // Based on current state. Do different things
        switch (currentState) {
            case State.Patrolling:
                patrollingStateHandler();
                break;
            case State.Looking:
                lookingStateHandler();
                break;
            case State.Engaged:
                engagedStateHandler();
                break;
            case State.Searching:
                searchingStateHandler();
                break;
            default:
                break;
        }

        // Plays footstep sound
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            GetComponent<FootSteps>().Step();
            footstepTimer = GetCurrentOffset;
        }

        // Uppdates current state of guard
        updateState();
    }

    // If the guard patrolls
    private void patrollingStateHandler() {

        agent.isStopped = false;        // The guard should move when patrolling
        agent.speed = patrollWalkSpeed; 
        anim.SetBool("Search", false);

        // Play idle clips after a random duration.
        playSoundAfterTime(idleClips);

        if (!agent.pathPending && agent.remainingDistance < 0.5f) {                         // Randomize standing looking
            GotoNextPoint();
        }

        // Transition to other state
        if (haveSeenPlayerFor > 0) {
            currentState = State.Looking;

            // Play lost interest sounds. Maybe change this to "where did he go" sounds
            audioSource.PlayOneShot(GetClipFromArray(seeingPlayerClips));                   // The Guards get a glimpse of the player
        }
    }

    // When the guard sees the player, it will stand still and look before seeing the player
    private void lookingStateHandler() {
        agent.isStopped = true;
        anim.SetBool("Look", true);

        lookAtPLayer(); // Will turn to face the player 

        // Transition to other state
        if (haveSeenPlayerFor <= 0) {
            // The guard failed to see the player completly

            // Reset state back to patrol state
            hasSeenPlayer = false;
            currentState = State.Patrolling;
            audioSource.PlayOneShot(GetClipFromArray(failedToSeeClips));    // If the guard didn't see the player
            anim.SetBool("Look", false);

            if (indicator != null) { // Remove indicator from screen
                indicator.GetComponent<StealthIndicator>().removeIndicator();
            }

        } else if (haveSeenPlayerFor >= timeToDetect) {
            // The guard have seen the player and will now engage

            currentState = State.Engaged;
            hasSeenPlayer = true;

            audioSource.PlayOneShot(GetClipFromArray(alertedClips));         // Always want to play sound here

            // Animate stealth indicator (Bounce animation)
            if (indicator != null) {
                indicator.GetComponent<StealthIndicator>().playFoundAnimation();
            }

            // Alert nearby guards 
            alertOtherGuards();
        }
    }

    // Guard will run after the player untill the player is not seen or catched
    private void engagedStateHandler() {
        anim.SetBool("Look", false);
        anim.SetBool("Engage", true);
        anim.SetBool("Search", false);
        
        agent.isStopped = false;                    // The guard should move
        agent.destination = knownTargetPosition;
        agent.speed = engagedRunSpeed;

        alertOtherGuards();     // Alert other guards about player position

        // Transition to searching state happens when the player is not directly seen by the guard
        if (currentSeeingPercentage <= 0) { 
            hasSeenPlayer = false;
            haveSeenPlayerFor = 0;
            
            if (indicator != null) { // Remove stealth indicator from screen
                indicator.GetComponent<StealthIndicator>().removeIndicator();
            }

            currentState = State.Searching;
        }
    }

    // The guard will go towards random points in a sphere around the players last known position to search for the player
    private void searchingStateHandler() {                      // Random standing att each location can be implemented
        agent.speed = engagedRunSpeed * searchMovmentSpeed;
        anim.SetBool("Engage", false);
        anim.SetBool("Look", false);
        anim.SetBool("Search", true);

        // When the guards has moved towards the randomized point it will acquire a now random point
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            agent.SetPath(randomNavmeshLocation(searchArea));
        }

        haveSearchedFor += Time.deltaTime;

        // Play taunting clips after a random duration.
        playSoundAfterTime(tauntingPlayerClips);

        // Remove indicator of guards don't see the player
        if (indicator != null && currentSeeingPercentage <= 0) {
            indicator.GetComponent<StealthIndicator>().removeIndicator();
        }

        // If guard see player, it stopps and looks
        if (currentSeeingPercentage > 0) {
            agent.isStopped = true;
            lookAtPLayer();             // Turn and look at player
        } else {
            agent.isStopped = false;
        }


        // Transition to other state
        if (haveSearchedFor >= searchTime) {
            // Reset guard to patrolling state
            haveSearchedFor = 0;
            GotoNextPoint();
            currentState = State.Patrolling;
            audioSource.PlayOneShot(GetClipFromArray(lostIntrestClips));   // Could not find player sound

        } else if (haveSeenPlayerFor >= timeToDetect) {
            hasSeenPlayer = true;
            currentState = State.Engaged;

            // Animate stealth indicator
            if (indicator != null) {
                indicator.GetComponent<StealthIndicator>().playFoundAnimation();
            }

            // If the alerting guard has played sound, don't play anything.
            if (haveSearchedFor > searchTime / 2) {
                audioSource.PlayOneShot(GetClipFromArray(alertedClips));
            }

            // Alert nearby guards 
            alertOtherGuards();
        }

    }

    // Will make guard turn and look towards the known player position
    private void lookAtPLayer() {
        Vector3 dirToLookTarget = (knownTargetPosition - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
        }
    }

    // Will randomize a new path to go towards from the last known player location with a given radius
    private NavMeshPath randomNavmeshLocation(float radius) {
        Vector3 finalPosition = Vector3.zero;
        NavMeshPath path;
        do {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += knownTargetPosition;
            NavMeshHit hit;
            finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
                finalPosition = hit.position;
            }
            path = new NavMeshPath();
        } while (!(agent.CalculatePath(finalPosition, path) && path.status == NavMeshPathStatus.PathComplete));
        return path;    // The path will always be on the navmesh for the guard
    }

    // Updates the variable for how long the guard has seen the player
    private void updateState() {
        float bonusSpotTime = currentState == State.Searching ? 3 : 1; // If the guard is in searching state the finding speed will be faster

        if (currentSeeingPercentage > 0) {
            haveSeenPlayerFor += currentSeeingPercentage * bonusSpotTime * Time.deltaTime;
        } else {
            haveSeenPlayerFor -= Time.deltaTime; // Maybe change this to be faster / slower or set a slider
        }

        // Want to update the indicator to show the value of haveSeenPlayerFor
        if (haveSeenPlayerFor > 0) {
            visibilityIndicatorUpdate();
        }

        haveSeenPlayerFor = Mathf.Clamp(haveSeenPlayerFor, 0, timeToDetect);
    }

    // Will make guards in a nearby sphere also know where the player is
    private void alertOtherGuards() {
        Collider[] otherGuards = Physics.OverlapSphere(transform.position, guardCallingGuardDistance, guardMask); // LayerMask.NameToLayer("Guard")
        foreach (Collider col in otherGuards) {
            col.GetComponent<patrol>().callFromOtherGuard(knownTargetPosition); // Make call to other guards with player location
        }
    }

    // Incoming call from other guard that sees the player
    public void callFromOtherGuard(Vector3 targetPosition) {
        if (currentSeeingPercentage <= 0 && currentState != State.Engaged) {    // If this guard is also engaged, do not change it's state. 
            //agent.ResetPath();

            // Otherwise make the guard begin searching
            agent.destination = targetPosition;             // Sets the navmesh agent to move towards the player that is seen by another guard
            knownTargetPosition = targetPosition;
            currentState = State.Searching;
        }

        haveSearchedFor = 0;  
    }

    // Gets a random soundclip from the given array
    private AudioClip GetClipFromArray(AudioClip[] clips)
    {
        return clips.Length > 0 ? clips[Random.Range(0, clips.Length)] : null;
    }

    // Plays a random sound from the given array based on time interval
    private void playSoundAfterTime(AudioClip[] clip) {
        if (timerBetweenIdleClips <= 0) {
            AudioClip getClip = GetClipFromArray(clip);
            if(getClip != null){
                audioSource.PlayOneShot(getClip);
            }
            
            timerBetweenIdleClips = Random.Range(15f, 40f);
        }
        timerBetweenIdleClips -= Time.deltaTime;
    }

    // Updates the player stealth meter in the Canvas
    private void visibilityIndicatorUpdate() {

        // Sets up the rotation
        Vector3 direction = transform.position - target.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rotation.z = -rotation.y;
        rotation.x = 0;
        rotation.y = 0;

        Vector3 north = new Vector3(0, 0, target.eulerAngles.y);

        // Create a new indicator if there is none for this guard (Not optimal but works)
        if (indicator == null) {
            indicator = Instantiate(indicatorPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), canvas.transform);
        }

        // Set the slider value based on time remaining untill beeing seen and update rotation towards guard
        indicator.GetComponent<StealthIndicator>().SetSliderValue(haveSeenPlayerFor / timeToDetect);
        indicator.GetComponent<StealthIndicator>().SetRoationToGuard(rotation * Quaternion.Euler(north));
    }

 
}