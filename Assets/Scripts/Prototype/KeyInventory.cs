using System.Collections.Generic;
using UnityEngine;

public class KeyInventory : MonoBehaviour
{
    private readonly HashSet<string> keys = new HashSet<string>();

    public void AddKey(KeyPrototype prototype)
    {
        if (prototype == null) return;
        keys.Add(prototype.id);
        Debug.Log("Key añadida: " + prototype.id);
    }

    public bool HasKey(KeyPrototype prototype)
    {
        if (prototype == null) return false;
        return keys.Contains(prototype.id);
    }
}