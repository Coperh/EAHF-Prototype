using System.Collections;
using System.ComponentModel;
using System.Drawing;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{

    // ------------------------------------------------
    // Must be set in the editor
    [Header("UI Stuff")]
    [SerializeField]
    private TMP_Text scoreDisplay;

    [SerializeField]
    private TMP_Text countdownDisplay;

    [Header("Menus")]
    [SerializeField]
    [Tooltip("Main menu used to select mode and start game ")]
    private GameObject MainMenu;

    [SerializeField]
    [Tooltip("Common HUD useds in both game modes")]
    private GameObject GameHUD;

    [SerializeField]
    [Tooltip("HUD used specificially in the experiment mode")]
    private GameObject ExperimentHUD;


    [Header("Prefabs")]
    [SerializeField]
    [Tooltip("Prefab for the projectile")]
    private GameObject hazardPrefab;
    private GameObject hazard;
    // ------------------------------------------------


    // Distnace from Player and time in seconds
    // Further away, the faster it seems
    // Should probably be divisible but not necessarilly
    private const float distance = 10;
    private const float time = 5.0f;
    private const float speed = distance / time;

    private DualSenseGamepadHID pad;
    private PlayerManager player;
    private RumbleManager rumbleManager;


    // Refernece posiotns
    private static float miss_distance = 3.0f;
    
    // Direciton of moving project
    private Vector3 direction;


    // Starting points of the ball
    private static Vector3 start_centre = new Vector3(0, 1, -distance);
    private static Vector3 start_left = new Vector3(-miss_distance, 1, -distance);
    private static Vector3 start_right = new Vector3(miss_distance, 1, -distance);

    private StartPos starting_position = StartPos.Centre;
    Vector3 GetStartPoint(StartPos pos) => pos switch { // Switch expresison assigned to a function inplace
            // cases
            StartPos.Left => start_left,
            StartPos.Centre => start_centre,
            StartPos.Right => start_right,
            _ => start_centre, // default value
        };


    // Target Points
    private static Vector2 hit_player = new Vector3(0, 1.0f, 0);
    private static Vector2 left_miss = new Vector3(-miss_distance, 1.0f, 0);
    private static Vector2 right_miss = new Vector3(miss_distance, 1.0f, 0);

    private PlayerHit is_hit = PlayerHit.Hit;
    Vector3 GetHitPoint(PlayerHit pos) => pos switch
    {
        PlayerHit.LeftMiss => left_miss,
        PlayerHit.Hit => hit_player,
        PlayerHit.RightMiss => right_miss,
        _ => hit_player, // default value
    };


    // if training mode. Some features only available in training or test mode
    private bool is_training = true;


    void CreateHazard(Vector3 start, Vector3 end)
    {
        hazard = Instantiate(hazardPrefab);
        direction = Vector3.Normalize(end - start);

        // 5 seconds away moving at a speed of 2
        hazard.transform.position = end + (direction * -distance);

        Debug.Log($"Created ball at positon{hazard.transform.position} with direction {direction}");
    }


    private float currCountdownValue;
    public IEnumerator RunRound(float countdownValue)
    {


        Vector3 start_pos = GetStartPoint(starting_position);
        Vector3 end_pos = GetHitPoint(is_hit);

        CreateHazard(start_pos, end_pos);



        currCountdownValue = countdownValue;

        countdownDisplay.SetText($"Time to Guess: {currCountdownValue}");
        while (currCountdownValue > 0)
        {
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
            countdownDisplay.SetText($"Time to Guess: {currCountdownValue}");
        }



        // remove hazard object
        Destroy(hazard);
        hazard = null;
        rumbleManager.ResetRumble();
    }


    IEnumerator StartRounds()
    {

        yield return RunRound(time-1.0f);


    }



    void StartGame()
    {

        MainMenu.SetActive(false);
        GameHUD.SetActive(true);
        ExperimentHUD.SetActive(!is_training);


        StartCoroutine(StartRounds());
    }





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = PlayerManager.instance;
        }
        if (rumbleManager == null)
        {
            rumbleManager = RumbleManager.instance;
        }

        // Enable main menu and disable everything else
        MainMenu.SetActive(true);
        GameHUD.SetActive(false);
        ExperimentHUD.SetActive(false);

        StartGame();

    }


    private void Update()
    {

        if (hazard is not null)
        {
            hazard.transform.Translate(direction * speed * Time.deltaTime);
            rumbleManager.DistanceRumble(player.transform.position, hazard.transform.position, 5.0f);

        }
    }

}
