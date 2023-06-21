using Music.Applications.Windows.Enums;
using Music.Applications.Windows.ViewModels.Default;

namespace Music.Applications.Windows.Core.Player;

public interface ICoopPlayer : IPlayerBase
{
}

public class CoopPlayer : PlayerBase, ICoopPlayer
{
    public CoopPlayer(SettingsViewModel settings) : base(settings)
    {
    }

    //todo ROOM

    public override bool ChangePosition(long position)
    {
        throw new NotImplementedException();
    }

    public override PlayerType PlayerType => PlayerType.Coop;
}