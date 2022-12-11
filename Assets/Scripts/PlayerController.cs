using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
#region movement
    public Camera MainCamera;
    /// <summary>
    /// Indicates if the player uses the gamepad
    /// </summary>
    private bool isGamepadActive = true;
#endregion movement

#region player
    private Rigidbody _rigidbody;
    public PlayerStatus PlayerStatus;
#endregion

#region current player stats
    public float Health;
    public int BulletsInMagazine;
#endregion

#region player base stats
    public int BaseMaxHealth;
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
    public float MagazineSizeModifier = 0;
    public float TimeToReloadModifier = 0;
#endregion

#region calculated player stats
    public int MaxHealth => Mathf.Max(1, (int)(BaseMaxHealth + BaseMaxHealth * MaxHealthModifier));
    public float MovementSpeed => BaseMovementSpeed + BaseMovementSpeed * MovementSpeedModifier;
    public float RateOfFire => BaseRateOfFire + BaseRateOfFire * RateOfFireModifier * -1;
    public float DamagePerBullet => BaseDamagePerBullet + BaseDamagePerBullet * DamagePerBulletModifier;
    public int MagazineSize => Mathf.Max(1, (int)(BaseMagazineSize + BaseMagazineSize * MagazineSizeModifier));
    public float TimeToReload => BaseTimeToReload + BaseTimeToReload * TimeToReloadModifier;
#endregion

    void OnEnable() {
        HasFiringCooldown = false;
        IsReloading = false;
        IsShooting = false;
        BulletsInMagazine = MagazineSize;
        Health = MaxHealth;
        PlayerStatus.SetCurrentBulletsInMagazine(BulletsInMagazine);
        PlayerStatus.SetPlayerHealth(Mathf.CeilToInt(Health));
        FindObjectOfType<GameManager>().UpdateHealth(Mathf.CeilToInt(Health), MaxHealth);
        FindObjectOfType<GameManager>().UpdateAmmo(BulletsInMagazine, MagazineSize);
        Debug.Log("Active player");
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
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
        
        RotateTowardsMouse();
        ManualReloadMouse();
    }

    void RotateTowardsMouse()
    {
        if (Input.GetMouseButton(0))
        {
            isGamepadActive = false;
            
            var rawInput = Input.mousePosition;
            rawInput.z = 20; // abused as factor for rotation speed
            Vector3 clickTarget = MainCamera.ScreenToWorldPoint(rawInput);

            Vector3 lookDirection =  clickTarget - transform.position;
            lookDirection.y = 0;
            
            var targetDirection = Vector3.RotateTowards(RotateContainer.forward, lookDirection, 1, 0.0f);
            RotateContainer.rotation = Quaternion.LookRotation(targetDirection);
        }
        
        if (!isGamepadActive)
        {
            // only set if the player uses mouse
            IsShooting = Input.GetMouseButton(0);
        }
    }
    
    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        _rigidbody.velocity = (movementDirection * MovementSpeed);
    }

    void ManualReloadMouse()
    {
        if (Input.GetKeyDown(KeyCode.R) && !IsReloading)
        {
            isGamepadActive = false;
            Reload();
            // set to display "reloading" text
            PlayerStatus.SetCurrentBulletsInMagazine(BulletsInMagazine, true); 
        }
    }

    void OnReload(InputValue value)
    {
        isGamepadActive = true;
        if (value.isPressed && !IsReloading)
        {
            Reload();
            // set to display "reloading" text
            PlayerStatus.SetCurrentBulletsInMagazine(BulletsInMagazine, true); 
        }
    }
    
    void OnLookAround(InputValue value)
    {
        isGamepadActive = true;
        
        Vector2 inputValueVector = value.Get<Vector2>();

        var lookDirection = new Vector3(inputValueVector.x, 0, inputValueVector.y);

        var targetDirection = Vector3.RotateTowards(RotateContainer.forward, lookDirection, 1, 0.0f);
        RotateContainer.rotation = Quaternion.LookRotation(targetDirection);
    }

    void OnShoot(InputValue value)
    {
        isGamepadActive = true;
        IsShooting = value.isPressed;
    }

    void Shoot() {
        if (IsReloading || HasFiringCooldown) {
            return;
        }

        HasFiringCooldown = true;

        BulletsInMagazine = Mathf.Max(0, BulletsInMagazine - 1);
        FireBullet();

        FindObjectOfType<GameManager>().UpdateAmmo(BulletsInMagazine, MagazineSize);
        PlayerStatus.SetCurrentBulletsInMagazine(BulletsInMagazine);

        if (BulletsInMagazine == 0) {
            Reload();
            FindObjectOfType<GameManager>().SetReloading();
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
        bullet.Damage = DamagePerBullet;
        bullet.isOriginPlayer = true;
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
        PlayerStatus.SetCurrentBulletsInMagazine(BulletsInMagazine);
        FindObjectOfType<GameManager>().UpdateAmmo(BulletsInMagazine, MagazineSize);
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

    public void Damage(float damage)
    {
        Debug.LogWarning("Player damaged by " + damage);
        Health -= damage;
        PlayerStatus.SetPlayerHealth(Mathf.CeilToInt(Health));
        FindObjectOfType<GameManager>().UpdateHealth(Mathf.CeilToInt(Health), MaxHealth);

        if (Health <= 0) {
            FindObjectOfType<GameManager>().LevelFailed();
        }
    }
}
