using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
internal class GameEventListner : MonoBehaviour
{
    [SerializeField] GameEvent _event;
    public UnityEvent<Vector3> Response;

    private void OnEnable()
    {
        _event.RegisterListner(this);
    }
    private void OnDisable()
    {
        _event.UnregisterListner(this);
    }
    public void OnEventRaised()
    {
        Response.Invoke(Vector3.zero);
    }
    
}
