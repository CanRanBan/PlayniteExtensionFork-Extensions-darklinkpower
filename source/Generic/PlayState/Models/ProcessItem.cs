using System.Diagnostics;

namespace PlayState.Models
{
    public class ProcessItem
    {
        public string ExecutablePath;
        public Process Process;

        public ProcessItem(Process process, string executablePath)
        {
            ExecutablePath = executablePath;
            Process = process;
        }
    }
}