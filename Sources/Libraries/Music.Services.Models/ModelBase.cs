using Music.Shared.Common;

namespace Music.Services.Models;

public abstract class ModelBase : IModel
{
    public Guid Id { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is not IModel model) return false;
        return model.Id == Id;
    }

    protected bool Equals(ModelBase other)
    {
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}