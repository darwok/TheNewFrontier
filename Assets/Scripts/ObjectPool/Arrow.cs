using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float maxTime = 3f;

    private float currentTime;
    private Rigidbody rb;
    private ObjectPool pool;

    public void Init(ObjectPool pool)
    {
        this.pool = pool;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        currentTime = 0f;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= maxTime)
            Despawn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Despawn();
    }

    private void Despawn()
    {
        if (pool != null)
            pool.Return(gameObject);
        else
            gameObject.SetActive(false);
    }
}