using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

#region setup
    public int BaseEnemyCount = 15;
    public float BaseEnemyCountModifier = 1.25f;

    /// The player
    public PlayerController Player;
    /// Blueprints for the enemies
    public List<GameObject> EnemyBlueprints;
    /// List of enemy spawn points
    public List<Vector3> EnemySpawnPoints;
    /// List of all possible items
    public List<Item> ItemBlueprints;
#endregion

#region ui elements
    public CountDownUI CountDownUI;
    public VictoryScreen VictoryScreenUI;
    public TMPro.TextMeshProUGUI EnemyCountUI;
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

    // Update is called once per frame
    void Update()
    {
        
    } 

    void GenerateEnemies() {
        var enemyCount = (int) (BaseEnemyCount * BaseEnemyCountModifier);
        for (int i = 0; i < enemyCount; i++) {
            var spawnIndex = Random.Range(0, EnemySpawnPoints.Count);
            var enemySpawn = EnemySpawnPoints[spawnIndex];

            var enemyIndex = Random.Range(0, EnemyBlueprints.Count);
            var enemyBlueprint = EnemyBlueprints[enemyIndex];

            // Create enemy facing towards player
            var enemy = Instantiate(
                enemyBlueprint,
                enemySpawn,
                Quaternion.identity // TODO wede: face enemy towards player
            );
            enemy.GetComponent<HuntingEnemy>().target = Player.gameObject;
            enemy.SetActive(false);
            Enemies.Add(enemy);
        }
    }

    public void StartNextLevel() {
        StartCoroutine(StartLevel());
    }

    IEnumerator StartLevel() {
        CurrentLevel++;
        VictoryScreenUI.gameObject.SetActive(false);
        CountDownUI.StartCountdown();
        GenerateEnemies();

        Player.gameObject.SetActive(false);

        IsFighting = false;
        IsLevelClear = false;

        yield return new WaitForSeconds(5);

        Player.gameObject.SetActive(true);
        Enemies.ForEach((enemy) => enemy.SetActive(true));

        IsFighting = true;

        EnemyCountUI.text = "Enemies: " + Enemies.Count;
    }

    void LevelCleared() {
        IsLevelClear = true;
        IsFighting = false;

        Player.gameObject.SetActive(false);

        // TODO: check that distinct items are generated
        var left = GenerateRandomItem();
        var right = GenerateRandomItem();

        // TODO: show victory screen
        VictoryScreenUI.gameObject.SetActive(true);
        VictoryScreenUI.SetItems(left, right);
    }

    Item GenerateRandomItem() {
        var index = Random.Range(0, ItemBlueprints.Count);
        return ItemBlueprints[index];
    }

    public void SelectItem(Item item) {
        Player.MaxHealthModifier += item.MaxHealthModifier;
        Player.MovementSpeedModifier += item.MovementSpeedModifier;
        Player.DamagePerBulletModifier += item.DamagePerBulletModifier;
        Player.TimeToReloadModifier += item.TimeToReloadModifier;
        Player.RateOfFireModifier += item.RateOfFireModifier;
        Player.MagazineSizeModifier += item.MagazineSizeModifier;
    }

    public void KillEnemy(GameObject enemy) {
        Enemies.Remove(enemy);

        if (Enemies.Count == 0) {
            LevelCleared();
        }

        EnemyCountUI.text = "Enemies: " + Enemies.Count;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        foreach (var spawnPoint in EnemySpawnPoints) {
            Gizmos.DrawSphere(spawnPoint, 0.05f);
        }
    }
}
