using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HyprConfr.Models;

public class ConfModel : INotifyPropertyChanged
{
    public string SourcePath
    {
        get => _sourcePath;
        set
        {
            _sourcePath = value;
            OnPropertyChanged();
        }
    }
    private string _sourcePath;

    public string UpdateUrl
    {
        get => _updateUrl;
        set
        {
            _updateUrl = value;
            OnPropertyChanged();
        }
    }
    private string _updateUrl;

    public bool AutoUpdate
    {
        get => _autoUpdate;
        set
        {
            _autoUpdate = value;
            OnPropertyChanged();
        }
    }
    private bool _autoUpdate;
    
    
    
    
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