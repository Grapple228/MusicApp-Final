using Music.Applications.Windows.Enums;
using MusicClient.Exceptions;

namespace Music.Applications.Windows.Core;

public abstract class LoadableViewModel : ViewModelBase
{
    private bool _isLoading;
    private LoadStatus _loadStatus;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public LoadStatus LoadStatus
    {
        get => _loadStatus;
        set
        {
            _loadStatus = value;
            OnPropertyChanged();
            IsLoading = LoadStatus == LoadStatus.Loading;
        }
    }

    protected async Task LoadInfo(Guid mediaId, bool isFirstLoad = true)
    {
        LoadStatus = isFirstLoad ? LoadStatus.Loading : LoadStatus.Success;

        try
        {
            await LoadMedia(mediaId);
        }
        catch (ServerUnavailableException)
        {
            LoadStatus = LoadStatus.ServerUnavailable;
            return;
        }
        catch (Status404Exception)
        {
            LoadStatus = LoadStatus.NotFound;
            return;
        }
        catch (Exception)
        {
            LoadStatus = LoadStatus.Problem;
            return;
        }

        LoadStatus = LoadStatus.Success;
    }

    protected abstract Task LoadMedia(Guid mediaId);
    public abstract Task Reload();
}