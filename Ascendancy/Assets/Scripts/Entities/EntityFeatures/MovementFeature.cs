using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewMovementFeature", menuName = "Entity Features/Movement Feature", order = 0)]
public class MovementFeature : EntityFeature
{
    public float speed;

    public float turnSpeed;

    public UnitController unitController { get; set; }

    public override void UpdateOverride(Entity entity)
    {
        base.UpdateOverride(entity);
    }
    
}
