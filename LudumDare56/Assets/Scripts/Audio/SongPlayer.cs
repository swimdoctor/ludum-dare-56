using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject player;

    public AudioSource[] audioSources; // Array to hold multiple audio sources
    private int currentSongIndex = 0; // Keep track of the current song

    private void OnEnable()
    {
        // Start playing the first song
        PlaySong(currentSongIndex);
        Debug.Log("PLAY");
    }

    public void PlaySong(int index)
    {
        if (index < 0 || index >= audioSources.Length) return; // Check for valid index

        if (currentSongIndex == index)
        {
            return; // Do nothing if the same song is requested
        }

        // Stop the current song
        if (audioSources[currentSongIndex].isPlaying)
        {
            audioSources[currentSongIndex].Stop();
        }

        // Update the current song index and play the new song
        currentSongIndex = index;
        audioSources[currentSongIndex].Play();
    }

    public void ChangeSong(int index)
    {
        PlaySong(index); // Method to change songs
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            ChangeSong(1);
        }
        
    }
}