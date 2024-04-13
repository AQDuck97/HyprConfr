using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using HyprConfr.Managers;
using HyprConfr.Models;
using ReactiveUI;

namespace HyprConfr.ViewModels;

public class WallpaperViewModel : ViewModelBase
{
    
    public List<Monitor> Monitors => WPManager.Monitors;

    public List<Wallpaper> Wallpapers
    {
        get => _wallpapers;
        set => this.RaiseAndSetIfChanged(ref _wallpapers, value);
    }
    private List<Wallpaper> _wallpapers;

    public string Location
    {
        get => HomeCheck(_location);
        set => this.RaiseAndSetIfChanged(ref _location, value);
    }
    private string _location;

    public bool DesktopExists
    {
        get => _desktopExists;
        set => this.RaiseAndSetIfChanged(ref _desktopExists, value);
    }
    private bool _desktopExists;
    
    public LogModel Log => MainManager.Log;

    public WallpaperViewModel()
    {
        _location = WPManager.ReadConf();
        _wallpapers = WPManager.GetImages(_location);
        WPManager.SetMonitors();
        DesktopExists = MainManager.CheckDesktop();
    }

    public void Search()
    {
        Wallpapers = WPManager.GetImages(_location);
    }

    public async void Set()
    {
        await WPManager.SetWallpapers(_location);
    }

    public void AddDesktop()
    {
        MainManager.CreateDesktop();
        DesktopExists = MainManager.CheckDesktop();
    }

    string HomeCheck(string text)
    {
        return text.Replace($"/home/{Environment.UserName}", "~");
    }
}