using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace sokoban;

public static class SoundHandler
{
    public static bool MuteSound { set; get; }
    public static bool MuteMusic
    {
        set { MediaPlayer.IsMuted = value; }
        get { return MediaPlayer.IsMuted; }
    }

    static SoundHandler()
    {
        MediaPlayer.IsRepeating = true;
    }

    public static void Play(SoundEffect sound)
    {
        if (!MuteSound)
            sound.Play();
    }

    public static void RestartMusic()
    {
        MediaPlayer.Play(Resources.IsSeriousTheme ? Resources.TimeSong : Resources.PopcornSong);
    }
}
