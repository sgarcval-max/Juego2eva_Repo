using UnityEngine;

public class KeyBindingsLoader : MonoBehaviour
{
    void Awake()
    {
        if (KeyBindingsManager.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("KeyBindingsManager");
            Instantiate(prefab);
        }
    }
}
