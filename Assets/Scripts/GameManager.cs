using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region setup

    public Vector3 PlayerSpawn = new Vector3(-1.5f, 0.5f, -6.5f);
    public int BaseEnemyCount = 5;
    public float BaseEnemyCountModifier = 1.25f;
    public bool Debug_NoCooldownTimer = false;
    public bool Debug_Invincible = false;

    public float SpawnProtectionDistance = 10;
    /// The player
    public PlayerController Player;

    /// Blueprints for the enemies
    public List<GameObject> EnemyBlueprints;

    /// List of enemy spawn points
    public List<GameObject> EnemySpawnPoints;

    /// List of all possible items
    public List<Item> ItemBlueprints;

    #endregion

    #region ui elements

    public CountDownUI CountDownUI;
    public NextLevelCountdownUI NextLevelCountdownUI;
    public VictoryScreen VictoryScreenUI;
    public TMPro.TextMeshProUGUI EnemyCountUI;
    public HealthbarUI healthbar;
    
    #endregion

    #region state

    public List<GameObject> Enemies = new List<GameObject>();

    public int CurrentLevel = 0;

    public bool IsFighting = false;
    public bool IsLevelClear = false;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        StartNextLevel();
    }

    private void Update()
    {
        if (Debug_Invincible)
        {
            Player.Health = 10;
        }
    }

    void GenerateEnemies()
    {
        var enemyCount = (int) (BaseEnemyCount * Mathf.Pow(BaseEnemyCountModifier, CurrentLevel - 1));
        var eligibleSpawns = EnemySpawnPoints.FindAll(p =>
            (p.transform.position - Player.transform.position).magnitude > SpawnProtectionDistance
        );
        if (!eligibleSpawns.Any())
        {
            // Fallback to all spawns if no spawns would be left
            eligibleSpawns = EnemySpawnPoints;
        }

        for (var i = 0; i < enemyCount; i++)
        {
            var spawnIndex = Random.Range(0, eligibleSpawns.Count);
            var enemySpawn = eligibleSpawns[spawnIndex].transform.position;

            var enemyIndex = Random.Range(0, EnemyBlueprints.Count);
            var enemyBlueprint = EnemyBlueprints[enemyIndex];

            // Create enemy facing towards player
            var enemy = Instantiate(
                enemyBlueprint,
                enemySpawn,
                Quaternion.identity // TODO wede: face enemy towards player
                // TODO andi: remove wede todo
            );
            enemy.GetComponent<HuntingEnemy>().target = Player.gameObject;
            enemy.SetActive(false);
            Enemies.Add(enemy);
        }
    }

    public void StartNextLevel()
    {
        StartCoroutine(StartLevel());
    }

    IEnumerator StartLevel()
    {
        CurrentLevel++;
        VictoryScreenUI.gameObject.SetActive(false);
        healthbar.gameObject.SetActive(true);
        CountDownUI.StartCountdown();
        GenerateEnemies();

        Player.gameObject.SetActive(false);

        IsFighting = false;
        IsLevelClear = false;

        if (!Debug_NoCooldownTimer)
        {
            yield return new WaitForSeconds(5);
        }

        Player.gameObject.SetActive(true);
        Player.transform.position = PlayerSpawn;
        Enemies.ForEach((enemy) => enemy.SetActive(true));
        Enemies.ForEach((enemy) => enemy.GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Force));

        IsFighting = true;

        EnemyCountUI.text = "Enemies: " + Enemies.Count;
    }

    IEnumerator LevelCleared() {
        NextLevelCountdownUI.StartCountdown();
        healthbar.gameObject.SetActive(false);

        if (!Debug_NoCooldownTimer)
        {
            yield return new WaitForSeconds(5);
        }

        Player.gameObject.SetActive(false);

        var items = GenerateRandomItems();

        // TODO: show victory screen
        VictoryScreenUI.gameObject.SetActive(true);
        VictoryScreenUI.SetWinning(true);
        VictoryScreenUI.SetItems(items[0], items[1]);
    }

    public void LevelFailed() {
        Player.gameObject.SetActive(false);
        VictoryScreenUI.gameObject.SetActive(true);
        VictoryScreenUI.SetWinning(false);
    }

    List<Item> GenerateRandomItems()
    {
        var index1 = Random.Range(0, ItemBlueprints.Count);
        var index2 = index1;
        while (index2 == index1)
        {
            index2 = Random.Range(0, ItemBlueprints.Count);
        }

        return new List<Item> { ItemBlueprints[index1], ItemBlueprints[index2] };
    }

    public void SelectItem(Item item)
    {
        Player.MaxHealthModifier += item.MaxHealthModifier;
        Player.MovementSpeedModifier += item.MovementSpeedModifier;
        Player.DamagePerBulletModifier += item.DamagePerBulletModifier;
        Player.TimeToReloadModifier += item.TimeToReloadModifier;
        Player.RateOfFireModifier += item.RateOfFireModifier;
        Player.MagazineSizeModifier += item.MagazineSizeModifier;
    }

    public void KillEnemy(GameObject enemy)
    {
        Enemies.Remove(enemy);

        if (Enemies.Count == 0) {
            IsLevelClear = true;
            IsFighting = false;
            StartCoroutine(LevelCleared());
        }

        EnemyCountUI.text = "Enemies: " + Enemies.Count;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var spawnPoint in EnemySpawnPoints)
        {
            Gizmos.DrawSphere(spawnPoint.transform.position, 0.05f);
        }
    }

    public void UpdateHealth(int health, int maxHealth)
    {
        healthbar.CurrentHealth = health;
        healthbar.MaxHealth = maxHealth;
    }
}