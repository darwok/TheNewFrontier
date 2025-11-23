using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Bow,
    TeslaGun
}

public class AmmoMags : MonoBehaviour
{
    public Vector3 rotationSpeed = new(0, 50, 0);
    public string targetWeapon;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 2f;
    private Vector3 startPosition;
    //public Bow bowComponent;
    //public TeslaGun teslaGunComponent;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
