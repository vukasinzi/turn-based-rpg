using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public BattleManager manager;
    public Move nextMove;
    public Hero hero;
    public Monster currentMonster;
    public GameObject heroPrefab;
    public GameObject[] monsterPrefabs;
    public Config config;
    public int currentMonsterIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GameObject heroObj = Instantiate(heroPrefab);
        hero = heroObj.GetComponent<Hero>();
        DontDestroyOnLoad(heroObj);
    }

    void Start()
    {
        StartCoroutine(GetConfigOnStartup());
    }

    public void SpawnMonster()
    {
        if (currentMonster != null)
            Destroy(currentMonster.gameObject);

        MonsterData monsterData = config.monsters[currentMonsterIndex];
        GameObject monsterObj = Instantiate(monsterPrefabs[currentMonsterIndex]);
        currentMonster = monsterObj.GetComponent<Monster>();
        currentMonster.Id = monsterData.Id;
        currentMonster.Stats = monsterData.Stats;
        currentMonster.Moveset = monsterData.Moveset;
        currentMonsterIndex++;

        manager.Init();
    }

    IEnumerator GetConfigOnStartup()
    {
        using var request = UnityWebRequest.Get("http://localhost:5267/api/game/config");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Ne ucitava config lepo");
            yield break;
        }

        config = JsonConvert.DeserializeObject<Config>(request.downloadHandler.text);
        SpawnMonster();
    }

    public IEnumerator GetNextMove()
    {
        string body = JsonConvert.SerializeObject(new
        {
            MonsterId = currentMonster.Id,
            MonsterBuffs = currentMonster.Buffs ?? new List<Buff>(),
            HeroBuffs = hero.Buffs ?? new List<Buff>()
        });

        using var request = UnityWebRequest.Post("http://localhost:5267/api/game/potez", body, "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Status: {request.responseCode} | Error: {request.downloadHandler.text}");    
            yield break;
        }

        nextMove = JsonConvert.DeserializeObject<Move>(request.downloadHandler.text);
    }
}