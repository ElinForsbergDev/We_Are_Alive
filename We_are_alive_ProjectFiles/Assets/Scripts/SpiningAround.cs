using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiningAround : MonoBehaviour {
    public enum State {
        Spinning,
        Looking,
        Engaged,
        Searching
    }

    [Header("Random Controlls")]
    [SerializeField] private GameObject obj;
    [SerializeField] private Vector3 vector;
    [SerializeField] private LayerMask viewMask;
    [SerializeField] private LayerMask guardMask;
    [SerializeField] private Vector3 eyeLocation = new Vector3(0, 24, 0);

    [Header("Information")]
    [SerializeField] private Vector3 knownTargetPosition;
    [SerializeField] private float currentSeeingPercentage = 0;
    [SerializeField] private float haveSeenPlayerFor = 0f;
    private bool hasSeenPlayer = false;

    [Header("State system variables")]
    [SerializeField] private State currentState = State.Spinning;
    [SerializeField] private float timeToDetect = 1f;
    [SerializeField] private float engagedTurnSpeed = 40f;
    [SerializeField] private float searchTime = 50f;
    [SerializeField] private float searchArea = 15f;
    private float haveSearchedFor = 0f;
    [SerializeField] private float guardCallingGuardDistance = 50f;

    [Header("Player stealth indicator")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject indicatorPrefab;
    private GameObject indicator = null;


    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {

        switch (currentState) {
            case State.Spinning:
                spinningStateHandler();
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

        updateState();
    }


    private void spinningStateHandler() {

        obj.transform.Rotate(vector * Time.deltaTime);

        // Transition to other state
        if (haveSeenPlayerFor > timeToDetect / 3) {
            currentState = State.Looking;
        }
    }

    private void lookingStateHandler() {

        Vector3 dirToLookTarget = (knownTargetPosition - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, vector.y * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
        }

        // Transition to other state
        if (haveSeenPlayerFor <= 0) {
            hasSeenPlayer = false;
            currentState = State.Spinning;
        } else if (haveSeenPlayerFor >= timeToDetect) {
            currentState = State.Engaged;
            hasSeenPlayer = true;

            // Animate stealth indicator
            if (indicator != null) {
                indicator.GetComponent<StealthIndicator>().playFoundAnimation();
            }

            // Alert nearby guards 
            alertOtherGuards();
        }
    }

    private void engagedStateHandler() {

        Vector3 dirToLookTarget = (knownTargetPosition - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, engagedTurnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
        }

        // Transition to other state
        if (currentSeeingPercentage <= 0) {
            hasSeenPlayer = false;
            haveSeenPlayerFor = 0;
            currentState = State.Searching;
        }
    }

    private void searchingStateHandler() {

        haveSearchedFor += Time.deltaTime;

        transform.eulerAngles += (Vector3.up * searchArea * Mathf.Sin(haveSearchedFor + Mathf.PI/2) * Time.deltaTime);

        // Transition to other state
        if (haveSearchedFor >= searchTime) {
            haveSearchedFor = 0;
            currentState = State.Spinning;
        } else if (haveSeenPlayerFor >= timeToDetect) {
            hasSeenPlayer = true;
            currentState = State.Engaged;
            
            // Animate stealth indicator
            if (indicator != null) {
                indicator.GetComponent<StealthIndicator>().playFoundAnimation();
            }
        }

    }

    private void updateState() {
        float bonusSpotTime = currentState == State.Searching ? 3 : 1; // If the tower is in searching state the finding speed will be faster

        if (currentSeeingPercentage > 0) {
            haveSeenPlayerFor += currentSeeingPercentage * bonusSpotTime * Time.deltaTime;
        } else {
            haveSeenPlayerFor -= Time.deltaTime; // Maybe change this to be faster / slower or set a slider
        }

        haveSeenPlayerFor = Mathf.Clamp(haveSeenPlayerFor, 0, timeToDetect);

    }

    private void alertOtherGuards() {
        Collider[] otherGuards = Physics.OverlapSphere(transform.position, guardCallingGuardDistance, guardMask);
        foreach (Collider col in otherGuards) {
            col.GetComponent<patrol>().callFromOtherGuard(knownTargetPosition);
        }
    }

    private void OnTriggerStay(Collider other) {
        float seePercentage = 0;

        if (other.tag == "Player") {

            Vector3 ray1 = other.transform.GetChild(3).position;
            Vector3 ray2 = other.transform.GetChild(4).position;
            Vector3 ray3 = other.transform.GetChild(5).position;
            Vector3 ray4 = other.transform.GetChild(6).position;
            Vector3 ray5 = other.transform.GetChild(7).position;
            Vector3 ray6 = other.transform.GetChild(8).position;

            RaycastHit hit;

            if (!Physics.Linecast((transform.position + eyeLocation), other.transform.position, out hit, viewMask)) {
                seePercentage += 0.2f;
            }
            if (!Physics.Linecast((transform.position + eyeLocation), ray1, viewMask)) {
                seePercentage += 0.15f;
            }
            if (!Physics.Linecast((transform.position + eyeLocation), ray2, viewMask)) {
                seePercentage += 0.1f;
            }
            if (!Physics.Linecast((transform.position + eyeLocation), ray3, viewMask)) {
                seePercentage += 0.1f;
            }
            if (!Physics.Linecast((transform.position + eyeLocation), ray4, viewMask)) {
                seePercentage += 0.25f;
            }
            if (!Physics.Linecast((transform.position + eyeLocation), ray5, viewMask)) {
                seePercentage += 0.1f;
            }
            if (!Physics.Linecast((transform.position + eyeLocation), ray6, viewMask)) {
                seePercentage += 0.1f;
            }

            if (seePercentage > 0) {
                knownTargetPosition = other.transform.position;
            }
        }

        currentSeeingPercentage = seePercentage;
        visibilityIndicatorUpdate(other.transform);
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            currentSeeingPercentage = 0;
            knownTargetPosition = other.transform.position;
            haveSeenPlayerFor = 0;

            // Remove indicator
            if (indicator != null) {
                indicator.GetComponent<StealthIndicator>().removeIndicator();
            }
        }
    }

    private void visibilityIndicatorUpdate(Transform target) {
        Vector3 direction = transform.position - target.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rotation.z = -rotation.y;
        rotation.x = 0;
        rotation.y = 0;

        Vector3 north = new Vector3(0, 0, target.eulerAngles.y);

        if (indicator == null) {
            indicator = Instantiate(indicatorPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), canvas.transform);
            indicator.transform.SetAsFirstSibling();
        }

        if (currentState == State.Searching && indicator != null && currentSeeingPercentage <= 0) {
            // Remove indicator
            indicator.GetComponent<StealthIndicator>().removeIndicator();

        } else if (haveSeenPlayerFor <= 0) {
            // Remove indicator
            indicator.GetComponent<StealthIndicator>().removeIndicator();
        } else {
            indicator.GetComponent<StealthIndicator>().SetSliderValue(haveSeenPlayerFor / timeToDetect);
            indicator.GetComponent<StealthIndicator>().SetRoationToGuard(rotation * Quaternion.Euler(north));
        }
    }
}
