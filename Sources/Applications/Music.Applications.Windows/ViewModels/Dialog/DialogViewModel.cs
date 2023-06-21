using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;

namespace Music.Applications.Windows.ViewModels.Dialog;

public class DialogViewModel : DialogViewModelBase
{
    public override string ModelName { get; protected set; } = nameof(DialogViewModel);

    public static void OpenGlobal(string title, ViewModelBase content)
    {
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();
        model.DialogViewModel.Open(title, content);
    }
}