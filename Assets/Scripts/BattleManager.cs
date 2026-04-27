using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public Hero hero;
    public Monster monster;

    bool inProgress = false;

    public void OnMoveClicked()
    {
        if (!inProgress)
        {
            Move move = hero.AllMoves[Random.Range(0, hero.AllMoves.Count)];

            StartCoroutine(ExecuteTurn(move));
        }
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
    }

    public IEnumerator ExecuteTurn(Move move)
    {
        inProgress = true;
        try
        {
             hero.BuffExpire();
            monster.BuffExpire();
            while (!hero.ExecuteCorrectMove(move, monster))
            {
                move = hero.AllMoves[Random.Range(0, hero.AllMoves.Count)];
            }

            Debug.Log($"Hero koristi {move.Name} | Kind: {move.Kind} | Scale: {move.Scale} | Power: {move.Power} | Hero HP: {hero.Stats.Health} | Monster HP: {monster.Stats.Health}");
            yield return new WaitForSeconds(1f);

            Move monsterMove;
            do
            {
                yield return StartCoroutine(GameManager.Instance.GetNextMove());
                monsterMove = GameManager.Instance.nextMove;
            }
            while (!monster.ExecuteCorrectMove(monsterMove, hero));

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

}
