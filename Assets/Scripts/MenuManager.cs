using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;



public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public Config config;
    public Image white;
    public float waitTime = 1f;
    public float duration = 2f;
    public GameObject mapButtonPrefab;
    public Transform mapButtonContainer;

    public GameObject MapMenuPanel;
    public GameObject MainMenuPanel;
    public GameObject InventoryMenuPanel;

    private MonsterDTO selectedMonster;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(LoadConfig());

        if (white != null)
        {
            white.fillAmount = 0f;
            StartCoroutine(FillBar());
        }
    }
    public void InventoryMenu()
    {
        InventoryMenuPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        MapMenuPanel.SetActive(false);
    }
    public void MapMenu()
    {
        if (EquipManager.Instance != null && EquipManager.Instance.hero.equippedMoves.Count < 4)
        {
            Debug.Log("Nema opremljenih poteza!");
            return;
        }
        MapMenuPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        InventoryMenuPanel.SetActive(false);

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
    void Clear(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
    void GenerateMapButtons()
    {
        Clear(mapButtonContainer);

        var layout = mapButtonContainer.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.spacing = 80f;

        for (int i = 0; i < config.monsters.Count; i++)
        {
            GameObject newButton = Instantiate(mapButtonPrefab, mapButtonContainer);

            newButton.GetComponentInChildren<TextMeshProUGUI>().text = config.monsters[i].Name;
            var monster = config.monsters[i];
            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedMonster = monster;
                Play();
            });
            Sprite icon = Resources.Load<Sprite>("Icons/" + config.monsters[i].Name);
            if (icon != null)
                newButton.GetComponent<Image>().sprite = icon;
        }
    }
    public void Play()
    {
        if (EquipManager.Instance == null || EquipManager.Instance.hero == null || config == null)
        {
            Debug.Log("Nedostaju podaci za start borbe!");
            return;
        }

        var dto = EquipManager.Instance.hero;
        var monster = selectedMonster;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "SampleScene") return;
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (GameManager.Instance == null)
            {
                Debug.Log("GameManager nije pronađen u SampleScene!");
                return;
            }

            GameManager.Instance.SendAll(config, dto, monster);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("SampleScene");
    }


}