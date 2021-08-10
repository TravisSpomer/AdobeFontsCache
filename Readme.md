# Adobe Fonts Cache Tool

I got tired of Adobe Fonts constantly deactivating fonts I was using in Figma, and the mind-bogglingly high resource usage of the Adobe Creative Cloud app, so I made a tool to copy my fonts out of the cache folder. This tool only works on Windows, but could probably be easily ported to Mac OS if you replace a couple of path names.

Please don't use this for anything nefarious.

## Building

If you're not using Visual Studio, you can build a release EXE with the following ([.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) is required):

```
dotnet build --configuration Release
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true --self-contained false
```

---
© 2021 Travis Spomer. [MIT license](License.txt).
