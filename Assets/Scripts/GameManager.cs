using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public BattleManager manager;
    public Move nextMove;
    public Hero hero;
    public Monster currentMonster;
    public GameObject heroPrefab;
    public Transform heroSpawnPoint;
    public GameObject[] monsterPrefabs;
    public Transform monsterSpawnPoint;
    public Config config;
    private int selectedMonsterId;

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


    }

    void Start()
    {
    }

    public void SpawnMonster()
    {
        if (currentMonster != null)
            Destroy(currentMonster.gameObject);

        MonsterDTO monsterDTO = config.monsters.Find(m => m.Id == selectedMonsterId);
        int index = config.monsters.IndexOf(monsterDTO);

        GameObject monsterObj = Instantiate(monsterPrefabs[index],monsterSpawnPoint.position,Quaternion.identity);
        currentMonster = monsterObj.GetComponent<Monster>();
        currentMonster.Stats = monsterDTO.Stats;
        currentMonster.Id = monsterDTO.Id;
        currentMonster.Name = monsterDTO.Name;
        currentMonster.Moveset = monsterDTO.Moveset;
        manager.Init();
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

    internal void SendAll(Config config, HeroDTO dto, MonsterDTO selectedMonster)
    {
        this.config = config;
        this.selectedMonsterId = selectedMonster.Id;

        GameObject heroObj = Instantiate(heroPrefab, heroSpawnPoint.position,heroPrefab.transform.rotation);
        this.hero = heroObj.GetComponent<Hero>();
        this.hero.Name = dto.name;
        this.hero.Stats = dto.stats;
        this.hero.Moveset = dto.equippedMoves;

        SpawnMonster();
    }

}