using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.UIElements;

public class Hazard : MonoBehaviour
{


    private const float max_dist = 10.0f;

    private DualSenseGamepadHID pad;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    private float GetIntensity(Vector3 position)
    {
        Vector3 object_pos = transform.position;

        float distance = Vector3.Distance(object_pos, position);


        //Debug.Log($"Distance: {distance}");

        // We want this to be exponetialyl related to the distnace. 
        // So that it decreases dramaticalty the further it gets away.
        // Currently does not work like that
        float instensity =  Mathf.Sqrt(max_dist - distance) / Mathf.Sqrt(max_dist);
        return Mathf.Clamp(instensity, 0.0f, 1.0f);
    }


    // Update is called once per frame
    void Update()
    {

        //transform.transform.Translate(Vector3.forward * 4 * Time.deltaTime);


        Vector3 player_pos = PlayerManager.instance.transform.position;
        float distance = Vector3.Distance(transform.position, player_pos);


        // Get normalised direction, ignoring y-axis
        Vector3 dir = Vector3.Normalize(Vector3.Scale(player_pos - transform.position, new Vector3(1, 0, 1)));


        // Get angle between foward and direction, make value between 0 and 180  (for direcitons from behind, does not work with in front)
        float angle = Vector3.SignedAngle(dir, PlayerManager.instance.transform.forward, Vector3.up) + 90.0f;


        // 1 is fully right, 0 is fully left
        float right_percentage = angle / 180.0f;
        float left_percentage = 1.0f - right_percentage;

        //float left_intensity = GetIntensity(PlayerManager.instance.left_sensor.transform.position);
        //float right_intensity = GetIntensity(PlayerManager.instance.right_sensor.transform.position);

        float intensity = Mathf.Clamp((max_dist - distance)/max_dist, 0.0f, 1.0f) * 2.0f;

        pad = (DualSenseGamepadHID)DualSenseGamepadHID.current;

        Debug.Log($"Vib: L {left_percentage} R {right_percentage}");

        if (pad != null)
        {
            pad.SetMotorSpeedsAndLightBarColor(
                left_percentage * intensity,
                right_percentage * intensity,
                Color.green
                );
        }
    }


    private void OnDestroy()
    {
        pad = (DualSenseGamepadHID)DualSenseGamepadHID.current;

        if (pad != null)
        {
            pad.SetMotorSpeedsAndLightBarColor(
                0.0f, 
                0.0f,
                Color.red
                );
        }
    }
}
