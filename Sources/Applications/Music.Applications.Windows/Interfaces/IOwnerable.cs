using Music.Shared.Common;

namespace Music.Applications.Windows.Interfaces;

public interface IOwnerable : IModel
{
    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }
}