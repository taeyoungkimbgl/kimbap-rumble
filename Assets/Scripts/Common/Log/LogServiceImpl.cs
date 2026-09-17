using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Scripts.Common.Log
{
    public class LogServiceImpl : ILogService
    {
        [Inject] LogModel _model;

        public void Write(string content)
        {
            string line = $"[{DateTime.Now:HHmmss}]{content ?? string.Empty}{Environment.NewLine}";
            File.AppendAllText(_model.FilePath, line, Encoding.UTF8);
        }
    }
}
