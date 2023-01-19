using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Units.Player
{
    // Will always add a NavMesh
    [RequireComponent(typeof(NavMeshAgent), typeof(NavMeshObstacle))]

    public class HandlePlayerUnit : MonoBehaviour
    {
        [SerializeField] private float carvingTime = .5f;
        [SerializeField] private float carvingMoveThreshold = .1f;

        private NavMeshAgent navMeshAgent;
        private NavMeshObstacle navMeshObstacle;

        private float lastMoveTime;
        private Vector3 lastPosition;

        private void Awake()
        {
            // When game start, give reference
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshObstacle = GetComponent<NavMeshObstacle>();

            navMeshObstacle.enabled = false;
            navMeshObstacle.carveOnlyStationary = false;
            navMeshObstacle.carving = true;

            lastPosition = transform.position;
        }

        private void Update()
        {
            // Check if we have moved far enough
            if(Vector3.Distance(lastPosition, transform.position) > carvingMoveThreshold)
            {
                lastMoveTime = Time.time;
                lastPosition = transform.position;
            }

            if(lastMoveTime + carvingTime < Time.time)
            {
                navMeshAgent.enabled = false;
                navMeshObstacle.enabled = true;
            }
        }

        public void SetDestination(Vector3 destination)
        {
            navMeshObstacle.enabled = false;

            lastMoveTime = Time.time;
            lastPosition = transform.position;

            StartCoroutine(MoveUnit(destination));
        }

        private IEnumerator MoveUnit(Vector3 destination)
        {
            // To wait a frame or there is shifting
            yield return null;

            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(destination);
        }
    }
}

