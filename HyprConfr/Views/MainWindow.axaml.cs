using Avalonia.Controls;
using HyprConfr.Managers;
using HyprConfr.ViewModels;

namespace HyprConfr.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Main.Win = this;
        Main.OnLaunch();
    }
}