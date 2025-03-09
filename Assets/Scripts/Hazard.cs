using UnityEngine;
using UnityEngine.InputSystem;

public class Hazard : MonoBehaviour
{


    private const float max_dist = 5.0f;

    private Gamepad pad;



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

        //Vector3 old_pos = transform.position;
        //float x_pos = Mathf.PingPong(Time.time, 8.0f) - 4f;
        //transform.position = new Vector3(x_pos, old_pos.y, old_pos.z);

        float left_intensity = GetIntensity(PlayerManager.instance.left_sensor.transform.position);
        float right_intensity = GetIntensity(PlayerManager.instance.right_sensor.transform.position);

        pad = Gamepad.current;

        //Debug.Log($"Vib: L {left_intensity} R {right_intensity}");

        if (pad != null)
        {
            pad.SetMotorSpeeds(left_intensity, right_intensity);
        }

    }




    private void OnDestroy()
    {
        pad = Gamepad.current;

        if (pad != null)
        {
            pad.SetMotorSpeeds(0.0f, 0.0f);
        }
    }
}
