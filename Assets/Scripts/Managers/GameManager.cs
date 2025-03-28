using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private TMP_Text guessDisplay;

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
    private const float time = 7.0f;
    private const float speed = distance / time;

    private DualSenseGamepadHID pad;
    private PlayerManager player;
    private RumbleManager rumbleManager;
    private Scoreboard scoreboard;


    // Refernece posiotns
    private static float miss_distance = 5.0f;
    
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
    public bool is_training = true;

    // can guesss and record player input
    private bool can_guess = false;

    private float currCountdownValue;

    private bool current_guess = false;

    public int total_training_rounds = 10; // Should be divisible by 2
    
    public int total_testing_rounds = 20; // Should be divisible by 2
    


    // ------------------------------------------------
    // =========== Acutal code starts Here  ===========
    // ------------------------------------------------


    // Creates hazard at a position with a direction
    void CreateHazard(Vector3 start, Vector3 end)
    {
        hazard = Instantiate(hazardPrefab);
        direction = Vector3.Normalize(end - start);

        // move back so that it hits the why Axis at the Same Time
        hazard.transform.position = end + (direction * -distance);

        // Debug.Log($"Created ball at positon{hazard.transform.position} with direction {direction}");
    }


    


    // creats object, manages countdown, removes object
    public IEnumerator RunRound(float countdownValue)
    {

        can_guess = true;
        current_guess = false;
        guessDisplay.SetText($"Guess: Miss");

        // subscribe to event
        InputManager.instance.OnButtonPressed += HandleButtonPress;

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


        can_guess = false;

        Debug.Log($"Guess: {current_guess}; Hit: {is_hit}");
        if ((is_hit == PlayerHit.Hit && current_guess) || (is_hit == PlayerHit.LeftMiss && !current_guess) || (is_hit == PlayerHit.RightMiss && !current_guess))
            scoreboard.AddPoints(1);
        
        
        
        scoreDisplay.SetText($"Score: {scoreboard.GetCurrentScore().ToString()}");

        yield return new WaitForSeconds(1.0f);
        
        // unsubscribe from event
        InputManager.instance.OnButtonPressed -= HandleButtonPress;


        // remove hazard object
        Destroy(hazard);
        hazard = null;

        rumbleManager.ResetRumble();

    }


    private void HandleButtonPress()
    {
        if(can_guess)
        {
            current_guess = !current_guess;
            if (current_guess)
                guessDisplay.SetText($"Guess: Hit");
            else
                guessDisplay.SetText($"Guess: Miss");
        }
    }

    private float time_between_rounds = 2.0f;

    IEnumerator StartRounds()
    {
        int total_rounds = is_training ? total_training_rounds : total_testing_rounds;
        List<PlayerHit> hitPool = new List<PlayerHit>();

        for (int i = 0; i < total_rounds / 2; i++)
            hitPool.Add(PlayerHit.Hit);

        int missCount = total_rounds / 2;
        int leftMissCount = missCount / 2;
        int rightMissCount = missCount / 2;
        
        if (missCount % 2 != 0)
        {
            if (UnityEngine.Random.value > 0.5f)
                leftMissCount++;
            else
                rightMissCount++;
        }

        for (int i = 0; i < leftMissCount; i++)
            hitPool.Add(PlayerHit.LeftMiss);
        for (int i = 0; i < rightMissCount; i++)
            hitPool.Add(PlayerHit.RightMiss);

        for (int i = 0; i < hitPool.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, hitPool.Count);
            PlayerHit temp = hitPool[i];
            hitPool[i] = hitPool[randomIndex];
            hitPool[randomIndex] = temp;
        }

        foreach (PlayerHit hitType in hitPool)
        {
            is_hit = hitType;
            yield return RunRound(time - 1.0f);
            yield return new WaitForSeconds(time_between_rounds);
        }
        
        if(!is_training) scoreboard.ExportScoreToFile();
        ResetGame();
    }


    public void StartTest()
    {
        is_training = false;
        StartGame();
    }

    public void StartTraining()
    {
        is_training = true;
        StartGame();

    }


    public void StartGame()
    {

        MainMenu.SetActive(false);
        GameHUD.SetActive(true);
        ExperimentHUD.SetActive(!is_training);
        scoreboard.ResetScore();
        scoreDisplay.SetText($"Score: 0");

        StartCoroutine(StartRounds());
    }


    public void ResetGame()
    {

        // Enable main menu and disable everything else
        MainMenu.SetActive(true);
        GameHUD.SetActive(false);
        ExperimentHUD.SetActive(false);
        scoreboard.ResetScore();
        scoreDisplay.SetText($"Score: 0");
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
        if (scoreboard == null)
        {
            scoreboard = Scoreboard.Instance;
        }

        ResetGame();

    }


    private void FixedUpdate()
    {
        if (hazard is not null)
        {
            hazard.transform.Translate(direction * speed * Time.deltaTime);

        }
    }


    private void Update()
    {
        if (can_guess)
        {

            // TODO: 
            // record hit (will need to create aciton)
            //if (InputManager.instance.controls.Rumble.RumbleAction.WasPerformedThisFrame())
            //{

            //}

            // record left miss?

            // record right miss?

        }
    }



    private void LateUpdate()
    {

        if (hazard is not null)
        {
            // needs to be in late update (otherwise will not stop)
            rumbleManager.DistanceRumble(player.transform.position, hazard.transform.position, 10.0f);

        }
    }
}
