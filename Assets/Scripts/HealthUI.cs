using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public static HealthUI instance;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private float maxHP = 100f;
    private float currHP;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currHP = maxHP;
        hpSlider.maxValue = maxHP;
        hpSlider.value = currHP;
    }

    void Update()
    {
        
    }

    public void UpdateHP(float damage)
    {
        currHP -= damage;
        hpSlider.value = currHP;
        if (currHP <= 0) Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
