using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton class that manages events in the game.
/// Allows dynamic subscription, removal, and dispatching of events with specific arguments.
/// </summary>
internal class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// Delegate representing an event handler with sender and event arguments.
    /// </summary>
    /// <typeparam name="T">Type of the event argument.</typeparam>
    /// <param name="sender">The source of the event.</param>
    /// <param name="eventArgs">The event-specific arguments.</param>
    public delegate void EventHandler<T>(object sender, T eventArgs);

    /// <summary>
    /// Dictionary storing event lists indexed by event keys.
    /// </summary>
    private Dictionary<string, LinkedList<Delegate>> _eventDict = new Dictionary<string, LinkedList<Delegate>>();

    /// <summary>
    /// Retrieves or creates a linked list of delegates for a given event key.
    /// </summary>
    /// <param name="key">The unique key identifying the event.</param>
    /// <returns>A linked list of delegates for the specified event key.</returns>
    private LinkedList<Delegate> GetOrCreateList(string key)
    {
        if (!_eventDict.TryGetValue(key, out var list))
        {
            list = new LinkedList<Delegate>();
            _eventDict[key] = list;
        }
        return list;
    }

    /// <summary>
    /// Adds an event listener for the specified event key.
    /// </summary>
    /// <typeparam name="T">Type of the event argument.</typeparam>
    /// <param name="key">The unique key identifying the event.</param>
    /// <param name="eventHandler">The event handler to subscribe.</param>
    public void AddEventListener<T>(string key, EventHandler<T> eventHandler)
    {
        var eventList = GetOrCreateList(key);
        if (!eventList.Contains(eventHandler))
        {
            eventList.AddLast(eventHandler);
        }
        else
        {
            Debug.LogWarning($"Event handler already exists: {key} >> {eventHandler}");
        }
    }

    /// <summary>
    /// Removes an event listener for the specified event key.
    /// </summary>
    /// <typeparam name="T">Type of the event argument.</typeparam>
    /// <param name="key">The unique key identifying the event.</param>
    /// <param name="eventHandler">The event handler to unsubscribe.</param>
    public void RemoveEventListener<T>(string key, EventHandler<T> eventHandler)
    {
        if (_eventDict.TryGetValue(key, out var eventList) && eventList.Contains(eventHandler))
        {
            eventList.Remove(eventHandler);
        }
    }

    /// <summary>
    /// Clears all event handlers associated with the specified event key.
    /// </summary>
    /// <param name="key">The unique key identifying the event.</param>
    public void ClearEventList(string key)
    {
        _eventDict.Remove(key);
    }

    /// <summary>
    /// Dispatches an event to all subscribed listeners for the specified event key.
    /// </summary>
    /// <typeparam name="T">Type of the event argument.</typeparam>
    /// <param name="key">The unique key identifying the event.</param>
    /// <param name="sender">The source of the event.</param>
    /// <param name="eventArgs">The event-specific arguments.</param>
    public void Dispatch<T>(string key, object sender, T eventArgs)
    {
        if (_eventDict.TryGetValue(key, out var eventList))
        {
            foreach (var eventDelegate in eventList)
            {
                if (eventDelegate is EventHandler<T> handler)
                {
                    handler.Invoke(sender, eventArgs);
                }
            }
        }
    }
}
