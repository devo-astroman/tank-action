using UnityEngine;

public static class GameSound
{
    // A separate source lets destruction sounds finish after the enemy disappears.
    public static void Play(AudioClip clip)
    {
        if (clip == null) return;
        GameObject sound = new GameObject("Sound: " + clip.name);
        AudioSource source = sound.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0.7f;
        source.clip = clip;
        source.Play();
        Object.Destroy(sound, clip.length + 0.1f);
    }
}
