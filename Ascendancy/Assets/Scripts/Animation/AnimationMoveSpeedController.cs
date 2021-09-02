using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationMoveSpeedController : MonoBehaviour
{
    private enum AnimationState { Idle, Running };


    private Vector3 previousPosition;
    private Animator animator;

    private Dictionary<AnimationState, RuntimeAnimatorController> animation;
    private AnimationState animationState;

    private void Start()
    {
        previousPosition = transform.position;
        animator = GetComponent<Animator>();

        animation = new Dictionary<AnimationState, RuntimeAnimatorController>();
        animation.Add(AnimationState.Idle, (RuntimeAnimatorController)Resources.Load("AnimatorControllers/1H/1H@CombatIdle", typeof(RuntimeAnimatorController)));
        animation.Add(AnimationState.Running, (RuntimeAnimatorController)Resources.Load("AnimatorControllers/1H/1H@RunForward", typeof(RuntimeAnimatorController)));

        animationState = AnimationState.Idle;
    }

    private void Update()
    {
        float movementDistance = Vector3.Distance(previousPosition, transform.position);
        AnimationState newAnimationState = movementDistance < 0.002f ? AnimationState.Idle : AnimationState.Running;

        if (newAnimationState != animationState)
            animator.runtimeAnimatorController = animation[newAnimationState];

        animationState = newAnimationState;
        previousPosition = transform.position;
    }
}
