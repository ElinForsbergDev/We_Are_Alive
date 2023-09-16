using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius = 15f;
    [Range(0,360)] public float guardFov = 145f;
    public float guardFovSearchBoost = 40f;
    public float findDistanceWhenSeaching = 7f;

    //public GameObject playerRef;
    public patrol patrolScript;

    public LayerMask targetMask;
    public LayerMask viewMask;

    public Vector3 eyeLocation = new Vector3(0,2,0);

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            yield return wait;          // This helps with preformance as rays are not cast every frame
            FieldOfViewCheck();
        }
    }

    // Checks current view of player 
    private void FieldOfViewCheck()
    {
        float fov = guardFov;
        float seeingDistance = radius;

        if (patrolScript.currentState != patrol.State.Patrolling) {     // This makes the guard better at finding the player if it's not in its patrol state
            fov = Mathf.Clamp(guardFov + guardFovSearchBoost, 0, 360);
            seeingDistance *= 1.3f;
        }

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, seeingDistance, targetMask); // First, make a check if the player is even close

        if (rangeChecks.Length != 0) // If player is not close, do not cast any rays
        {
            Transform target = rangeChecks[0].transform; // Player transform
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            
            bool needRayTrace = true;
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            // This makes the guard see the player if it's close from behind if the guard is in searching mode
            if (distanceToPlayer < findDistanceWhenSeaching && patrolScript.currentState == patrol.State.Searching) {
                patrolScript.currentSeeingPercentage = 1;
                patrolScript.knownTargetPosition = target.position;
                patrolScript.currentState = patrol.State.Engaged;
                needRayTrace = false;
            }
            

            if (Vector3.Angle(transform.forward, directionToTarget) < fov / 2 && needRayTrace) { // Only cast rays if guard faces player

                // The different body part transforms from the player model (To cast ray to)
                Vector3 ray1 = target.GetChild(3).position;
                Vector3 ray2 = target.GetChild(4).position;
                Vector3 ray3 = target.GetChild(5).position;
                Vector3 ray4 = target.GetChild(6).position;
                Vector3 ray5 = target.GetChild(7).position;
                Vector3 ray6 = target.GetChild(8).position;

                float seePercentage = 0;

                if (!Physics.Linecast((transform.position + eyeLocation), target.position, viewMask)) {
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
                    // If the player is seen, send it to the patrol script
                    patrolScript.knownTargetPosition = target.position;
                    patrolScript.target = target;

                    if (distanceToPlayer > seeingDistance/2) { // If the player is far away give a little more time 
                        seePercentage *= 1 - ((distanceToPlayer - (seeingDistance / 2)) / seeingDistance/2);
                    }

                    patrolScript.currentSeeingPercentage = seePercentage;
                } else {
                    patrolScript.currentSeeingPercentage = 0;
                }
            } else if (needRayTrace) {
                patrolScript.currentSeeingPercentage = 0;
            }
        } else {
            patrolScript.currentSeeingPercentage = 0;
        }
        
    }

    // Think this is unused and replaced by thing in the big method
    private float isSearchingForPlayer(Vector3 playerPosition) {
        if (patrolScript.currentState == patrol.State.Searching) {
            if (Vector3.Distance(transform.position, playerPosition) < 4) {
                patrolScript.currentSeeingPercentage = 1;
            }

            return Mathf.Clamp(guardFov + guardFovSearchBoost, 0, 360);
        }

        return guardFov;
    }
}