using UnityEngine;

public class KeyGiverNPC : MonoBehaviour
{
    public KeyPrototype requiredKey;
    public KeyPrototype rewardKey;

    public void TryGiveKey(KeyInventory inventory)
    {
        if (inventory == null) return;

        if (requiredKey != null && !inventory.HasKey(requiredKey))
        {
            Debug.Log("NPC: todavía no has conseguido " + requiredKey.id);
            return;
        }

        if (rewardKey != null && !inventory.HasKey(rewardKey))
        {
            inventory.AddKey(rewardKey);
            Debug.Log("NPC: te doy " + rewardKey.id);
        }
        else
        {
            Debug.Log("NPC: ya tienes la llave");
        }
    }
}