using UnityEngine;

public class Dart_Audio_Impact_Completed : MonoBehaviour
{
    private bool hasPlayed = false;

    void OnCollisionEnter(Collision other)
    {
        if (!hasPlayed)  // Check if the audio has already played
        {
            // Play Plastic Impact Audio File
            GetComponent<AudioSource>().Play();

            hasPlayed = true; // Set the flag to true so the audio won't play again
        }
    }
}