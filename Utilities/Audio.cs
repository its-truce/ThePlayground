using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;

// ReSharper disable MemberCanBePrivate.Global

namespace ThePlayground.Utilities;

public static class Audio
{
    /// <summary>
    ///     Plays the given <see cref="SoundStyle" />.
    /// </summary>
    /// <param name="soundStyle">The <see cref="SoundStyle" /> to play</param>
    /// <param name="position">Position to play the sound at</param>
    /// <param name="volume">Volume multiplier</param>
    /// <param name="pitchVariance">The pitch offset randomness value</param>
    /// <param name="maxInstances">Max instances of the sound in the world. Set to -1 for infinite</param>
    /// <returns>The <see cref="SlotId" /> of the played sound.</returns>
    public static SlotId Play(this SoundStyle soundStyle, Vector2? position = null, float volume = 1f, float pitchVariance = 1f, int maxInstances = 1)
    {
        SlotId id = SoundEngine.PlaySound(soundStyle with { Volume = volume, PitchVariance = pitchVariance, MaxInstances = maxInstances }, position);
        return id;
    }

    /// <summary>
    ///     Returns a <see cref="SoundStyle" /> from the provided path.
    /// </summary>
    /// <param name="name">Name of the audio file</param>
    /// <param name="volume">Volume multiplier</param>
    /// <param name="pitchVariance">The pitch offset randomness value</param>
    /// <param name="maxInstances">Max instances of the sound in the world. Set to -1 for infinite</param>
    /// <param name="path">The path to the audio folder</param>
    /// <returns></returns>
    public static SoundStyle GetSoundStyle(string name, float volume = 1f, float pitchVariance = 1f, int maxInstances = 1,
        string path = $"{nameof(ThePlayground)}/Assets/Audio")
    {
        return new SoundStyle($"{path}/{name}") { Volume = volume, PitchVariance = pitchVariance, MaxInstances = maxInstances };
    }

    /// <summary>
    ///     Plays a sound from the provided path.
    /// </summary>
    /// <param name="name">Name of the audio file</param>
    /// <param name="position">Position to play the sound at</param>
    /// <param name="volume">Volume multiplier</param>
    /// <param name="pitchVariance">The pitch offset randomness value</param>
    /// <param name="maxInstances">Max instances of the sound in the world. Set to -1 for infinite</param>
    /// <param name="path">The path to the audio folder</param>
    /// <returns>The <see cref="SlotId" /> of the played sound.</returns>
    public static SlotId PlayFromPath(string name, Vector2? position = null, float volume = 1f, float pitchVariance = 1f, int maxInstances = 1,
        string path = $"{nameof(ThePlayground)}/Assets/Audio")
    {
        SlotId id = GetSoundStyle(name, volume, pitchVariance, maxInstances, path).Play(position);
        return id;
    }
}