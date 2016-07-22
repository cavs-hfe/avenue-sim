using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this script to a game object to allow it to be moved by the PositionUpdateServer
/// </summary>
public class PositionUpdateBehavior : MonoBehaviour {

    [Tooltip("Name to be used to identify game object in update messages, should be unique in scene")]
    public string name;
}
