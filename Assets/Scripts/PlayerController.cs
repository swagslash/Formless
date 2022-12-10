using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

#region player
    public Rigidbody _rigidbody;
#endregion

#region current player stats
    public float Health;
    public int BulletsInMagazine;
#endregion

#region player base stats
    public float BaseMaxHealth;
    public float BaseMovementSpeed;
    public float BaseRateOfFire;
    public float BaseDamagePerBullet;
    public int BaseMagazineSize;
    public float BaseTimeToReload;
#endregion

#region shooting

    /// The bullet gameobject to be created if shooting
    public Bullet BulletBlueprint;
    /// Position where bullets start
    public Transform BulletOrigin;
    /// Offset added to BulletOrigin
    public float BulletOffset;
    public float BulletSpeed;
    public Transform RotateContainer;
    /// Internal control variables
    public bool HasFiringCooldown = false;
    public bool IsReloading = false;
    public bool IsShooting = false;
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
    public float MaxHealth => BaseMaxHealth + BaseMaxHealth * MaxHealthModifier;
    public float MovementSpeed => BaseMovementSpeed + BaseMovementSpeed * MovementSpeedModifier;
    public float RateOfFire => BaseRateOfFire + BaseRateOfFire * RateOfFireModifier;
    public float DamagePerBullet => BaseDamagePerBullet + BaseDamagePerBullet * DamagePerBulletModifier;
    public int MagazineSize => BaseMagazineSize + MagazineSizeModifier;
    public float TimeToReload => BaseTimeToReload + BaseTimeToReload * TimeToReloadModifier;
#endregion

    void Start()
    {
        BulletsInMagazine = MagazineSize;
        Health = MaxHealth;
    }

    void Update()
    {
        if (IsShooting)
        {
            Shoot();
        }
        
        Move();
    }
    
    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        _rigidbody.velocity = (movementDirection * MovementSpeed);
    }

    void OnLookAround(InputValue value)
    {
        Vector2 inputValueVector = value.Get<Vector2>();
        var lookDirection = new Vector3(inputValueVector.x, 0, inputValueVector.y);

        var targetDirection = Vector3.RotateTowards(RotateContainer.forward, lookDirection, 1, 0.0f);
        RotateContainer.rotation = Quaternion.LookRotation(targetDirection);
    }

    void OnShoot(InputValue value)
    {
        IsShooting = value.isPressed;
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

        StartCoroutine(ResetWeaponFire());
    }

    void FireBullet() {
        var direction = RotateContainer.forward;
        var offset = BulletOffset * direction;
        var bullet = Instantiate(
            BulletBlueprint,
            BulletOrigin.position + offset,
            Quaternion.LookRotation(direction)
            );
        bullet.BulletSpeed = BulletSpeed;
        bullet.Direction = direction;
    }

    void Reload() {
        IsReloading = true;
        StartCoroutine(ResetReload());
    }

    IEnumerator ResetWeaponFire() {
        yield return new WaitForSeconds(RateOfFire);
        HasFiringCooldown = false;
    }

    IEnumerator ResetReload (){
        Debug.Log("Reload");
        yield return new WaitForSeconds(TimeToReload);
        IsReloading = false;
        BulletsInMagazine = MagazineSize;
        Debug.Log("Reloaded");
    } 

    void ConsumeItem(Item item) {
        MaxHealthModifier += item.MaxHealthModifier;
        MovementSpeedModifier += item.MovementSpeedModifier;
        RateOfFireModifier += item.RateOfFireModifier;
        DamagePerBulletModifier += item.DamagePerBulletModifier;
        MagazineSizeModifier += item.MagazineSizeModifier;
        TimeToReloadModifier += item.TimeToReloadModifier;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(BulletOrigin.position, 0.05f);
        Gizmos.DrawSphere(BulletOrigin.position + Vector3.left * BulletOffset, 0.05f);
    }
    
}
