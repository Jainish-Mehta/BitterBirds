using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources (The Speakers)")]
    public AudioSource soundEffectsSpeaker;
    public AudioSource birdSpeaker;
    public AudioSource musicSpeaker;
    public AudioSource flightSpeaker;

    [Header("Music")]
    public AudioClip themeSong;

    [Header("Bird Sounds")]
    public AudioClip birdReady;
    public AudioClip stretch;
    public AudioClip snap;
    public AudioClip birdYell;
    public AudioClip flightWind;

    [Header("Level Sounds")]
    public AudioClip levelWon;
    public AudioClip levelLost;

    private Dictionary<AudioClip, float> soundCooldownMemory = new Dictionary<AudioClip, float>();
    private float soundCooldownTime = 0.05f;
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

    private void Start()
    {
        // Start the background music!
        if (themeSong != null && musicSpeaker != null)
        {
            musicSpeaker.clip = themeSong;
            musicSpeaker.loop = true;
            musicSpeaker.volume = 0.5f;
            musicSpeaker.Play();
        }
    }

    // =====================================
    // METHODS TO PLAY SPECIFIC SOUNDS
    // =====================================

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null || soundEffectsSpeaker == null) return;

        // --- FIX 1: THE COOLDOWN SYSTEM ---
        // Check if we have played this exact sound recently
        if (soundCooldownMemory.TryGetValue(clip, out float lastTimePlayed))
        {
            // If it hasn't been 50 milliseconds yet, IGNORE THIS SOUND to prevent audio clipping!
            if (Time.time - lastTimePlayed < soundCooldownTime)
            {
                return;
            }
        }

        // Record the exact time we played this sound
        soundCooldownMemory[clip] = Time.time;

        // --- FIX 2: PITCH RANDOMIZATION ---
        // Slightly change the pitch so multiple blocks breaking sounds organic, not robotic
        soundEffectsSpeaker.pitch = Random.Range(0.85f, 1.15f);

        // Play the sound!
        soundEffectsSpeaker.PlayOneShot(clip, volume);
    }

    public void PlayBirdSound(AudioClip clip)
    {
        if (clip != null && birdSpeaker != null)
        {
            // Reset pitch to normal for bird sounds so they don't sound weird
            birdSpeaker.pitch = 1f;
            birdSpeaker.PlayOneShot(clip);
        }
    }

    public void PlayStretch()
    {
        if (stretch != null && birdSpeaker != null && !birdSpeaker.isPlaying)
        {
            birdSpeaker.clip = stretch;
            birdSpeaker.Play();
        }
    }

    public void StopStretch()
    {
        if (birdSpeaker != null && birdSpeaker.clip == stretch)
        {
            birdSpeaker.Stop();
        }
    }

    public void UpdateFlightWind(float birdSpeed)
    {
        if (flightWind == null || flightSpeaker == null) return;

        if (birdSpeed > 10f)
        {
            if (!flightSpeaker.isPlaying)
            {
                flightSpeaker.clip = flightWind;
                flightSpeaker.loop = true;
                flightSpeaker.Play();
            }

            // The faster the bird goes, the louder the wind gets! (Max volume at speed 20)
            flightSpeaker.volume = Mathf.Clamp01(birdSpeed / 30f);
        }
        else
        {
            // If the bird slows down or stops, fade the wind out
            flightSpeaker.Stop();
        }
    }
}