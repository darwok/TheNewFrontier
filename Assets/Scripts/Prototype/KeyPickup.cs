using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public KeyPrototype keyPrototype;

    private void OnTriggerEnter(Collider other)
    {
        // Look for KeyInventory in the parent objects of the collider
        var inventory = other.GetComponentInParent<KeyInventory>();
        if (inventory != null && keyPrototype != null)
        {
            Debug.Log($"[KeyPickup] Añadiendo llave: {keyPrototype.id} al jugador {other.name}");
            inventory.AddKey(keyPrototype);
            gameObject.SetActive(false);
        }
        else
        {
            // Test to see if inventory is null, cause the door is not "opening" when picking up the key.
            Debug.LogWarning("[KeyPickup] No se encontró KeyInventory en el objeto que colisionó.");
        }
    }
}
