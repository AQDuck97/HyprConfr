using HyprConfr.Managers;
using HyprConfr.Models;

namespace HyprConfr.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // public string Greeting => "Welcome to Avalonia!";
    public WallpaperViewModel WPVM => _wpVM;
    private WallpaperViewModel _wpVM;
    public UpdaterViewModel UPVM { get; set; }

    public bool DesktopExists => Main.CheckDesktop();

    public LogModel Log => Main.Log;

    public MainWindowViewModel()
    {
        _wpVM = new WallpaperViewModel();
        UPVM = new();
    }
}