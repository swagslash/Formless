using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Health;
    public int BulletsInMagazine;

    public readonly float BaseMaxHealth;
    public readonly float BaseMovementSpeed;
    public readonly float BaseRateOfFire;
    public readonly float BaseDamagePerBullet;
    public readonly int BaseMagazineSize;
    public readonly float BaseTimeToReload;

    // public List Items = new List<Item>();

    // Values below are calculated  when a new item is consumed
    public float MaxHealthModifier = 0;
    public float MovementSpeedModifier = 0;
    public float RateOfFireModifier = 0;
    public float DamagePerBulletModifier = 0;
    public int MagazineSizeModifier = 0;
    public float TimeToReloadModifier = 0;

    public float MaxHealth => BaseMaxHealth * MaxHealthModifier;
    public float MovementSpeed => BaseMovementSpeed * MovementSpeedModifier;
    public float RateOfFire => BaseRateOfFire * RateOfFireModifier;
    public float DamagePerBullet => BaseDamagePerBullet * DamagePerBulletModifier;
    public int MagazineSize => BaseMagazineSize + MagazineSizeModifier;
    public float TimeToReload => BaseTimeToReload * TimeToReloadModifier;

    /// Internal control variables
    public bool HasFiringCooldown = false;
    public bool IsReloading = false;


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

        if (BulletsInMagazine == 0) {
            Reload();
        }

        ResetWeaponFire();
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
