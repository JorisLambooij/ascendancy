using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GOPool))]
public class RecruitmentMenuCategory : MenuCategory
{
    protected GOPool pool;

    public Entity SelectedRecruiter;

    void Awake()
    {
        pool = GetComponent<GOPool>();
    }

    protected override void Start()
    {
        base.Start();

        optionsGO = this.gameObject;
        pool = GetComponent<GOPool>();

        // Assign this as reference to all the options in this category
        RecruitmentOption[] recruitmentOptions = transform.GetComponentsInChildren<RecruitmentOption>();
        foreach (RecruitmentOption option in recruitmentOptions)
            option.Category = this;
    }

    public void SelectRecruiter(Entity recruiter)
    {
        this.SelectedRecruiter = recruiter;

        RecruitmentFeature recruitmentFeature = SelectedRecruiter.FindFeature<RecruitmentFeature>();
        if (recruitmentFeature != null)
            SetOptions(recruitmentFeature);
    }

    public void SetOptions(RecruitmentFeature recruitmentFeature)
    {
        Debug.Log("pool " + pool);
        pool.Generate(recruitmentFeature.recruitableUnits.Count);

        int i = 0;
        foreach (GameObject optionGO in pool.pool)
        {
            RecruitmentOption option = optionGO.GetComponent<RecruitmentOption>();
            option.Unit = recruitmentFeature.recruitableUnits[i];
            Debug.Log("Set Category");
            option.Category = this;

            i++;
        }
    }
}
