using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewConversionFeature", menuName = "Entity Features/Building Conversion Feature")]
public class BuildingConversionFeature : EntityFeature
{
    public EntityInfo convertedEntity;
    public string conversionAnimation;
    public float conversionDelay = 0f;

    public override void ContextMenuOption()
    {
        float delay = 0f;
        //play animation first if not null
        if (conversionAnimation != "")
        {
            Animator animator = entity.GetComponentInChildren<Animator>();

            animator.Play(conversionAnimation);
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == conversionAnimation)
                {
                    delay = clip.length;
                }
            }

            if (delay == 0f)
            {
                Debug.LogError("ERROR: Animation '" + conversionAnimation + "' not found or has length 0!");
            }
        }

        delay += conversionDelay;

        if (delay > 0f)
        {
            entity.StartCoroutine(DoAfterAnimation(delay));
        }
        else
        {
            DoAfterAnimation(0f);
        }
    }

    IEnumerator DoAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

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
