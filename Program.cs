#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Adobe Fonts Cache Tool");
Console.ResetColor();
Console.WriteLine("by Travis Spomer (GitHub @TravisSpomer)");
Console.WriteLine();

WriteHeader("STEP 1: Cut a hole in the box");
if (Environment.OSVersion.Platform != PlatformID.Win32NT)
{
	WriteError("Sorry, this app is for Windows only. I don't know where to find these files for Mac OS.");
	return;
}
string destPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Adobe Fonts");
Console.WriteLine("I'm gonna put your cached font files in:");
Console.WriteLine($"  {destPath}");

Console.WriteLine();
WriteHeader("STEP 2: Put your fonts in the box");
string adobeFontsRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Adobe\CoreSync\plugins\livetype");
if (!Directory.Exists(adobeFontsRootPath))
{
	WriteError("Well shit.");
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
	string fontFullName = properties.Element("fullName")!.Value;
	string fontFamilyName = properties.Element("familyName")!.Value;

	string sourceFilename = Path.Combine(adobeFontsRootPath, installState == "OS" ? "r" : "w", fontID);
	if (!File.Exists(sourceFilename))
	{
		WriteError($" - {fontFullName}: font file not found");
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

// ------------------------------------------------------------

void WriteHeader(string headerText)
{
	Console.ForegroundColor = ConsoleColor.Yellow;
	Console.WriteLine(headerText);
	Console.ResetColor();
	Console.WriteLine();
}

void WriteError(string errorText)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine(errorText);
	Console.ResetColor();
}
