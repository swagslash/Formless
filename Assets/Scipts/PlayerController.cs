using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

#region current player stats
    public float Health;
    public int BulletsInMagazine;
#endregion

 #region player base stats
    public readonly float BaseMaxHealth;
    public readonly float BaseMovementSpeed;
    public readonly float BaseRateOfFire;
    public readonly float BaseDamagePerBullet;
    public readonly int BaseMagazineSize;
    public readonly float BaseTimeToReload;
#endregion

#region shooting

    /// The bullet gameobject to be created if shooting
    public Bullet BulletBlueprint;
    /// Position where bullets start
    public Transform BulletOrigin;
    /// Offset added to BulletOrigin
    public Vector3 BulletOffset;
    public float BulletSpeed;
    /// Internal control variables
    public bool HasFiringCooldown = false;
    public bool IsReloading = false;
#endregion

#region stat modifiers
    // Values below are calculated  when a new item is consumed
    public float MaxHealthModifier = 0;
    public float MovementSpeedModifier = 0;
    public float RateOfFireModifier = 0;
    public float DamagePerBulletModifier = 0;
    public int MagazineSizeModifier = 0;
    public float TimeToReloadModifier = 0;
#endregion

#region calculated player stats
    public float MaxHealth => BaseMaxHealth * MaxHealthModifier;
    public float MovementSpeed => BaseMovementSpeed * MovementSpeedModifier;
    public float RateOfFire => BaseRateOfFire * RateOfFireModifier;
    public float DamagePerBullet => BaseDamagePerBullet * DamagePerBulletModifier;
    public int MagazineSize => BaseMagazineSize + MagazineSizeModifier;
    public float TimeToReload => BaseTimeToReload * TimeToReloadModifier;
#endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move() {

    }

    void Shoot() {
        if (IsReloading || HasFiringCooldown) {
            return;
        }

        HasFiringCooldown = true;

        BulletsInMagazine--;
        FireBullet();

        if (BulletsInMagazine == 0) {
            Reload();
        }

        ResetWeaponFire();
    }

    void FireBullet() {
        var direction = Vector3.left;// TODO: take rotation/direction from player-input
        var bullet = Instantiate(
            BulletBlueprint,
            BulletOrigin.position + BulletOffset,
            Quaternion.LookRotation(direction)
            );
        bullet.BulletSpeed = BulletSpeed;
        bullet.Direction = direction;
    }

    void Reload() {
        IsReloading = true;
        ResetReload();
    }

    IEnumerator ResetWeaponFire() {
        yield return new WaitForSeconds(RateOfFire);
        HasFiringCooldown = false;
    }

    IEnumerator ResetReload (){
        yield return new WaitForSeconds(TimeToReload);
        IsReloading = false;
        BulletsInMagazine = MagazineSize;
    } 

    void ConsumeItem(Item item) {
        MaxHealthModifier += item.MaxHealthModifier;
        MovementSpeedModifier += item.MovementSpeedModifier;
        RateOfFireModifier += item.RateOfFireModifier;
        DamagePerBulletModifier += item.DamagePerBulletModifier;
        MagazineSizeModifier += item.MagazineSizeModifier;
        TimeToReloadModifier += item.TimeToReloadModifier;
    }
}
