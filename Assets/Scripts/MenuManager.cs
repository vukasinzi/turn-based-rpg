using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public Config config;
    public Image white;
    public float waitTime = 1f;
    public float duration = 2f;
    public GameObject mapButtonPrefab;
    public Transform mapButtonContainer;

    public GameObject MapMenuPanel;
    public GameObject MainMenuPanel;

    void Start()
    {
        StartCoroutine(LoadConfig());

        if (white != null)
        {
            white.fillAmount = 0f;
            StartCoroutine(FillBar());
        }
    }
    public void MapMenu()
    {
        MapMenuPanel.SetActive(true);
        MainMenuPanel.SetActive(false);

        GenerateMapButtons();
    }
    IEnumerator LoadConfig()
    {
        using var request = UnityWebRequest.Get("http://localhost:5267/api/game/config");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }

        config = JsonConvert.DeserializeObject<Config>(request.downloadHandler.text);
    }
    IEnumerator FillBar()
    {
        yield return new WaitForSeconds(waitTime);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            white.fillAmount = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }

        white.fillAmount = 1f;
    }
    void GenerateMapButtons()
    {
        var layout = mapButtonContainer.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.spacing = 80f;

        for (int i = 0; i < config.monsters.Count; i++)
        {
            GameObject newButton = Instantiate(mapButtonPrefab, mapButtonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = config.monsters[i].Name;

            Sprite icon = Resources.Load<Sprite>("Icons/" + config.monsters[i].Name);
            if (icon != null)
                newButton.GetComponent<Image>().sprite = icon;
            else
                Debug.Log("Nema ikonice: " + config.monsters[i].Name);
        }
    }
}