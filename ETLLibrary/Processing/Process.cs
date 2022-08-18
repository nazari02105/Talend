using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ETLLibrary.Enums;
using ETLLibrary.Model.Pipeline;

namespace ETLLibrary.Processing
{
    public class Process
    {
        private static List<Process> _allProcesses = new List<Process>();

        public string ErrorMessage;
        private string _username;
        private string _pipelineName;
        private Pipeline _pipeline;
        public Thread MyThread;
        public Status Status { get; set; }

        public Process(string username, string pipelineName, Pipeline pipeline)
        {
            _pipeline = pipeline;
            _pipelineName = pipelineName;
            _username = username;
            ErrorMessage = "An error occured during running process.";
        }

        public Process()
        {
        }

        public void Run()
        {
            try
            {
                Status = Status.Running;
                _pipeline.Run();
                // Thread.Sleep(10000);
                // throw new Exception();
                Status = Status.Finished;
            }
            catch (Exception e)
            {
                Status = Status.Failed;
                Console.WriteLine(e);
                ErrorMessage = e.Message;
            }
        }

        public void Start()
        {
            var threadStart = new ThreadStart(this.Run);
            MyThread = new Thread(threadStart);
            MyThread.Start();
        }

        public static void AddToProcesses(Process process)
        {
            _allProcesses.Add(process);
        }

        public static Process GetProcess(string username, string pipelineName)
        {
            for (int i = _allProcesses.Count - 1; i >= 0; --i)
            {
                if (_allProcesses[i]._username == username && 
                    _allProcesses[i]._pipelineName == pipelineName) return _allProcesses[i];
            }

            return new Process() {Status = Status.NotRunning};
        }

        public static bool RunningProcessExists(string username)
        {
            return  _allProcesses.Any(x => x._username == username && x.Status == Status.Running);
        }

        public static void DeleteFromProcesses(string username)
        {
            _allProcesses.RemoveAll(x => x._username == username);
        }

        public static void CancelProcess(string username)
        {
            var process = _allProcesses.Single(x => x._username == username && x.Status == Status.Running);
            process._pipeline.Cancel();
            _allProcesses.Remove(process);
        }
    }
}