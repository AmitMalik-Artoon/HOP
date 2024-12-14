using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameEvent",menuName ="Scriptable/GameEvent")]
internal class GameEvent : ScriptableObject
{
    private List<GameEventListner> _listners = new List<GameEventListner>();

    public void RegisterListner(GameEventListner listner)
    {
        _listners.Add(listner);
    }
    public void UnregisterListner(GameEventListner listner)
    {
        _listners.Remove(listner);
    }

    public void Raise()
    {
        foreach (var listner in _listners)
        {
            listner.OnEventRaised();
        }
    }

}
