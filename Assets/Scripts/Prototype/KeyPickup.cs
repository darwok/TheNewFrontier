using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public KeyPrototype keyPrototype;

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<KeyInventory>();
        if (inventory != null && keyPrototype != null)
        {
            inventory.AddKey(keyPrototype);
            gameObject.SetActive(false);
        }
    }
}