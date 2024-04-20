using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HyprConfr.Managers.Updater;
using HyprConfr.Models;

namespace HyprConfr.Managers;

public class Main
{
    public static Window Win { get; set; }
    public static LogModel Log { get; set; } = new();
    public static ConfModel Conf { get; set; } = new();
    private static ReleaseManager _releaseManager = new();
    private static string _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    private static readonly string ConfPath = $"/home/{Environment.UserName}/.config/hyprconfr";
    public static readonly string Home = $"/home/{Environment.UserName}";

    public static void OnLaunch()
    {
        // Log.Version = Version();
        // LoadConf();
        // CheckUpdate();
    }

    public static string PathCheck(string path)
    {
        return path.Replace($"~", $"{Home}")
            .Replace("$HOME", $"{Home}");
    }
    
    public static string HomeCheck(string path)
    {
        return path.Replace($"{Home}", "~");
    }
    
    public static string Version()
    {
         return $"v{_version.Substring(0, _version.LastIndexOf("."))}";
    }

    public static async void CheckUpdate()
    {
        Log.Status = await Task.Run(() => _releaseManager.CheckUpdates(Conf.UpdateUrl, Version()));
    }
    
    public static void Kill(string name)
    {
        try
        {
            Process[] procs = Process.GetProcessesByName(name);
            if(procs.Length > 0)
                procs[0].Kill();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public static void BackRun(string app, string args)
    {
        string command = $"nohup /bin/{app} {args} >/dev/null 2>&1 &";
        
        ProcessStartInfo psi = new()
        {
            FileName = $"/bin/bash",
            Arguments = $"-c \"{command}\"",
            UseShellExecute = true,
        };

        Process.Start(psi);
    }

    public static async Task<string> RunAsync(string app, string args)
    {
        ProcessStartInfo psi = new()
        {
            FileName = $"/bin/{app}",
            Arguments = $"{args}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        };

        Process proc = new();
        proc.StartInfo = psi;
        proc.Start();
        await proc.WaitForExitAsync();
        
        string output = await proc.StandardOutput.ReadToEndAsync();
        Console.WriteLine($"{app} {args}\n{output}");
        return output;
    }

    public static bool CheckDesktop()
    {
        if (File.Exists($"/home/{Environment.UserName}/.local/share/applications/hyprconfr.desktop"))
            return true;
        return false;
    }

    public static void LoadConf()
    {
        try
        {
            if (File.Exists($"{ConfPath}/conf.json"))
            {
                Conf = JsonSerializer.Deserialize<ConfModel>(File.ReadAllText($"{ConfPath}/conf.json"));
                Log.Status = "Loaded config";
            }
            else
                Log.Status = "No conf file found";
        }
        catch (Exception e)
        {
            Log.Status = e.Message;
        }
    }

    public void SaveConf(ConfModel confModel)
    {
        try
        {
            DirCheck($"{ConfPath}");
            Conf = confModel;
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            string content = JsonSerializer.Serialize(confModel, options);
            File.WriteAllText($"{ConfPath}/conf.json", content);
        }
        catch (Exception e)
        {
            Log.Status = e.Message;
        }
    }

    static void DirCheck(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public static void CreateDesktop()
    {
        string appsDir = $"{Home}/.local/share/applications";
        
        string dir = Directory.GetCurrentDirectory();
        string appName = $"{Directory.GetFiles(dir).FirstOrDefault(a => a.ToLower().Contains($"hyprconfr.app"))}";
        
        DirCheck(appsDir);
        
        string content = $"""
                          [Desktop Entry]
                          Encoding=UTF-8
                          Version={_version}
                          Type=Application
                          Terminal=false
                          Exec={appName}
                          Icon={ConfPath}/hyprconfr.png
                          Name=HyprConfr
                          Description=Configuration tool for Hyprland
                          """;
        File.WriteAllText($"{appsDir}/hyprconfr.desktop", content);
        Log.Status = $"Added .desktop file to {HomeCheck(appsDir)}";
    }

    public static async Task<string> DirPicker(string dir)
    {
        TopLevel top = TopLevel.GetTopLevel(Win);

        FolderPickerOpenOptions options = new();
        options.AllowMultiple = false;
        options.SuggestedStartLocation = await top.StorageProvider.TryGetFolderFromPathAsync(new Uri(Home));

        var folder = await top.StorageProvider.OpenFolderPickerAsync(options);

        if (folder.Count > 0)
            return folder[0].Path.ToString().Replace("file://", "");
        
        return dir;
    }
}