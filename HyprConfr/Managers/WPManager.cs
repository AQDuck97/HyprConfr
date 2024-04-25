using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HyprConfr.Models;
using SixLabors.ImageSharp;

namespace HyprConfr.Managers;

public class WPManager
{
    public static List<Monitor> Monitors { get; set; } = new();
    private static readonly string _paperConf = $"{Main.Home}/.config/hypr/hyprpaper.conf";

    public static async Task SetWallpapers(string source)
    {
        string preload = "";
        string papers = "";
        Main.Kill("hyprpaper");
        Main.BackRun("hyprpaper", "");

        await Task.Delay(50);

        List<string> paths = new();

        foreach (Monitor monitor in Monitors)
        {
            try
            {
                string path = monitor.Wallpaper.Path.Replace($"/home/{Environment.UserName}", "$HOME");
                papers += $"\nwallpaper = {monitor.Name},{path}";
                paths.Add(path);

                await Main.RunAsync("hyprctl", $"hyprpaper preload \"{path}\"");
                await Main.RunAsync("hyprctl", $"hyprpaper wallpaper \"{monitor.Name},{path}\"");
            }
            catch (Exception e)
            {
                Main.Log.Status = $"Failed to set wallpaper for {monitor.Name}: {e.Message}";
            }
        }

        string confText = $"""
                           #{source}
                           {papers}
                           """;
        File.WriteAllText(_paperConf, confText);

        foreach (string path in paths)
        {
            await Task.Delay(100);
            await Main.RunAsync("hyprctl", $"hyprpaper unload \"{path}\"");
        }
    }

    public static string ReadConf()
    {
        if (Monitors.Count < 1)
            Monitors = GetMonitors();
        
        string source = $"~/Pictures/wallpapers";
        try
        {
            if (File.Exists(_paperConf))
            {
                string wpText = File.ReadAllText(_paperConf);
                source = wpText.Substring(1, wpText.IndexOf("\n") - 1);

                int idx = wpText.IndexOf("wallpaper =");

                wpText = wpText.Substring(idx, wpText.Length - idx).Replace("wallpaper = ", "");
                string[] rawLines = wpText.Split("\n");

                foreach (Monitor monitor in Monitors)
                {
                    string wp = rawLines.Where(l => l.StartsWith(monitor.Name)).FirstOrDefault();
                    wp = Main.PathCheck(
                        wp.Substring(wp.IndexOf(",") + 1, wp.Length - wp.IndexOf(",") - 1));
                    monitor.Wallpaper = new(wp);
                }
            }
        }
        catch (Exception e)
        {
            Main.Log.Status = $"Failed to read conf: {e.Message}";
        }

        return source;
    }

    public static List<Wallpaper> GetImages(string location)
    {
        List<Wallpaper> bits = new();
        try
        {
            foreach (string i in Directory.GetFiles(Main.PathCheck(location)))
            {
                string file = i;
                if (!file.EndsWith(".png"))
                {
                    file = ConvertToPng(file, location);
                }

                bits.Add(new Wallpaper(file));
            }
        }
        catch (Exception e)
        {
            Main.Log.Status = e.Message;
        }

        return bits;
    }

    private static string ConvertToPng(string file, string location)
    {
        string fName = Path.GetFileNameWithoutExtension(file);
        string png = $"{location}/{fName}.png";
        
        if (!File.Exists(png))
        {
            string target = $"{location}/jpegs";
            string targetFile = $"{target}/{Path.GetFileName(file)}";
            var stream = File.OpenRead(file);
            
            Image img = Image.Load(stream);
            img.SaveAsPng(png);
            
            Main.DirCheck(target);
            if(!File.Exists(targetFile))
                File.Move(file, targetFile);
            else
                File.Delete(file);
        }

        return png;
    }

    private static List<Monitor> GetMonitors()
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
            
            monitor.Height /= 10;
            monitor.Width /= 10;
            monitor.Model = $"{make.Substring(0, idx)} {monitor.Model}";
        }
    }
}