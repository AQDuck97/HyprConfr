using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DynamicData;
using HyprConfr.Models;

namespace HyprConfr.Managers;

public class WPManager
{
    public static List<Monitor> Monitors { get; set; } = new();
    public static Monitor? SelectedMonitor { get; set; }

    public static async Task SetWallpapers(string source)
    {
        string preload = "";
        string papers = "";
        MainManager.Kill("hyprpaper");
        MainManager.BackRun("hyprpaper","");

        await Task.Delay(50);

        List<string> paths = new();

        foreach (Monitor monitor in Monitors)
        {
            try
            {

                string path = monitor.Wallpaper.Path.Replace($"/home/{Environment.UserName}", "$HOME");
                papers += $"\nwallpaper = {monitor.Name},{path}";
                paths.Add(path);

                await MainManager.RunAsync("hyprctl", $"hyprpaper preload \"{path}\"");
                await MainManager.RunAsync("hyprctl", $"hyprpaper wallpaper \"{monitor.Name},{path}\"");
            }
            catch (Exception e)
            {
                MainManager.Log.Status = $"Failed to set wallpaper for {monitor.Name}: {e.Message}";
            }
        }
        string confText = $"""
                           #{source}
                           {papers}
                           """;
        File.WriteAllText($"/home/{Environment.UserName}/.config/hypr/hyprpaper.conf", confText);
        
        foreach (string path in paths)
        {
            await Task.Delay(100);
            await MainManager.RunAsync("hyprctl", $"hyprpaper unload \"{path}\"");
        }
    }

    public static string ReadConf()
    {
        if (Monitors.Count < 1)
            Monitors = GetMonitors();
        
        string file = $"/home/{Environment.UserName}/.config/hypr/hyprpaper.conf";
        string source = $"~/Pictures/wallpapers";
        try
        {
            if (File.Exists(file))
            {
                string wpText = File.ReadAllText(file);
                source = wpText.Substring(1, wpText.IndexOf("\n") - 1);

                int idx = wpText.IndexOf("wallpaper =");

                wpText = wpText.Substring(idx, wpText.Length - idx).Replace("wallpaper = ", "");
                string[] rawLines = wpText.Split("\n");

                foreach (Monitor monitor in Monitors)
                {
                    string wp = rawLines.Where(l => l.StartsWith(monitor.Name)).FirstOrDefault();
                    wp = wp.Substring(wp.IndexOf(",") + 1, wp.Length - wp.IndexOf(",") - 1)
                        .Replace("$HOME", $"/home/{Environment.UserName}");
                    monitor.Wallpaper = new(wp);
                }
                // await MainManager.RunAsync("hyprctl", $"hyprpaper unload \"{path}\"");
            }
        }
        catch (Exception e)
        {
            MainManager.Log.Status = $"Failed to read conf: {e.Message}";
        }

        return source;
    }
    
    public static List<Wallpaper> GetImages(string location)
    {
        List<Wallpaper> bits = new();
        try
        {
            foreach (string i in Directory.GetFiles(location
                         .Replace("~", $"/home/{Environment.UserName}"))
                         .Where(w => w.EndsWith(".png")))
            {
                    bits.Add(new Wallpaper(i));
            }
        }
        catch (Exception e)
        {
            MainManager.Log.Status = e.Message;
        }

        return bits;
    }

    public static List<Monitor> GetMonitors()
    {
        List<Monitor> monitors = new();
        ProcessStartInfo psi = new()
        {
            FileName = "/bin/hyprctl",
            Arguments = "monitors -j",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        
        Process proc = Process.Start(psi);
        proc.WaitForExit();
        
        string json = proc.StandardOutput.ReadToEnd();

        monitors = JsonSerializer.Deserialize<List<Monitor>>(json);

        return monitors.OrderBy(m => m.Y).ThenBy(m => m.X).ToList();
    }

    public static void SetMonitors()
    {
        foreach (Monitor monitor in Monitors)
        {
            string make = monitor.Make;
            int idx = make.Length;
            if (make.Contains(" "))
                idx = make.IndexOf(" ");
            // monitor.Wallpaper = _wallpapers.FirstOrDefault();
            monitor.Height /= 10;
            monitor.Width /= 10;
            monitor.Model = $"{make.Substring(0, idx)} {monitor.Model}";
        }
    }
}