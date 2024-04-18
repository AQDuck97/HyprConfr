using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HyprConfr.Managers;
using HyprConfr.Models;
using HyprConfr.ViewModels;
using HyprConfr.Views;

namespace HyprConfr;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (!HasArgs(desktop.Args))
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    bool HasArgs(string[]? args)
    {
        if (args.Length > 0)
        {
            Switcher(args);
            return true;
        }
        return false;
    }

    async void Switcher(string[]? args)
    {
        foreach (string arg in args)
        {
            switch (arg)
            {
                case "-v":
                    Console.WriteLine($"HyprConfr {Main.Version()}");
                    break;
                case "-wp":
                    await WPManager.SetWallpapers(WPManager.ReadConf());
                    break;
                case "-h":
                    Console.WriteLine(Help());
                    break;
                
            }
        }
        Environment.Exit(0);
    }

    string Help()
    {
        return $"""
                HyprConfr {Main.Version()}
                
                Launching with any option will kill the app once it's done the thing
                    -h  Prints this text
                    -v  Prints version
                    -wp Sets wallpapers according to hyprpaper.conf and runs HyprPaper
                        
                Recommended to set "exec-once = {Process.GetCurrentProcess().MainModule.FileName} -wp"
                in your hyprland.conf (or where you set autolaunch)
                HyprConfr will clean up HyprPaper by unloading the images once it's set up
                
                """;
    }
}
