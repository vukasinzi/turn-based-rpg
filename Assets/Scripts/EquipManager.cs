using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EquipManager : MonoBehaviour
{
    public GameObject moveButtonPrefab;
    public Transform allMovesLayout;
    public Transform equippedLayout;

    public HeroDTO hero;
    public static EquipManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);}

    void Start()
    {
        StartCoroutine(Load());
        var allLayout = allMovesLayout.GetComponent<HorizontalLayoutGroup>();
        if (allLayout != null) allLayout.spacing = 70f;

        var equippedLayoutGroup = equippedLayout.GetComponent<HorizontalLayoutGroup>();
        if (equippedLayoutGroup != null) equippedLayoutGroup.spacing = 70f;


    }
    public void Clear(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
    IEnumerator Load()
    {
        using var request = UnityWebRequest.Get("http://localhost:5267/api/game/player");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            yield break;

        hero = JsonConvert.DeserializeObject<HeroDTO>(request.downloadHandler.text);
        hero.MapIdsToMoves();

        hero.equippedMoves ??= new List<Move>();

        hero.allMoves = hero.allMoves.Except(hero.equippedMoves).ToList();

        RenderMoves(allMovesLayout, hero.allMoves, OnClickAllMoves);
        RenderMoves(equippedLayout, hero.equippedMoves, OnClickEquippedMoves);
    }

    void RenderMoves(Transform layout, List<Move> moves, Action<Move> onClick)
    {
        Clear(layout);
        foreach (var m in moves)
        {
            var moveObj = Instantiate(moveButtonPrefab, layout);
            moveObj.GetComponentInChildren<TextMeshProUGUI>().text = m.Name;

            var img = moveObj.GetComponent<Image>();
            if (img != null)
                img.sprite = Resources.Load<Sprite>($"Icons/Moves/{m.Name}");

            Move captured = m;
            moveObj.GetComponent<Button>().onClick.AddListener(() => onClick(captured));
        }
    }

    void OnClickEquippedMoves(Move move)
    {
        hero.equippedMoves.Remove(move);
        hero.allMoves.Add(move);
        RenderMoves(allMovesLayout, hero.allMoves, OnClickAllMoves);
        RenderMoves(equippedLayout, hero.equippedMoves, OnClickEquippedMoves);
    }

    void OnClickAllMoves(Move move)
    {
        if (hero.equippedMoves.Count >= 4)
        {
            Debug.Log("Ne mozes vise od 4 poteza!");
            return;
        }

        hero.allMoves.Remove(move);
        hero.equippedMoves.Add(move);
        RenderMoves(allMovesLayout, hero.allMoves, OnClickAllMoves);
        RenderMoves(equippedLayout, hero.equippedMoves, OnClickEquippedMoves);
    }

}
