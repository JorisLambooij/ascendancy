using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationMoveSpeedController : MonoBehaviour
{
    [SerializeField]
    private float speedMultiplier = 1;

    private Vector3 previousPosition;
    private Animator animator;

    private void Start()
    {
        previousPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float movementDistance = Vector3.Distance(previousPosition, transform.position);
        animator.SetFloat("speed", movementDistance);
        previousPosition = transform.position;
    }
}
