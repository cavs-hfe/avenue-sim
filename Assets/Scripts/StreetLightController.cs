using UnityEngine;
using System.Collections;

public class StreetLightController : MonoBehaviour
{
    [Tooltip("Turn the streetlights on or off")]
    public bool streetlightsOn;

    [Tooltip("What time should the streetlights turn on? (Time is a float ranging from 0 [midnight] to 1 [11:59pm])")]
    public float streetlightOnTime;
    [Tooltip("What time should the streetlights turn off? (Time is a float ranging from 0 [midnight] to 1 [11:59pm])")]
    public float streetlightOffTime;

    [Tooltip("DayNightScript responsible for controlling time of day. Usually found in Ambience prefab.")]
    public DayNightScript dayNightController;

    private bool lightsOnNow;

    // Use this for initialization
    void Start()
    {
        if (streetlightsOn)
        {
            turnStreetlightsOn();
        }
        else
        {
            turnStreetlightsOff();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dayNightController != null)
        {
            if (dayNightController.currentTimeOfDay > streetlightOffTime && lightsOnNow)
            {
                turnStreetlightsOff();
            }
            else if (dayNightController.currentTimeOfDay > streetlightOnTime && !lightsOnNow)
            {
                turnStreetlightsOn();
            }
        } else {
            if (streetlightsOn != lightsOnNow && streetlightsOn)
            {
                turnStreetlightsOn();
            } else if (streetlightsOn != lightsOnNow && !streetlightsOn) {
                turnStreetlightsOff();
            }
        }
        
    }

    private void turnStreetlightsOn()
    {
        foreach (Light light in this.GetComponentsInChildren<Light>())
        {
            light.enabled = true;
        }
        lightsOnNow = true;
    }

    private void turnStreetlightsOff()
    {
        foreach (Light light in this.GetComponentsInChildren<Light>())
        {
            light.enabled = false;
        }
        lightsOnNow = false;
    }
}
