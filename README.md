# HyprConfr

## GUI configurator for HyprLand written in .NET 8 with Avalonia UI

![image](https://github.com/AQDuck97/HyprConfr/assets/102743075/0dc6b1e4-59e8-4b7b-843c-140badbdd543)


## Features (as of v0.1.0):
### Wallpaper manager
⚠️ [Requires HyprPaper](https://github.com/hyprwm/hyprpaper) ⚠️
* Supports multiple monitors
* Uses `~/.config/hypr/hyprpaper.conf` file
* Cleanly manages HyprPaper by preloading and unloading wallpapers so they don't clog memory


## CLI usage:
Launching with any option will kill the app once it's done the thing
                    -h  Prints this text
                    -v  Prints version
                    -wp Sets wallpapers according to hyprpaper.conf and runs HyprPaper
                        
Recommended to set `exec-once = /path/to/HyprConfr.AppImage -wp` in your hyprland.conf (or where you set autolaunch)
HyprConfr will clean up HyprPaper by unloading the images once it's set up. 
Much lower memory usage than preloading from .conf file (down from 300MB to 14MB on my setup)

## Running:
The app is self-contained and does not require `dotnet-runtime` to be installed

Non-Ubuntu users should be able to just run the AppImage, may have to make it executable with `chmod +x`
If it doesn't work, make sure you have `fuse2` installed.

## Building:
Dependencies: `dotnet-sdk 8.0` or later

```sh
git clone https://github.com/AQDuck97/HyprConfr/
cd HyprConfr/HyprConfr
dotnet publish -c Release --self-contained -o /path/to/where/you/want
```
