using System;
using System.IO;
using Scripts.Common.Log;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LogPresenter : IStartable
{
    [Inject] LogModel _model;
    public void Start()
    {
        _model.DirectoryPath = Path.Combine(Application.persistentDataPath, "File", "Logs");
        var fileName = DateTime.Now.ToString("yyMMddHHmmss");
        _model.FilePath = Path.Combine(Application.persistentDataPath, "File", "Logs", $"{fileName}.log");

        Directory.CreateDirectory(_model.DirectoryPath);
        if (!File.Exists(_model.FilePath))
        {
            using var stream = File.Create(_model.FilePath);
        }
    }
}
