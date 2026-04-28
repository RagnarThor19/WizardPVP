using UnityEngine;

public interface ICombatTarget : IDamageable, IHealable
{
    GameObject TargetGameObject { get; }
    Transform TargetTransform { get; }
    TeamId Team { get; }
}
