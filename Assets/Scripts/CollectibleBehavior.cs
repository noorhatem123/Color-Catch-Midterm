using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CollectibleBehavior : MonoBehaviour
{
    // UI elements for displaying score, game over message, and timer
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI GameOverText;
    public TextMeshProUGUI timerText;

    // Game objects for player and retry button
    public GameObject player;
    public GameObject retryButton;

    // UI element to display the target color
    public Image TargetColorDisplay;

    // Audio clips for correct and incorrect collectible sounds
    public AudioClip correctSound;
    public AudioClip wrongSound;

    // AudioSource for playing sound effects
    private AudioSource audioSource;

    // Static variables to keep track of the score and total number of collectibles
    private static int score = 0;
    private static int totalCollectibles = 12;

    // Timer variables
    private float timeRemaining = 35f;
    private bool timerIsRunning = true;

    // Static variable to store the current target color
    private static Color targetColor;

    private void Start()
    {
        // Hide game over text and retry button initially
        GameOverText.gameObject.SetActive(false);
        retryButton.SetActive(false);

        // Update score and timer text at the start
        UpdateScoreText();
        UpdateTimerText();

        // Set the initial target color
        SetRandomTargetColor();

        // Initialize the AudioSource component for playing sounds
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // Ensure it doesn't play automatically
        audioSource.enabled = true;
    }

    private void Update()
    {
        // Update timer if the game is still running
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Decrease time
                UpdateTimerText(); // Update UI
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                EndGame(); // End game when time is up
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has collided with the collectible
        if (other.CompareTag("Player"))
        {
            Color collectibleColor = GetComponent<Renderer>().material.color; // Get collectible's color

            // Check if collectible color matches target color
            if (collectibleColor == targetColor)
            {
                score++; // Increase score
                UpdateScoreText(); // Update UI

                // Play the correct sound and deactivate collectible
                PlaySound(correctSound);
                gameObject.SetActive(false); // Only deactivate if it is the correct color

                // Check if all items are collected
                if (score >= totalCollectibles)
                {
                    EndGame(); // End game if all collectibles are collected
                }
                else
                {
                    SetRandomTargetColor(); // Set a new target color
                }
            }
            else
            {
                // Handle incorrect color collectible
                Debug.Log("Incorrect color collected!");

                score = Mathf.Max(0, score - 1); // Decrease score by 1 but ensure it doesn’t go below 0
                UpdateScoreText(); // Update UI

                // Play incorrect sound without deactivating collectible
                PlaySound(wrongSound);
            }
        }
    }

    // Method to play a sound effect
    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip; // Set the clip to play
        audioSource.Play(); // Play the audio clip
    }

    // Method to update the score text UI
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    // Method to update the timer text UI
    private void UpdateTimerText()
    {
        timerText.text = "Time Left: " + Mathf.Ceil(timeRemaining) + " Seconds";
    }

    // Method to handle game over logic
    private void EndGame()
    {
        GameOverText.gameObject.SetActive(true);
        GameOverText.text = "Game Over! Final Score: " + score; // Display final score
        retryButton.SetActive(true); // Show retry button
        timerIsRunning = false; // Stop timer
        player.SetActive(false); // Deactivate player
    }

    // Method to restart the game
    public void RestartGame()
    {
        score = 0; // Reset score
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    // Method to set a random target color from active collectibles
    private void SetRandomTargetColor()
    {
        // Define the available colors
        Color[] colors = { Color.red, Color.blue, Color.green };
        List<Color> availableColors = new List<Color>();

        // Find active collectibles with each color
        foreach (Color color in colors)
        {
            GameObject[] collectibles = GameObject.FindGameObjectsWithTag("collectible");
            foreach (GameObject collectible in collectibles)
            {
                if (collectible.activeSelf && collectible.GetComponent<Renderer>().material.color == color)
                {
                    availableColors.Add(color); // Add color to available options
                    break; // No need to check further collectibles of this color
                }
            }
        }

        if (availableColors.Count > 0)
        {
            // Pick a random color from available options
            targetColor = availableColors[Random.Range(0, availableColors.Count)];
            TargetColorDisplay.color = targetColor; // Update UI to show target color
        }
        else
        {
            Debug.LogWarning("No collectibles left to target.");
            EndGame(); // End game if no collectibles are left
        }
    }
}
