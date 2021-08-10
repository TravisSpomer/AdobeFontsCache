#nullable enable
using System;
using System.IO;
using System.Xml.Linq;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Adobe Fonts Cache Tool");
Console.ResetColor();
Console.WriteLine("by Travis Spomer (GitHub @TravisSpomer)");
Console.WriteLine();

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("STEP 1: Cut a hole in the box");
Console.ResetColor();
Console.WriteLine();
if (Environment.OSVersion.Platform != PlatformID.Win32NT)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("Sorry, this app is for Windows only. I don't know where to find these files for Mac OS.");
	Console.ResetColor();
	return;
}
string destPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Adobe Fonts");
Console.WriteLine("I'm gonna copy your font files to:");
Console.WriteLine($"  {destPath}");

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("STEP 2: Put your fonts in the box");
Console.ResetColor();
Console.WriteLine();
string adobeFontsRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Adobe\CoreSync\plugins\livetype");
if (!Directory.Exists(adobeFontsRootPath))
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("Well shit.");
	Console.ResetColor();
	Console.WriteLine("I wasn't able to find your Adobe Fonts cache. Maybe...");
	Console.WriteLine(" - You don't have the Adobe Creative Cloud app running?");
	Console.WriteLine(" - You haven't synced any fonts in Adobe Fonts?");
	Console.WriteLine(" - They changed where the fonts are stored and this app is now useless?");
	return;
}
Console.WriteLine("I found your Adobe Fonts cached files here:");
Console.WriteLine($"  {adobeFontsRootPath}");

string manifestPath = Path.Combine(adobeFontsRootPath, @"c\entitlements.xml");
XDocument manifest = XDocument.Load(manifestPath);
XContainer fontList = manifest.Element("typekitSyncState")!.Element("fonts")!;

// Okay, we're ready to start the copying.
if (!Directory.Exists(destPath))
	Directory.CreateDirectory(destPath);

Console.WriteLine();
Console.WriteLine("Now I'll copy the fonts:");
Console.WriteLine();

int fontsFound = 0;
int errorsCount = 0;
foreach (var fontElement in fontList.Elements())
{
	string fontID = fontElement.Element("id")!.Value;
	XContainer properties = fontElement.Element("properties")!;
	if (properties.Element("owner")!.Value == "Self") continue;
	string installState = properties.Element("installState")!.Value;
	string fontFullName = properties.Element("fullName")!.Value.Trim();
	string fontFamilyName = properties.Element("familyName")!.Value.Trim();
	string? familyUrl = properties.Element("familyURL")?.Value;

	string sourceFilename = Path.Combine(adobeFontsRootPath, installState == "OS" ? "r" : "w", fontID);
	if (!File.Exists(sourceFilename))
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($" - {fontFullName}: font file not found");
		Console.ResetColor();
		errorsCount++;
		continue;
	}

	string destFolder = Path.Combine(destPath, fontFamilyName);
	if (!Directory.Exists(destFolder))
		Directory.CreateDirectory(destFolder);
	string destFilename = Path.Combine(destFolder, Path.ChangeExtension(fontFullName, "otf"));
	File.Copy(sourceFilename, destFilename, overwrite: true);

	// Unhide the file if necessary.
	var attributes = File.GetAttributes(destFilename);
	if ((attributes & FileAttributes.Hidden) != 0)
	{
		attributes &= ~FileAttributes.Hidden;
		File.SetAttributes(destFilename, attributes);
	}

	// Add links to Adobe Fonts if possible.
	string linkFilename = Path.Combine(destFolder, $"Adobe Fonts license.url");
	if (!File.Exists(linkFilename))
		File.WriteAllText(linkFilename, $"[InternetShortcut]\r\nURL=https://helpx.adobe.com/fonts/using/font-licensing.html");
	if (familyUrl != null)
	{
		linkFilename = Path.Combine(destFolder, $"Adobe Fonts - {fontFamilyName}.url");
		if (!File.Exists(linkFilename))
			File.WriteAllText(linkFilename, $"[InternetShortcut]\r\nURL={familyUrl}");
	}

	fontsFound++;
	Console.WriteLine($" - {fontFullName}");
}
Console.WriteLine($"{fontsFound} fonts in total.");

Console.WriteLine();
if (errorsCount > 0)
{
	Console.ForegroundColor = ConsoleColor.Yellow;
	Console.WriteLine("Done, but with errors.");
}
else
{
	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("Done!");
}
Console.ResetColor();
Console.WriteLine();
