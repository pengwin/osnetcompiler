using System;
using System.Diagnostics;
using System.Linq;

namespace Compiler
{
    public class CommandExecutor
    {

        private readonly string _command;

        private Process _process;

        public CommandExecutor(string command)
        {
            _command = command;
        }

        public CommandExecutor Start(string[] commandParams)
        {
            _process = new Process();

            _process.StartInfo.FileName = _command;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.Arguments = string.Join(" ", commandParams);
            _process.Start();

            return this;

            /*cmd.StandardInput.WriteLine("echo 123");
            cmd.StandardInput.Flush();
            cmd.StandardInput.WriteLine("exit");
            cmd.StandardInput.Flush();*/
            //Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }

        public CommandExecutor SendCommand(string command)
        {
            CheckProcess();

            _process.StandardInput.WriteLine(command);
            _process.StandardInput.Flush();

            return this;
        }

        public CommandExecutor Wait(int seconds)
        {
            CheckProcess();
            System.Threading.Thread.Sleep(seconds*1000);

            return this;
        }

        public CommandExecutor WaitLastOutputLine(string lineContent)
        {
            CheckProcess();
            var lastLine = _process.StandardOutput.ReadLine();
            while(lastLine != lineContent) {
                System.Threading.Thread.Sleep(100);
                lastLine = _process.StandardOutput.ReadLine();
            }

            return this;
        }

        public CommandExecutor WaitCommandResult(string command, string result)
        {
            CheckProcess();
            SendCommand(command);

            Func<string> readLine = () => {
                var lastLine = _process.StandardOutput.ReadLine();
                if (lastLine.StartsWith("(qemu)")) { //skip qemu prompt
                    lastLine = _process.StandardOutput.ReadLine();
                }
                return lastLine;
            };

            
            while(readLine() != result) {
                SendCommand(command);
                System.Threading.Thread.Sleep(100);
            }

            return this;
        }

        public CommandExecutor WaitForEnd()
        {
            CheckProcess();
            _process.WaitForExit();

            return this;
        }

        public CommandExecutor Stop()
        {
            CheckProcess();

            _process.Kill();

            return this;
        }

        public string ToTextOutput()
        {
            CheckProcess();
            return _process.StandardOutput.ReadToEnd();
        }

        public string ToErrorOutput()
        {
            CheckProcess();
            return _process.StandardError.ReadToEnd();
        }

        private void CheckProcess()
        {
            if (_process == null)
            {
                throw new Exception("Process is not running");
            }
        }
    }
}