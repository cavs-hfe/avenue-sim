using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this behavior to a game object to allow it to be tracked by the PublishPositionServer. Add a name to the object to identify it in the scene.
/// </summary>
public class PublishPositionBehavior : MonoBehaviour {

    [Tooltip("Name to be used when sending update messages, should be unique in scene")]
    public string name;

}
