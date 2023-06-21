namespace Music.Applications.Windows.Core;

public abstract class ViewModelBase : ObservableObject, IDisposable
{
    public abstract string ModelName { get; protected set; }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ViewModelBase vm) return false;
        return vm.ModelName == ModelName;
    }

    protected bool Equals(ViewModelBase other)
    {
        return ModelName == other.ModelName;
    }

    public override int GetHashCode()
    {
        return ModelName.GetHashCode();
    }
}