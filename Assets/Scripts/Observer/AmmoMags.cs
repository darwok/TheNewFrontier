using UnityEngine;

public class AmmoMags : MonoBehaviour
{
    public Vector3 rotationSpeed = new(0, 50, 0);
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 2f;
    [SerializeField] private int ammoAmount = 10;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        var stats = other.GetComponent<PlayerStatsSubject>();
        var bow = other.GetComponentInChildren<Bow>();
        var tesla = other.GetComponentInChildren<TeslaGun>();

        if (stats != null)
            stats.AddAmmo(ammoAmount);

        // Restock de mags de armas también
        if (bow != null) bow.RestockAmmo();
        if (tesla != null) tesla.RestockAmmo();

        gameObject.SetActive(false);
    }
}