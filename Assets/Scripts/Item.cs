using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite Icon;

    /// % of max-health increae/decrease
    public float MaxHealthModifier = 0;
    /// % of movement-speed increase/decrease
    public float MovementSpeedModifier = 0;
    /// % of rate-of-fire increase/decrease
    public float RateOfFireModifier = 0;
    /// % of damage-per-bullet increase/decrease
    public float DamagePerBulletModifier = 0;
    /// Absolute magazine-size increase/decrease
    public int MagazineSizeModifier = 0;
    /// % of time-to-reload increase/decrease
    public float TimeToReloadModifier = 0;
}
