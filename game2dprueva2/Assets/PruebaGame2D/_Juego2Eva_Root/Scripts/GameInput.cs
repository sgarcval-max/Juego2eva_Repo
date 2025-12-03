using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public InputSystem inputActions;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (inputActions == null) inputActions = new InputSystem();
        inputActions.Enable();

        LoadBindings();
    }

    #region Rebinding
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void StartRebind(string actionName, int bindingIndex, Action onComplete = null)
    {
        InputAction action = inputActions.asset.FindAction(actionName);
        if (action == null) { Debug.LogWarning($"Action {actionName} not found"); return; }

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/position") // opcional: excluir controles
            .OnComplete(op =>
            {
                op.Dispose();
                SaveBinding(action);
                onComplete?.Invoke();
            })
            .Start();
    }
    #endregion

    #region Saving
    private void SaveBinding(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            string key = $"Binding_{action.name}_{i}";
            PlayerPrefs.SetString(key, action.bindings[i].overridePath);
        }
        PlayerPrefs.Save();
    }

    private void LoadBindings()
    {
        foreach (var action in inputActions.asset)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                string key = $"Binding_{action.name}_{i}";
                if (PlayerPrefs.HasKey(key))
                {
                    action.ApplyBindingOverride(i, PlayerPrefs.GetString(key));
                }
            }
        }
    }
    #endregion

    public string GetBindingDisplayString(string actionName, int bindingIndex = 0)
    {
        InputAction action = inputActions.asset.FindAction(actionName);
        if (action == null) return "";
        return InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}