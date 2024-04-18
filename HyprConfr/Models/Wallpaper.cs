using System.Diagnostics;
using System.IO;
using Avalonia.Media.Imaging;
using HyprConfr.Managers;

namespace HyprConfr.Models;

public class Wallpaper
{
    public string Path { get; set; }
    public Bitmap Bit { get; set; }
    public Wallpaper WP { get; set; }

    public Wallpaper(string path)
    {
        Path = path;
        Bit = Img(path);
        WP = this;
    }

    Bitmap Img(string path)
    {
        var stream = File.OpenRead(path);
        
        return Bitmap.DecodeToWidth(stream, 304, BitmapInterpolationMode.MediumQuality);
    }

    // public void Command()
    // {
    //     WPManager.SelectedMonitor.Wallpaper = this;
    // }
}