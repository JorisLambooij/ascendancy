using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentOption : MonoBehaviour
{
    public RecruitmentMenuCategory Category;

    private EntityInfo unit;
    public EntityInfo Unit
    {
        get => unit;
        set
        {
            unit = value;
            UpdateThumbnail();
        }
    }

    protected GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        UpdateThumbnail();
    }

    public void RecruitUnit()
    {
        if (Category.SelectedRecruiter == null)
        {
            Debug.LogWarning("No Entity selected!");
            return;
        }
        
        RecruitmentFeature recruitmentF = Category.SelectedRecruiter.FindFeature<RecruitmentFeature>();
        Debug.Assert(recruitmentF != null, "Selected Entity cannot recruit!");
        recruitmentF.AddToQueue(Unit);
    }

    protected void UpdateThumbnail()
    {
        GetComponent<Image>().sprite = Unit.Thumbnail;
    }
}
