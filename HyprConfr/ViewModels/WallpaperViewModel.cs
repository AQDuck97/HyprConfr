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
        get => WPManager.Wallpapers;
        set => this.RaiseAndSetIfChanged(ref _wallpapers, value);
    }
    private List<Wallpaper> _wallpapers;

    public Monitor Monitor
    {
        get => _monitor;
        set => this.RaiseAndSetIfChanged(ref _monitor, value);
    }
    private Monitor _monitor;

    public string Location
    {
        get => HomeCheck(_location);
        set => this.RaiseAndSetIfChanged(ref _location, value);
    }
    private string _location;

    public WallpaperViewModel()
    {
        _location = WPManager.ReadConf();
        _wallpapers = WPManager.GetImages(_location);
        WPManager.SetMonitors();
    }

    public void Random()
    {
        foreach (Monitor monitor in Monitors.Where(m => m.IsSelected))
        {
            monitor.Wallpaper = WPManager.Randomize();
        }
    }

    public async void Set()
    {
        await WPManager.SetWallpapers(_location);
    }

    public void Search()
    {
        Wallpapers = WPManager.GetImages(_location);
    }

    public async void Browse()
    {
        Location = await Task.Run(() => Main.DirPicker(_location));
    }

    public void SetWp(Wallpaper wp)
    {
        foreach (Monitor mon in Monitors.Where(m => m.IsSelected))
        {
            mon.Wallpaper = wp;
        }
    }

    string HomeCheck(string text)
    {
        return text.Replace($"/home/{Environment.UserName}", "~");
    }
}