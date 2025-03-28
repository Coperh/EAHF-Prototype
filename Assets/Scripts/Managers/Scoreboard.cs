using UnityEngine;
using System.IO;
using UnityEditor; // Needed for AssetDatabase refresh

public class Scoreboard : MonoBehaviour
{
    private int currentScore = 0;
    public enum GameMode { Testing, Experiment }
    public GameMode currentGameMode = GameMode.Testing;

    // Singleton pattern
    public static Scoreboard Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        Debug.Log($"Score updated. Current: {currentScore}");
    }

    public int GetCurrentScore() => currentScore;
    public void ResetScore() => currentScore = 0;

    public void ExportScoreToFile(string fileName = "score")
    {
        // Determine subfolder based on game mode
        string subfolder = currentGameMode.ToString();
        string folderPath = Path.Combine(Application.dataPath, "Scores", subfolder);
        string filePath = Path.Combine(folderPath, $"{fileName}_{System.DateTime.Now:yyyyddMM_HHmmss}.txt");

        Debug.Log($"Exporting Score to {filePath}");

        // Create directory if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            #if UNITY_EDITOR
            AssetDatabase.Refresh(); // Only works in Editor
            #endif
        }

        // Prepare file content
        string content = $"Score: {currentScore} \n" +
                        $"Timestamp: {System.DateTime.Now}\n";

        try
        {
            File.WriteAllText(filePath, content);
            #if UNITY_EDITOR
            AssetDatabase.Refresh(); // Update Unity project window
            #endif
            Debug.Log($"Score saved to: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }
}