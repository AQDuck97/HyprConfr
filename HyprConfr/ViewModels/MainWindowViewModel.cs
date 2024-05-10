using HyprConfr.Managers;
using HyprConfr.Models;
using ReactiveUI;

namespace HyprConfr.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // public string Greeting => "Welcome to Avalonia!";
    public WallpaperViewModel WPVM => _wpVM;
    private WallpaperViewModel _wpVM;
    public UpdaterViewModel UPVM { get; set; }

    public bool DesktopExists
    {
        get => _desktopExists;
        set => this.RaiseAndSetIfChanged(ref _desktopExists, value);
    }
    private bool _desktopExists;

    public LogModel Log => Main.Log;

    public MainWindowViewModel()
    {
        _wpVM = new WallpaperViewModel();
        DesktopExists = Main.CheckDesktop();
        UPVM = new();
    }

    public void AddDesktop()
    {
        Main.CreateDesktop();
        DesktopExists = Main.CheckDesktop();
    }

    public void Dismiss()
    {
        Log.Status = null;
    }
}