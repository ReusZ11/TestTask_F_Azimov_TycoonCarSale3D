using UnityEngine;
using UnityEngine.AI;
using System;

public class CustomerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    private const string ANIM_WALK = "IsWalking";
    private const string ANIM_IDLE = "Idle";

    private Transform targetPosition;
    private bool isMoving = false;

    public event Action OnDestinationReached;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Transform destination)
    {
        targetPosition = destination;
        isMoving = true;

        if (agent != null)
        {
            agent.SetDestination(destination.position);
        }

        if (animator != null)
        {
            animator.SetBool(ANIM_WALK, true);
        }
    }

    private void Update()
    {
        if (isMoving && agent != null)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isMoving = false;

                if (animator != null)
                {
                    animator.SetBool(ANIM_WALK, false);
                    animator.SetBool(ANIM_IDLE, true);
                }

                OnDestinationReached?.Invoke();
            }
        }
    }
}