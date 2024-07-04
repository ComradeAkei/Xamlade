using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Gif;
using Avalonia.Interactivity;

namespace Xamlade;

public static class TestWindow
{
    public static GifImage LoadingGif { get; set; }

    public static void Init(GifImage gifImage) => LoadingGif = gifImage; 
    public static async void RUN_WINDOW(object? sender, RoutedEventArgs e)
    {
        CopyAssetsToProject();
        LoadingGif.IsVisible = true;
        
        await ExecuteLinuxCommandAsync(@"XamladeDemo/BUILD.sh");
        LoadingGif.IsVisible = false;
        await ExecuteLinuxCommandAsync(@"XamladeDemo/RUN.sh");
    }


    public static void CopyAssetsToProject()
    {
        string[] files = Directory.GetFiles(@"assets");
        string targetDirectory = @"XamladeDemo/assets";
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(targetDirectory, fileName);
            File.Copy(file, destFile, true);
        }
    }
    public static async Task<string> ExecuteLinuxCommandAsync(string command)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string result = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Error: {error}");
            }

            return result;
        }
    }
}