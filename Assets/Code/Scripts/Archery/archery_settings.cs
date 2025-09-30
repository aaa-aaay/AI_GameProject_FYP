using UnityEngine;

[CreateAssetMenu(menuName = "Minigames/Archery/Settings")]
public class archery_settings : ScriptableObject
{
    [Header("Force")]
    [field: SerializeField, Min(5f)] public float minForce { get; private set; } = 10f;
    [field: SerializeField, Min(5f)] public float maxForce { get; private set; } = 30f;

    [Header("Aiming (degrees)")]
    [field: SerializeField, Range(0f, 45f)] public float maxYaw { get; private set; } = 30f;
    [field: SerializeField, Range(-90f, 90f)] public float minPitch { get; private set; } = 0f;
    [field: SerializeField, Range(-90f, 90f)] public float maxPitch { get; private set; } = 30f;

    [Header("Playfield")]
    [field: SerializeField] public float maxWindSpeed { get; private set; } = 30f;
    [field: SerializeField, Min(15f)] public float minTargetDistance { get; private set; } = 15f;
    [field: SerializeField, Min(15f)] public float maxTargetDistance { get; private set; } = 100f;
    [field: SerializeField, Min(0f)] public float maxLateralDistance { get; private set; } = 5f;
}
