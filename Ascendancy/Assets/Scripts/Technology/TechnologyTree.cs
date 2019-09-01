using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Researchability { NotResearchable, Researchable, Researched }

public struct TechnologyTree
{
    public List<Technology> technologies { get; private set; }
    public Dictionary<int, Technology> techDictionary { get; private set; }
    public Dictionary<int, int> techProgress { get; private set; }

    private Dictionary<int, List<System.Action<int>>> techProgressSubscriptions;

    public TechnologyTree(int techCount)
    {
        technologies = new List<Technology>(techCount);
        techDictionary = new Dictionary<int, Technology>(techCount);
        techProgress = new Dictionary<int, int>(techCount);
        techProgressSubscriptions = new Dictionary<int, List<System.Action<int>>>(techCount);
    }

    public void AddTech(Technology t)
    {
        technologies.Add(t);
        techDictionary.Add(t.id, t);
        techProgress.Add(t.id, 0);
        techProgressSubscriptions.Add(t.id, new List<System.Action<int>>());
    }

    public void AddProgress(int techID, int progress)
    {
        if (progress < 0)
            return;
        int newProgress = techProgress[techID] + progress;
        newProgress = Mathf.Min(newProgress, techDictionary[techID].cost);
        techProgress[techID] = newProgress;

        foreach (System.Action<int> callback in techProgressSubscriptions[techID])
            callback(techProgress[techID]);

    }

    public void Subscribe(int techID, System.Action<int> callback)
    {
        techProgressSubscriptions[techID].Add(callback);
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
        int progress = techProgress[techID];
        return cost == progress;
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