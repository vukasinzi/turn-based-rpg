using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Hero hero;
    public Monster monster;
    public GameObject moveIconPrefab;
    public Transform equippedLayout;
    bool inProgress = false;

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

            Debug.Log($"Hero koristi {move.Name} | Kind: {move.Kind} | Scale: {move.Scale} | Power: {move.Power} | Hero HP: {hero.Stats.Health} | Monster HP: {monster.Stats.Health}");
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(GameManager.Instance.GetNextMove());
            Move monsterMove = GameManager.Instance.nextMove;
            monster.ExecuteCorrectMove(monsterMove, hero);

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
        Debug.Log("Defeat");
        Destroy(hero);
        OnDestroy();
    }

    void OnMonsterDeath()
    {
        Debug.Log("Victory");
        Destroy(monster);
        GameManager.Instance.SpawnMonster();
        hero.Stats.Health = 50;
        hero.XP += 100;
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
    }
}