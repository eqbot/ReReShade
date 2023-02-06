using IniParser;

Console.Write("Enter the path to your FFXIV installation");
string? installPath = Console.ReadLine();

if(installPath == null)
{
    Console.WriteLine("ok don't then bye");
    Console.ReadLine();
    return;
}

//locate FFXIV game directory
if (File.Exists(installPath) && Path.GetFileName(installPath) == "ffxiv_dx11.exe")
{
    //we were given path to ffxiv exe
    installPath = Path.GetDirectoryName(installPath);
}
else if (Directory.Exists(installPath))
{
    //we were given a folder
    if (Directory.Exists(Path.Combine(installPath, "game")))
    {
        //the folder has 'game' folder in it, it was probably the top-level FFXIV directory
        installPath = Path.Combine(installPath, "game");
    }
}
else
{
    Console.WriteLine("Entered path does not exist.");
    return;
}

//validate that the things we expect to see in our path are there
if(installPath == null)
{
    Console.WriteLine("Got back a null install path after trying to locate FFXIV. Probably my bad.");
    return;
}

if (!File.Exists(Path.Combine(installPath, "ffxiv_dx11.exe")))
{
    Console.WriteLine("Unable to locate ffxiv_dx11.exe. Please ensure the path entered was correct.");
    return;
}

if(!File.Exists(Path.Combine(installPath, "GShade.ini")))
{
    Console.WriteLine("Unable to locate GShade ini. Please ensure it is actually installed on the selected FFXIV instance.");
    return;
}

if(Directory.Exists(Path.Combine(installPath, "gshade-shaders")))
{
    Console.WriteLine("The 'gshade-shaders' folder already exists in your FFXIV directory. Please move or get rid of it before running this installer.");
    return;
}

//read GShade ini to find the shaders
var iniParser = new FileIniDataParser();
var data = iniParser.ReadFile(Path.Combine(installPath, "GShade.ini"));
var GShadeEffectSearchPathsString = data["GENERAL"]["EffectSearchPaths"];
var GShadeEffectSearchPaths = GShadeEffectSearchPathsString.Split(',');
string? GShadeShadersPath = null, GShadeComputeShadersPath = null;
foreach(var path in GShadeEffectSearchPaths)
{
    if(Path.GetFileName(path) == "**")
    {
        //this is a custom shaders path, do nothing
        continue;
    }
    if(Path.GetFileName(path) == "Shaders")
    {
        GShadeShadersPath = path;
    }
    if(Path.GetFileName(path) == "ComputeShaders")
    {
        GShadeComputeShadersPath = path;
    }
}
if(GShadeShadersPath == null || GShadeComputeShadersPath == null)
{
    Console.WriteLine("Unable to read GShade shaders location from INI. They may have changed the format or some other issue may have occurred.");
    return;
}

//unhook GShade and copy shaders
var newShadersFolder = Path.Combine(installPath, "gshade-shaders");
Directory.CreateDirectory(newShadersFolder);
Directory.CreateDirectory(Path.Combine(newShadersFolder, "Shaders"));
foreach(var file in Directory.EnumerateFiles(GShadeShadersPath))
{
    File.Copy(file, Path.Combine(newShadersFolder, "Shaders", Path.GetFileName(file)));
}
Directory.CreateDirectory(Path.Combine(newShadersFolder, "ComputeShaders"));
foreach (var file in Directory.EnumerateFiles(GShadeComputeShadersPath))
{
    File.Copy(file, Path.Combine(newShadersFolder, "ComputeShaders", Path.GetFileName(file)));
}
if(File.Exists(Path.Combine(installPath, "dxgi.dll")))
{
    File.Delete(Path.Combine(installPath, "dxgi.dll"));
}

//prompt user to install ReShade
//TODO: do this ourselves later lol
Console.WriteLine("Please install ReShade (with addons) onto FFXIV manually and press Enter when it is complete.");
Console.ReadLine();

//hook ReShade up to our copied shaders folder
var reshadeIniData = iniParser.ReadFile(Path.Combine(installPath, "ReShade.ini"));
reshadeIniData["GENERAL"]["EffectSearchPaths"] = reshadeIniData["GENERAL"]["EffectSearchPaths"] + ',' + Path.Combine(newShadersFolder, "Shaders") + ',' + Path.Combine(newShadersFolder, "ComputeShaders");
iniParser.WriteFile(Path.Combine(installPath, "ReShade.ini"), reshadeIniData);

//done!