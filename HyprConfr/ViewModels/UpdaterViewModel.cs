using System.Collections.Generic;
using System.Threading.Tasks;
using HyprConfr.Managers;
using HyprConfr.Managers.Updater;
using ReactiveUI;

namespace HyprConfr.ViewModels;

public class UpdaterViewModel : ViewModelBase
{
    public string BtnTxt
    {
        get => _btnTxt;
        set => this.RaiseAndSetIfChanged(ref _btnTxt, value);
    }
    private string _btnTxt = "Check updates";

    public string Result
    {
        get => _result;
        set => this.RaiseAndSetIfChanged(ref _result, value);
    }
    private string _result = "Checking...";
    
    public List<ReleaseModel> Releases
    {
        get => _releases;
        set => this.RaiseAndSetIfChanged(ref _releases, value);
    }
    private List<ReleaseModel> _releases;
    
    private ReleaseManager _manager;
    private string _url;

    public UpdaterViewModel()
    {
        _url = Main.Conf.UpdateUrl;
        _manager = new();
        if(Main.Conf.AutoUpdate)
            Check();
    }

    public async void Check()
    {
        BtnTxt = Main.Version();
        Result = await Task.Run(() => _manager.CheckUpdates(_url, Main.Version()));
        Releases = _manager.Releases;
    }
}