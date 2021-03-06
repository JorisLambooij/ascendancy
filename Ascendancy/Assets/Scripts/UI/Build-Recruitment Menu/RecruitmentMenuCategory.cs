﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GOPool))]
public class RecruitmentMenuCategory : MenuCategory
{
    [SerializeField]
    protected GOPool pool;

    public Entity SelectedRecruiter;
    
    protected override void Start()
    {
        base.Start();

        optionsGO = this.gameObject;

        // Assign this as reference to all the options in this category
        RecruitmentOption[] recruitmentOptions = transform.GetComponentsInChildren<RecruitmentOption>();
        foreach (RecruitmentOption option in recruitmentOptions)
            option.Category = this;
    }

    public void SelectRecruiter(Entity recruiter)
    {
        Debug.Assert(recruiter != null, "ERROR: Recruiter Entity was null");
        this.SelectedRecruiter = recruiter;

        if (recruiter != null && recruiter.entityInfo.thumbnail != null)
            GetComponent<Image>().sprite = recruiter.entityInfo.thumbnail;

        RecruitmentFeature recruitmentFeature = SelectedRecruiter.FindFeature<RecruitmentFeature>();
        if (recruitmentFeature != null)
            SetOptions(recruitmentFeature);
    }

    public void SetOptions(RecruitmentFeature recruitmentFeature)
    {
        pool.Generate(recruitmentFeature.recruitableUnits.Count);

        int i = 0;
        foreach (GameObject optionGO in pool.pool)
        {
            RecruitmentOption option = optionGO.GetComponent<RecruitmentOption>();
            option.Unit = recruitmentFeature.recruitableUnits[i];
            option.Category = this;
            i++;
        }
    }
}
