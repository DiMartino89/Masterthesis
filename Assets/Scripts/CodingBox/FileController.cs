using System.IO;
using UnityEngine;

public class FileController : IFileController
{
    private string _sourceFolder = Application.streamingAssetsPath; // Path.Combine(Application.dataPath, "Scripts");

    private string _subFolder = string.Empty;

    private string _Path { get { return Path.Combine(_sourceFolder, _subFolder); } }

    public FileController(string subFolder)
    {
        _subFolder = subFolder;

        CreateFolder(string.Empty, _Path);
    }

    public string LoadTextFromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        string path = Path.Combine(_Path, fileName);

        if (!File.Exists(path))
        {
            return string.Empty;
        }

        return File.ReadAllText(path);
    }

    public void SaveTextToFile(string fileName, string text)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        string path = Path.Combine(_Path, fileName);

        if (!File.Exists(path))
        {
            path = CreateFile(fileName);
        }

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                File.WriteAllText(path, text);
            }
            catch
            {

            }
        }
    }

    public string GetFilePath(string fileName)
    {
        string path = Path.Combine(_Path, fileName);

        if (!File.Exists(path))
        {
            return string.Empty;
        }

        return path;
    }

    private string CreateFile(string fileName)
    {
        string path = Path.Combine(_Path, fileName);

        if (!File.Exists(path))
        {
            File.Create(path);
        }

        return path;
    }

    private string CreateFolder(string path, string folderName)
    {
        path = Path.Combine(path, folderName);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}
