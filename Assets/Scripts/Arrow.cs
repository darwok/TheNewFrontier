using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] float maxTime;
    private float currentTime;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime  > maxTime)
        {
            currentTime = 0;
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
