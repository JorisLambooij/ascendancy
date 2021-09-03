using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationStateController : MonoBehaviour
{
    private enum AnimationState { Idle, Running, Damaged, Death };


    private Vector3 previousPosition;
    private Animator animator;
    private Entity entity;

    private Dictionary<AnimationState, RuntimeAnimatorController> animation;

    [SerializeField]
    private AnimationState animationState;
    private bool animationLocked;

    private void Start()
    {
        previousPosition = transform.position;
        animator = GetComponent<Animator>();

        animation = new Dictionary<AnimationState, RuntimeAnimatorController>();
        animation.Add(AnimationState.Idle, (RuntimeAnimatorController)Resources.Load("AnimatorControllers/1H/1H@CombatIdle", typeof(RuntimeAnimatorController)));
        animation.Add(AnimationState.Running, (RuntimeAnimatorController)Resources.Load("AnimatorControllers/1H/1H@RunForward", typeof(RuntimeAnimatorController)));
        animation.Add(AnimationState.Damaged, (RuntimeAnimatorController)Resources.Load("AnimatorControllers/1H/1H@TakeDamage", typeof(RuntimeAnimatorController)));
        animation.Add(AnimationState.Death, (RuntimeAnimatorController)Resources.Load("AnimatorControllers/MW@Death01", typeof(RuntimeAnimatorController)));

        animationState = AnimationState.Idle;
        animationLocked = false;

        entity = GetComponentInParent<Entity>();
        entity.OnTakeDamageEvent.AddListener(PlayDamageAnimation);
        entity.OnDestroyEvent.AddListener(PlayDeathAnimation);
    }

    private void Update()
    {
        if (!animationLocked)
        {
            float movementDistance = Vector3.Distance(previousPosition, transform.position);
            AnimationState newAnimationState = movementDistance < 0.002f ? AnimationState.Idle : AnimationState.Running;

            ChangeAnimationState(newAnimationState);
        }

        previousPosition = transform.position;
    }

    private void ChangeAnimationState(AnimationState newState)
    {
        if (animationLocked || newState == animationState)
            return;

        animationState = newState;
        animator.runtimeAnimatorController = animation[newState];
    }

    private void PlayDamageAnimation()
    {
        ChangeAnimationState(AnimationState.Damaged);
        StartCoroutine(ChangeAnimationStateDelayed(0.625f, AnimationState.Idle));
    }

    private IEnumerator ChangeAnimationStateDelayed(float delay, AnimationState state)
    {
        if (animationLocked)
            yield return null;

        AnimationState initialState = animationState;

        animationLocked = true;
        yield return new WaitForSeconds(delay);

        if (initialState == animationState)
        {
            animationLocked = false;
            ChangeAnimationState(state);
        }
    }

    private void PlayDeathAnimation()
    {
        animationLocked = false;
        ChangeAnimationState(AnimationState.Death);
        animationLocked = true;
        //StartCoroutine(ChangeAnimationStateDelayed(1.819795f, AnimationState.Idle));
    }
}
