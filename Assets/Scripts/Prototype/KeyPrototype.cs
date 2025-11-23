using UnityEngine;

[CreateAssetMenu(menuName = "TheNewFrontier/KeyPrototype")]
public class KeyPrototype : ScriptableObject
{
    public string id;          // "Key0", "Key1", etc.
    public string displayName;
    public Sprite icon;        // Useful for UI representation, not implemented yet...
}