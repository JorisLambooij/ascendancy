using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewConversionFeature", menuName = "Entity Features/Building Conversion Feature")]
public class BuildingConversionFeature : EntityFeature
{
    public EntityInfo convertedEntity;
    public List<string> conversionAnimation;
    public float conversionDelay = 0f;

    private Animator animator;

    public override void ContextMenuOption()
    {
//TODO: align y rotation of entity slowly to 0 before conversion (better: to nearest cardinal direction);


//TODO: wait for current animation to finish animating


        //play animation first if not null
        if (conversionAnimation.Count > 0)
        {
            animator = entity.GetComponentInChildren<Animator>();

            entity.StartCoroutine(Animate(conversionAnimation));
        }
        else
            DoAfterAnimation();
    }

        IEnumerator Animate(List<string> animationQueue)
    {
        if (animationQueue.Count > 0)
        {
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animationQueue[0])
                {
                    animator.Play(animationQueue[0]);
                    //wait a bit, so we can get the correct animator state
                    yield return new WaitForSeconds(0.1f);
                    Debug.Log(animationQueue[0] + ": waiting " + clip.length + "/" + animator.GetCurrentAnimatorStateInfo(0).speed + " seconds");
                    yield return new WaitForSeconds(clip.length / animator.GetCurrentAnimatorStateInfo(0).speed);
                    break;
                }
            }
            animationQueue.RemoveAt(0);
            entity.StartCoroutine(Animate(animationQueue));

        }
        else
        {
            yield return new WaitForSeconds(conversionDelay);
            DoAfterAnimation();
        }
    }

    void DoAfterAnimation()
    {
        

        //GameManager.Instance.SwitchToMode(ControlModeEnum.buildingMode);
        BuildingPlacementMode buildingMode = GameManager.Instance.controlModeDict[ControlModeEnum.buildingMode] as BuildingPlacementMode;
        buildingMode.Building = convertedEntity;

        bool success = buildingMode.AttemptPlaceBuilding(convertedEntity, entity.transform.position);
        Debug.Log(entity.transform.position);
        if (success)
            Destroy(entity.gameObject);

        //GameObject newBuilding = Instantiate(buildingPrefab, entity.transform.parent);
    }
}
