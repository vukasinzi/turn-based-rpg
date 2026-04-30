using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Hero hero;
    public Monster monster;
    public GameObject moveIconPrefab;
    public Transform equippedLayout;
    public Image HeroHealth;
    public Image MonsterHealth;
    public GameObject damageTextPrefab;
    bool inProgress = false;
    private int MaxHeroHealth;
    private int MaxMonsterHealth;
    public void OnMoveClicked()
    {

    }

    void OnDestroy()
    {
        if (hero != null) hero.Death -= OnHeroDeath;
        if (monster != null) monster.Death -= OnMonsterDeath;
    }

    public void Init()
    {
        if (hero != null) hero.Death -= OnHeroDeath;
        if (monster != null) monster.Death -= OnMonsterDeath;

        hero = GameManager.Instance.hero;
        monster = GameManager.Instance.currentMonster;

        hero.Death += OnHeroDeath;
        monster.Death += OnMonsterDeath;
        equippedLayout.GetComponent<HorizontalLayoutGroup>().spacing = 70f;
        foreach (Transform child in equippedLayout)
            Destroy(child.gameObject);
        int i = 0;
        foreach (var move in hero.Moveset)
        {
            GameObject imageObj = Instantiate(moveIconPrefab, equippedLayout);
            var text = imageObj.GetComponentInChildren<TextMeshProUGUI>();
            i++;
            text.text = i + "." + move.Name;
            var img = imageObj.GetComponent<Image>();
            if (img != null)
                img.sprite = Resources.Load<Sprite>($"Icons/Moves/{move.Name}");
        }
        MaxHeroHealth = hero.Stats.Health;
        MaxMonsterHealth = monster.Stats.Health;
        HeroHealth.fillAmount = (float)hero.Stats.Health / MaxHeroHealth;
        MonsterHealth.fillAmount = (float)monster.Stats.Health / MaxMonsterHealth;

    }

    public IEnumerator ExecuteTurn(Move move)
    {
        inProgress = true;
        try
        {
            hero.BuffExpire();
            monster.BuffExpire();
            //izmena, prethodna verzija je podrazumevala reroll na frontu slanjem zahteva backu,
            //sada imamo samo neki fallback koji u sustini ako izbaci nevalidan potez, sam izabere drugi.
            while (!hero.ExecuteCorrectMove(move, monster))
            {
                move = hero.Moveset[Random.Range(0, hero.Moveset.Count)];
            }
            if (move.Kind == "damage" || move.Kind == "damage_debuff")
            {
                var popup = Instantiate(damageTextPrefab, monster.transform.position + Vector3.up, Quaternion.identity);
                popup.GetComponent<DamageText>().Setup((int)move.Power);
            }
            Debug.Log($"Hero koristi {move.Name} | Kind: {move.Kind} | Scale: {move.Scale} | Power: {move.Power} | Hero HP: {hero.Stats.Health} | Monster HP: {monster.Stats.Health}");
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(GameManager.Instance.GetNextMove());
            Move monsterMove = GameManager.Instance.nextMove;
            monster.ExecuteCorrectMove(monsterMove, hero);
            if (monsterMove.Kind == "damage" || monsterMove.Kind == "damage_debuff")
            {
                var popup = Instantiate(damageTextPrefab, hero.transform.position + Vector3.up, Quaternion.identity);
                popup.GetComponent<DamageText>().Setup((int)monsterMove.Power);
            }
            Debug.Log($"Monster koristi {monsterMove.Name} | Kind: {monsterMove.Kind} | Scale: {monsterMove.Scale} | Power: {monsterMove.Power} | Hero HP: {hero.Stats.Health} | Monster HP: {monster.Stats.Health}");
            yield return new WaitForSeconds(1f);
        }
        finally
        {
            inProgress = false;
        }
    }

    void OnHeroDeath()
    {
        inProgress = true;
        Debug.Log("Defeat");
        OnDestroy();
        Destroy(hero.gameObject);
        GameManager.Instance.currentMonster = null;
        GameManager.Instance.hero = null;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void OnMonsterDeath()
    {
        inProgress = true;
        OnDestroy();
        Debug.Log("Victory");
        StartCoroutine(SavePlayerAndReturn());


    }
    HeroDTO h;
    IEnumerator LoadHero()
    {
        using var request = UnityWebRequest.Get("http://localhost:5267/api/game/player");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            yield break;

        h = JsonConvert.DeserializeObject<HeroDTO>(request.downloadHandler.text);

        h.equippedMoves = new List<Move>();
    }
    IEnumerator SavePlayerAndReturn()
    {
        yield return StartCoroutine(LoadHero());

        var validMoves = GameManager.Instance.currentMonster.Moveset
        .Where(m => !h.allMoves.Any(e => e.Name == m.Name))
        .ToList();
        Move randomMove;
        if (validMoves.Count != 0)
        {
            randomMove = validMoves[Random.Range(0, validMoves.Count)];
            randomMove.Id = h.allMoves.Count > 0 ? h.allMoves.Max(m => m.Id) + 1 : 1;
            h.allMoves.Add(randomMove);
        }

        h.AddXpAndLevelUp(100);
        h.equippedMoveIds = hero.Moveset.Select(m => m.Id).ToList();

        string jsonData = JsonConvert.SerializeObject(h);

        using var request = UnityWebRequest.Post("http://localhost:5267/api/game/save", jsonData, "application/json");
        yield return request.SendWebRequest();

        GameManager.Instance.currentMonster = null;
        GameManager.Instance.hero = null;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if (!inProgress)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(ExecuteTurn(hero.Moveset[0]));
            else if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(ExecuteTurn(hero.Moveset[1]));
            else if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(ExecuteTurn(hero.Moveset[2]));
            else if (Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(ExecuteTurn(hero.Moveset[3]));
        }
        if (hero != null && monster != null)
        {
            HeroHealth.fillAmount = Mathf.Lerp(HeroHealth.fillAmount, (float)hero.Stats.Health / MaxHeroHealth, Time.deltaTime * 5f);
            MonsterHealth.fillAmount = Mathf.Lerp(MonsterHealth.fillAmount, (float)monster.Stats.Health / MaxMonsterHealth, Time.deltaTime * 5f);
        }
    }
}