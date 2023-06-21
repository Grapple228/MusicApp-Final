using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels.Default;

namespace Music.Applications.Windows.Core.Player;

public interface ISinglePlayer : IPlayerBase
{
}

public class SinglePlayer : PlayerBase, ISinglePlayer
{
    public SinglePlayer(SettingsViewModel settings) : base(settings)
    {
    }

    public override PlayerType PlayerType => PlayerType.Single;

    public override bool ChangePosition(long position)
    {
        if (position >= WaveStream!.Length)
        {
            TrackEnded();
            return false;
        }
        WaveStream.Position = position;
        currentPosition = position;
        return true;
    }
}