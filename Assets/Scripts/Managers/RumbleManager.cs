using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager instance;

    // might make sense to change back to generic gamepad. Might make stop
    private DualSenseGamepadHID pad;

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

        pad = (DualSenseGamepadHID)DualSenseGamepadHID.current;

        if (pad != null)
        {
            Debug.Log("Rumbling");
            pad.SetMotorSpeeds(lowfreq, highfreq);
        }

        stopRumbleAfterTime = StartCoroutine(StopRumble(duration, pad));

    }


    private IEnumerator StopRumble(float duration, DualSenseGamepadHID pad)
    {
        float elaspsed_time = 0.0f;

        while (elaspsed_time < duration)
        {
            elaspsed_time += Time.deltaTime;
            yield return null;
        }

        pad.SetMotorSpeeds(0.0f, 0.0f);
    }


    public void DistanceRumble(Vector3 target, Vector3 projectile, float max_dist) {


        float distance = Vector3.Distance(projectile, target);

        // Get normalised direction, ignoring y-axis
        Vector3 dir = Vector3.Normalize(Vector3.Scale(target - projectile, new Vector3(1, 0, 1)));

        // TODO: could be improved I think. Might be a btter way to do it.
        // Get angle between foward and direction, make value between 0 and 180  (for direcitons from behind, does not work with in front)
        float angle = Vector3.SignedAngle(dir, PlayerManager.instance.transform.forward, Vector3.up) + 90.0f;


        // 1 is fully right, 0 is fully left
        float right_percentage = angle / 180.0f;
        float left_percentage = 1.0f - right_percentage;


        // TODO: Could be improved, not enough different currently I think. 
        float intensity = Mathf.Clamp((max_dist - distance) / max_dist, 0.0f, 1.0f) * 2.0f;

        pad = (DualSenseGamepadHID)DualSenseGamepadHID.current;

        //Debug.Log($"Vib: L {left_percentage} R {right_percentage}");

        if (pad != null)
        {
            // setting the colour here will break PauseHaptics,  ResumeHaptics, ResetHaptics for some reason
            pad.SetMotorSpeeds(
                left_percentage * intensity,
                right_percentage * intensity
                );
        }

    }


    public void ResetRumble()
    {
        pad = (DualSenseGamepadHID)DualSenseGamepadHID.current;
        if (pad != null)
        {
            pad.SetMotorSpeeds(0.0f, 0.0f);
        }
    }


    // this does not work. I think the above is being called somehow
    private void OnApplicationPause(bool pause)
    {
        pad = (DualSenseGamepadHID)DualSenseGamepadHID.current;

        if (pad != null)
        {

            if (pause)
                pad.PauseHaptics();
            else
                pad.ResumeHaptics();
        }


    }


    private void OnDestroy()
    {
        pad = (DualSenseGamepadHID)DualSenseGamepadHID.current;

        if (pad != null)
        {
            pad.ResetHaptics();
        }
    }

}
