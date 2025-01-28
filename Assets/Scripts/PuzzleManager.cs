using UnityEngine;
using UnityEngine.Tilemaps;

public class LeverPuzzleManager : MonoBehaviour
{
    public LeverInteraction[] levers; // Assign your lever objects in the Inspector
    public TilemapRenderer hiddenPathRenderer; // Assign the hidden path's TilemapRenderer in the Inspector
    public TilemapCollider2D hiddenCollider;
    private int[] correctOrder = { 3, 1, 2 };
    private int currentLeverIndex = 0;
    private bool puzzleSolved = false;

    public AudioSource puzzleSolvedAudioSource;
    public AudioClip puzzleSolvedSound;

    void Start()
    {
        // Initialize the hidden path as hidden
        hiddenPathRenderer.enabled = false;
        hiddenCollider.enabled = false;
    }

    void Update()
    {
        if (puzzleSolved)
        {
            return; // Exit the function if the puzzle is already solved
        }

        if (currentLeverIndex < correctOrder.Length)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                int expectedLeverNumber = correctOrder[currentLeverIndex];

                foreach (var lever in levers)
                {
                    if (lever.leverNumber == expectedLeverNumber && lever.IsFlipped())
                    {
                        currentLeverIndex++;
                        break;
                    }
                }

                if (currentLeverIndex == correctOrder.Length)
                {
                    // Puzzle solved
                    puzzleSolved = true;
                    Debug.Log("Puzzle Solved! You can proceed."); // Add your debug message
                    puzzleSolvedAudioSource.clip = puzzleSolvedSound;
                    puzzleSolvedAudioSource.Play();
                    // Reveal the hidden path by enabling the TilemapRenderer
                    hiddenPathRenderer.enabled = true;
                    hiddenCollider.enabled = true;

                    // You can add your logic here, like opening a door, etc.
                }
                else if (currentLeverIndex > 0 && levers[currentLeverIndex - 1].IsFlipped())
                {
                    // Reset the puzzle if an incorrect lever is flipped
                    currentLeverIndex = 0;
                }
            }
        }
    }
}
