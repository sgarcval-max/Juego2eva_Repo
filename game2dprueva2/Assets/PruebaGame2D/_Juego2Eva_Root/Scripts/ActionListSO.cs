using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionList", menuName = "Input/Action List")]
public class ActionListSO : ScriptableObject
{
    public List<ActionData> actions;
}
