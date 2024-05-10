using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Avalonia.Media.Imaging;
using HyprConfr.Managers;

namespace HyprConfr.Models;

public class Monitor : INotifyPropertyChanged
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("width")]
    public int Width { get; set; }
    [JsonPropertyName("height")]
    public int Height { get; set; }
    [JsonPropertyName("x")]
    public int X { get; set; }
    [JsonPropertyName("y")]
    public int Y { get; set; }
    [JsonPropertyName("make")]
    public string Make { get; set; }
    [JsonPropertyName("model")]
    public string Model { get; set; }

    public bool RandomizeWP
    {
        get => _rndWP;
        set
        {
            _rndWP = value;
            OnPropertyChanged();
        }
    }
    private bool _rndWP;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }
    private bool _isSelected;
    public Wallpaper? Wallpaper
    {
        get { return _wallpaper; }
        set
        {
            _wallpaper = value;
            OnPropertyChanged();
        }
    }

    private Wallpaper? _wallpaper;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}