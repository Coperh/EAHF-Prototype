using UnityEngine;
using UnityEngine.InputSystem;

public class Hazard : MonoBehaviour
{


    private const float max_dist = 20.0f;

    private Gamepad pad;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    private float GetIntensity(Vector3 position)
    {
        Vector3 object_pos = transform.position;

        float distance = Vector3.Distance(object_pos, position);
        float instensity =  (max_dist - distance) / (max_dist * max_dist);
        return Mathf.Clamp(instensity, 0.0f, 1.0f);
    }


    // Update is called once per frame
    void Update()
    {

        Vector3 old_pos = transform.position;
        float x_pos = Mathf.PingPong(Time.time, 8.0f) - 4f;
        transform.position = new Vector3(x_pos, old_pos.y, old_pos.z);

        float left_intensity = GetIntensity(PlayerManager.instance.left_sensor.transform.position);
        float right_intensity = GetIntensity(PlayerManager.instance.right_sensor.transform.position);

        pad = Gamepad.current;

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
