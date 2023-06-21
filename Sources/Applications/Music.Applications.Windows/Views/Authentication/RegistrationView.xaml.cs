using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Authentication;
using Music.Shared.Identity.Common.Requests;
using MusicClient.Exceptions;

namespace Music.Applications.Windows.Views.Authentication;

public static partial class ValidatorExtensions
{
    public static bool IsValidEmailAddress(this string s)
    {
        var regex = MyRegex();
        return regex.IsMatch(s);
    }

    [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
    private static partial Regex MyRegex();
}

public partial class RegistrationView : UserControl
{
    private readonly MainViewModel _model;

    public RegistrationView()
    {
        InitializeComponent();
        _model = App.ServiceProvider.GetRequiredService<MainViewModel>();
    }

    private void DisplayErrorMessage(string message)
    {
        // TODO ДОБАВИТЬ СЛОВАРЬ С ОШИБКАМИ, ENUM

        if (_model.Navigation.CurrentView is not RegistrationViewModel registrationViewModel) return;
        registrationViewModel.ChangeErrorMessage(message);
    }

    private bool CheckData(string username, string email, string password, string secondPassword)
    {
        if (username == "")
        {
            DisplayErrorMessage("Username is empty");
            return false;
        }

        if (email == "")
        {
            DisplayErrorMessage("Email is empty");
            return false;
        }

        if (password == "")
        {
            DisplayErrorMessage("Password is empty");
            return false;
        }

        if (username.Contains(" "))
        {
            DisplayErrorMessage("Username can't contain white spaces!");
            return false;
        }

        if (username.Length < 3)
        {
            DisplayErrorMessage("Username length can't be less than 3!");
            return false;
        }

        if (!email.IsValidEmailAddress())
        {
            DisplayErrorMessage("Invalid email!");
            return false;
        }

        if (password.Length < 8)
        {
            DisplayErrorMessage("Password length can't be less than 8!");
            return false;
        }

        if (password != secondPassword)
        {
            DisplayErrorMessage("Passwords are different!");
            return false;
        }

        return true;
    }

    private async void TryRegister()
    {
        var username = UsernameText.Text;
        var password = PasswordText.Box.Password;
        var secondPassword = RepeatPasswordText.Box.Password;
        var email = EmailText.Text;

        if (!CheckData(username, email, password, secondPassword))
            return;

        try
        {
            var registerDto = await App.GetService().Register(new RegisterRequest(username, email, password));
            NotificationEvents.DisplayNotification("Registration completed", "Registration");
            var signInModel = new AuthenticationViewModel(registerDto.Username);
            _model.Navigation.SetCurrentView(signInModel);
        }
        catch (ServerUnavailableException)
        {
            DisplayErrorMessage("Server is unavailable");
        }
        catch (Exception ex)
        {
            DisplayErrorMessage(ex.Message);
        }
    }

    private void UsernameText_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            TryRegister();
    }

    private void PasswordText_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            TryRegister();
    }

    private void SignInTextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var signInModel = new AuthenticationViewModel(UsernameText.Text);
        _model.Navigation.SetCurrentView(signInModel);
    }

    private void SignUpButton_OnClicked(object sender, RoutedEventArgs e)
    {
        TryRegister();
    }
}