using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Move nextMove;
    public Monster currentMonster;
    public Config config;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(GetConfigOnStartup());
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
        
        foreach (Monster x in config.monsters)
            Debug.Log(x.Name);
    }
    IEnumerator GetNextMove()
    {
        using var request = UnityWebRequest.Post("http://localhost:5267/api/game/potez",JsonConvert.SerializeObject(new {currentMonster.Id}),"application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Ne ucitava potez lepo");
            yield break;
        }
        nextMove = JsonConvert.DeserializeObject<Move>(request.downloadHandler.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
