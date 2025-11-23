using UnityEngine;

public class healScript : MonoBehaviour
{
    private float floatAmplitude = 0.5f;
    private float floatFrequency = 2f;
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

    }
}
