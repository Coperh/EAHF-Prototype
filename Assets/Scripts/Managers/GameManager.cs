using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;

public class GameManager : MonoBehaviour
{



    // Distnace from Player and time in seconds
    // Further away, the faster it seems
    // Should probably be divisible but not necessarilly
    private const float distance = 20.0f;
    private const float time = 4.0f;
    private const float speed = distance / time;



    private DualSenseGamepadHID pad;
    private PlayerManager player;
    
    private Vector3 target;
    private Vector3 direction;


    [SerializeField]
    private GameObject hazardPrefab;
    private GameObject hazard;



    // Refernece posiotns

    private Vector2 hit_player = new Vector3(0, 1, 0);
    private Vector2 left_miss = new Vector3(-5, 1, 0);
    private Vector2 right_miss = new Vector3(5, 1, 0);


    private Vector3 start_mid = new Vector3(0,1, -distance);


    




    void CreateHazard(Vector3 start, Vector3 end)
    {

        hazard = Instantiate(hazardPrefab);
        direction = Vector3.Normalize(end - start);

        // 5 seconds away moving at a speed of 2
        hazard.transform.position = end + (direction * -distance);

        Debug.Log($"Created ball at positon{hazard.transform.position} with direction {direction}");
    }





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = PlayerManager.instance;
        }


        CreateHazard(start_mid, left_miss);

    }



    // Update is called once per frame
    void Update()
    {
        // distance over 5 seconds
        hazard.transform.Translate(direction * speed * Time.deltaTime);

        pad = (DualSenseGamepadHID)DualSenseGamepadHID.current;

        if (pad != null)
        {

        }
    }
}
