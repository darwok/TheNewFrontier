using UnityEngine;

public class healScript : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 2f;
    [SerializeField] private float healAmount = 20f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        var stats = other.GetComponent<PlayerStatsSubject>();
        if (stats != null)
        {
            stats.Heal(healAmount);
            gameObject.SetActive(false);
        }
    }
}