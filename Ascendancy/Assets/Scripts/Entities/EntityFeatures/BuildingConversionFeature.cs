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
    private BuildingPlacementMode buildingMode;

    private bool abort = false;

    public override void ContextMenuOption()
    {
        abort = false;

        buildingMode = GameManager.Instance.controlModeDict[ControlModeEnum.buildingMode] as BuildingPlacementMode;
        buildingMode.Building = convertedEntity;

        buildingMode.StartOrderMode(this);
        GameManager.Instance.SwitchToMode(ControlModeEnum.buildingMode);
    }

    public void BuildAt(Vector3 position)
    {

        MoveOrder movOrder;

        movOrder = new MoveOrder(entity, position);

        entity.IssueOrder(movOrder, false);

        entity.StartCoroutine(WaitForMovement(movOrder));

        
    }

    IEnumerator WaitForMovement(UnitOrder order)
    {
        while (!order.Fulfilled)
        {
            //Debug.Log("waiting for movement...");

            //cancel order if entity receives new order
            if (entity.Controller.currentOrder != order)
                if (entity.Controller.currentOrder == null)
                {
                    Debug.LogError("New order for " + entity.entityInfo.name + " detected. New order was NULL!");
                }
                else
                {
                    Debug.Log("New " + entity.Controller.currentOrder.GetType().ToString() + "order for " + entity.entityInfo.name + " detected. Aborting building conversion!");
                    abort = true;
                    yield break;
                }

            //wait
            yield return new WaitForSeconds(0.2f);
            //Debug.Log("Order " + order.CurrentDestination + " is fullfilled = " + order.Fulfilled);
            //Debug.Log("Current order: " + entity.Controller.currentOrder.GetType().Name);

        }
        Debug.Log("movement finished");

        if (abort)
            yield break;


        RotateOrder rotOrder;

        rotOrder = new RotateOrder(entity, new Vector3(0, 0, 1));

        entity.IssueOrder(rotOrder, false);

        entity.StartCoroutine(WaitForRotation(rotOrder));
    }

    IEnumerator WaitForRotation(UnitOrder order)
    {
        while (!order.Fulfilled)
        {
            //Debug.Log("waiting for rotation...");

            //cancel order if entity receives new order
            if (entity.Controller.currentOrder != order)
                if (entity.Controller.currentOrder == null)
                {
                    Debug.LogError("New order for " + entity.entityInfo.name + " detected. New order was NULL!");
                }
                else
                {
                    Debug.Log("New " + entity.Controller.currentOrder.GetType().ToString() + "order for " + entity.entityInfo.name + " detected. Aborting building conversion!");
                    abort = true;
                    yield break;
                }

            //wait
            yield return new WaitForSeconds(0.2f);
            //Debug.Log("Order " + order.CurrentDestination + " is fullfilled = " + order.Fulfilled);
            //Debug.Log("Current order: " + entity.Controller.currentOrder.GetType().Name);

        }
        Debug.Log("rotation finished");



        //play animation first if not null
        if (conversionAnimation.Count > 0)
        {
            animator = entity.GetComponentInChildren<Animator>();

            entity.StartCoroutine(Animate(conversionAnimation));
        }
        else if (!abort)
            DoAfterAnimation();
    }

    IEnumerator Animate(List<string> animationQueue)
    {
        if (abort)
        {
            yield break;
        }

        entity.Controller.lockEntity = true;    //we do not need to undo this, since this entity will be replaced

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
        if (abort)
        {
            entity.Controller.lockEntity = false;
            return;
        }

        //GameManager.Instance.SwitchToMode(ControlModeEnum.buildingMode);
        BuildingPlacementMode buildingMode = GameManager.Instance.controlModeDict[ControlModeEnum.buildingMode] as BuildingPlacementMode;
        buildingMode.Building = convertedEntity;

        bool success = entity.Owner.AttemptPlaceBuilding(convertedEntity, entity.transform.position);
        Debug.Log(entity.transform.position);
        if (success)
            Destroy(entity.gameObject);

        //GameObject newBuilding = Instantiate(buildingPrefab, entity.transform.parent);
    }
}
