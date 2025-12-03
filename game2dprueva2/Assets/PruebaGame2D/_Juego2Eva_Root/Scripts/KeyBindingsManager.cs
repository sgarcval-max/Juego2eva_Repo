using System.Collections.Generic;
using UnityEngine;

public class KeyBindingsManager : MonoBehaviour
{
    public static KeyBindingsManager Instance;

    public ActionListSO actionList;

    private Dictionary<string, KeyCode> bindings = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadBindings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadBindings()
    {
        bindings.Clear();

        foreach (var action in actionList.actions)
        {
            string keyName = "key_" + action.actionName;

            if (PlayerPrefs.HasKey(keyName))
                bindings[action.actionName] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyName));
            else
                bindings[action.actionName] = action.defaultKey;
        }
    }

    public void SaveBindings()
    {
        foreach (var b in bindings)
        {
            PlayerPrefs.SetString("key_" + b.Key, b.Value.ToString());
        }

        PlayerPrefs.Save();
    }

    public KeyCode GetBinding(string action)
    {
        return bindings[action];
    }

    public void Rebind(string action, KeyCode key)
    {
        bindings[action] = key;
        SaveBindings();
    }
}