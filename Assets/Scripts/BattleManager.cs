using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public Hero hero;
    public Monster monster;


    
    public void OnMoveClicked()
    {
        Init();
     Move move = hero.AllMoves[Random.Range(0, hero.AllMoves.Count)];     
       
        StartCoroutine(ExecuteTurn(move));
    }
   public void Init()
{
    hero = GameManager.Instance.hero;
    monster = GameManager.Instance.currentMonster;
    hero.Death += OnHeroDeath;
    monster.Death += OnMonsterDeath;
}
  
    public IEnumerator ExecuteTurn(Move move)
    {
        hero.ExecuteCorrectMove(move, monster);
        Debug.Log($"Hero koristi {move.Name} | Kind: {move.Kind} | Scale: {move.Scale} | Power: {move.Power} | Monster HP: {monster.Stats.Health}");
        
        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(GameManager.Instance.GetNextMove());
        Move monsterMove = GameManager.Instance.nextMove;
        monster.ExecuteCorrectMove(monsterMove, hero);
        Debug.Log($"Monster koristi {monsterMove.Name} | Kind: {monsterMove.Kind} | Scale: {monsterMove.Scale} | Power: {monsterMove.Power} | Hero HP: {hero.Stats.Health}");
        
        yield return new WaitForSeconds(1f);
        
        hero.BuffExpire();
        monster.BuffExpire();
    }


    void OnHeroDeath() { 
        Debug.Log("Defeat"); 
    }
    void OnMonsterDeath() { 
        Debug.Log("Victory");
    }  

}
