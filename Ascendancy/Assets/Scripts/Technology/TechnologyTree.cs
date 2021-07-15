using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Researchability { NotResearchable, Researchable, Researched }

public struct TechnologyTree
{
    public List<Technology> technologies { get; private set; }
    public Dictionary<int, Technology> techDictionary { get; private set; }

    public SubscribableDictionary<int, float> techProgress { get; private set; }
    public Dictionary<int, Vector2> techPosition { get; private set; }
    
    public TechnologyTree(int techCount)
    {
        technologies = new List<Technology>(techCount);
        techDictionary = new Dictionary<int, Technology>(techCount);
        techProgress = new SubscribableDictionary<int, float>(techCount);
        techPosition = new Dictionary<int, Vector2>(techCount);
    }

    public void AddTech(Technology t)
    {
        technologies.Add(t);
        techDictionary.Add(t.id, t);
        techProgress.Add(t.id, 0);
    }

    /// <summary>
    /// Adds the specified amount of Research Points to the given Tech. Excess points will be returned.
    /// </summary>
    /// <param name="techID">Which Tech to add the points to.</param>
    /// <param name="progress">How many points should be added.</param>
    /// <returns>The excess points.</returns>
    public float AddProgress(int techID, float progress)
    {
        if (progress < 0)
            return 0;

        if (techProgress == null)
            Debug.LogError("Technology not initialized");

        // if more points are added than necessary, keep the additional points.
        float newProgress = techProgress.GetValue(techID) + progress;
        float overflow = 0;
        if (newProgress > techDictionary[techID].cost)
            overflow = newProgress - techDictionary[techID].cost;

        newProgress = Mathf.Min(newProgress, techDictionary[techID].cost);
        techProgress.SetValue(techID, newProgress);
        return overflow;
    }

    public List<Technology> UnlockedTechs()
    {
        List<Technology> unlockedTechs = new List<Technology>();

        foreach (Technology tech in technologies)
            if (IsTechResearched(tech.id))
                unlockedTechs.Add(tech);

        return unlockedTechs;
    }

    /// <summary>
    /// Determines whether the Technology if not available for research, or if it is already researched.
    /// </summary>
    /// <param name="techID">The ID of the Technology.</param>
    /// <returns>The Researchability of the specified Technology.</returns>
    public Researchability TechResearchability(int techID)
    {
        if (IsTechResearched(techID))
            return Researchability.Researched;
        else if (IsTechResearchable(techID))
            return Researchability.Researchable;
        else
            return Researchability.NotResearchable;
    }

    /// <summary>
    /// Is the specified Technology already researched?
    /// </summary>
    /// <param name="techID">The ID of the Technology.</param>
    /// <returns>True if tech if fully researched, false otherwise.</returns>
    private bool IsTechResearched(int techID)
    {
        int cost = techDictionary[techID].cost;
        float progress = techProgress.GetValue(techID);
        return cost <= progress;
    }

    /// <summary>
    /// Is the specified Technology available for research?
    /// </summary>
    /// <param name="techID">The ID of the Technology.</param>
    /// <returns>True if all dependencies are researched AND the technology is not already researched. False otherwise.</returns>
    private bool IsTechResearchable(int techID)
    {
        if (IsTechResearched(techID))
            return false;

        foreach (int dependency in techDictionary[techID].dependencies)
            if (!IsTechResearched(dependency))
                return false;

        return true;
    }
}