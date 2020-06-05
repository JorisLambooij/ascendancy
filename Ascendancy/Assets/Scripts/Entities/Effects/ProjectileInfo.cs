using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileInfo
{
    // The Initial speed at which this projectile is launched at.
    public float launchVelocity;

    // How strong should the homing effect be?
    public float targetSeekingCoefficient;

    // How long should this projectile stay in the engine?
    public float lifeTime;

    // The damage composition of this projectile
    public AttackStrength attackStrength;
}
