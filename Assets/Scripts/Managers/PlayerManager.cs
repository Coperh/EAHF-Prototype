using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;


    public GameObject left_sensor;
    public GameObject right_sensor;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
