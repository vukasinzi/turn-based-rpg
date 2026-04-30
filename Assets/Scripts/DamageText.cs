using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private TextMeshPro text;
private float moveSpeed = 0.5f;    
private float fadeDuration = 0.8f;
    private float timer;

    void Awake() => text = GetComponentInChildren<TextMeshPro>();

    public void Setup(int damage)
    {
        text.text = "-" + damage.ToString();
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        text.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        if (timer >= fadeDuration)
            Destroy(gameObject);
    }
}