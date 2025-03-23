using UnityEngine;

public class RumbleTest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.controls.Rumble.RumbleAction.WasPerformedThisFrame())
        {
            RumbleManager.instance.RumblePulse(0.5f, 0.5f, 0.25f);
        }
        
    }
}
