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
        if(!inProgress)
        {
        Move move = hero.AllMoves[Random.Range(0, hero.AllMoves.Count)];     
       
        StartCoroutine(ExecuteTurn(move));
        }
    }
   public void Init()
{
    hero = GameManager.Instance.hero;
    monster = GameManager.Instance.currentMonster;
    hero.Death += OnHeroDeath;
    monster.Death += OnMonsterDeath;
    hero.Stats.Health = 50;
    hero.XP += 100;
}
 
    public IEnumerator ExecuteTurn(Move move)
    {
        inProgress = true;

            
        if(!hero.ExecuteCorrectMove(move, monster))
        {
            OnMoveClicked();
            inProgress = false;
            yield break;
        }
        Debug.Log($"Hero koristi {move.Name} | Kind: {move.Kind} | Scale: {move.Scale} | Power: {move.Power} | Monster HP: {monster.Stats.Health}");
        
        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(GameManager.Instance.GetNextMove());
        Move monsterMove = GameManager.Instance.nextMove;
        if(!monster.ExecuteCorrectMove(monsterMove, hero))
        {
            OnMoveClicked();
            inProgress = false;

            yield break;
        }
        Debug.Log($"Monster koristi {monsterMove.Name} | Kind: {monsterMove.Kind} | Scale: {monsterMove.Scale} | Power: {monsterMove.Power} | Hero HP: {hero.Stats.Health}");
        
        yield return new WaitForSeconds(1f);
        
        hero.BuffExpire();
        monster.BuffExpire();

        inProgress = false;
    }


    void OnHeroDeath() { 
        Debug.Log("Defeat"); 
        Destroy(hero);
    }
    void OnMonsterDeath() { 
        Debug.Log("Victory");
        Destroy(monster);
        GameManager.Instance.SpawnMonster();
    }  

}
