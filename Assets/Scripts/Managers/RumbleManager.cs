using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager instance;

    private Gamepad pad;

    private Coroutine stopRumbleAfterTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    public void RumblePulse(float lowfreq, float highfreq, float duration)
    {

        pad = Gamepad.current;

        if (pad != null)
        {
            Debug.Log("Rumbling");
            pad.SetMotorSpeeds(lowfreq, highfreq);
        }

        stopRumbleAfterTime = StartCoroutine(StopRumble(duration, pad));

    }


    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float elaspsed_time = 0.0f;

        while (elaspsed_time < duration)
        {
            elaspsed_time += Time.deltaTime;
            yield return null;
        }

        pad.SetMotorSpeeds(0.0f, 0.0f);
    }

    

}
