using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace HyprConfr.Managers.Updater;

public class ReleaseManager
{
    public List<ReleaseModel> Releases { get; set; }
    private static string response;

    public ReleaseManager()
    {
    }

    public async Task<string> CheckUpdates(string url, string version)
    {
        try
        {
            url = APIurl(url);
            if (url.Contains("github"))
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
                Releases = await client.GetFromJsonAsync<List<ReleaseModel>>(url);
            }
            
            string tag = Releases[0].Tag;

            if(Clean(version) < Clean(tag))
            {
                return $"Update available: {tag} (current: {version})";
            }

            return $"latest version {version}";
        }
        catch (Exception e)
        {
            return $"Failed update check:\n{e.Message}";
        }
    }

    private int Clean(string text)
    {
        return int.Parse(text.ToLower()
            .Replace("v", "")
            .Replace(".", ""));
    }

    private string APIurl(string url)
    {
        if (!url.Contains("github.com"))
        {
            url = $"https://github.com/{url}";
        }
        
        string newUrl = url.Replace("//github.com", "//api.github.com/repos");
        return $"{newUrl}/releases";
    }
}