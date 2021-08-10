# Adobe Fonts Cache Tool

I got tired of Adobe Fonts constantly deactivating fonts I was using in Figma, and the mind-bogglingly high resource usage of the Adobe Creative Cloud app, so I made a tool to copy my fonts out of the cache folder. This tool only works on Windows, but could probably be easily ported to Mac OS if you replace a couple of path names.

Please don't use this for anything nefarious. This tool isn't useful to you if you aren't an Adobe Creative Cloud subscriber.

## Usage

1. Install [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Run `AdobeFontsCache.exe` to extract the fonts to a folder on your desktop
3. Feel free to deactivate all of your fonts, remove the Adobe Creative Cloud Desktop app, and then just install those fonts locally

Enjoy your 480 MB of RAM savings by not having the CC app's 14 processes running!

## Building

If you're not using Visual Studio, you can build a release EXE with the following ([.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) is required):

```
dotnet build --configuration Release
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true --self-contained false
```

---
© 2021 Travis Spomer. [MIT license](License.txt).
