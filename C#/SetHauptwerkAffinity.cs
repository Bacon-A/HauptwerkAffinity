using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

class SetHauptwerkAffinity
{
    /// <summary>
    /// Retrieves the handle to the current process.
    /// </summary>
    [DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentProcess();

    /// <summary>
    /// Sets the processor affinity mask for the specified process.
    /// </summary>
    [DllImport("kernel32.dll")]
    static extern IntPtr SetProcessAffinityMask(IntPtr hProcess, IntPtr dwProcessAffinityMask);

    /// <summary>
    /// Checks if the current user has administrative privileges.
    /// </summary>
    /// <returns>True if the current user is an administrator; otherwise, false.</returns>
    static bool IsAdmin()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// Sets the CPU affinity for the specified processes.
    /// </summary>
    /// <param name="processNames">An array of process names to search for.</param>
    /// <param name="affinityMask">The CPU affinity mask to set.</param>
    /// <returns>The name of the process for which affinity was set; null if no process found.</returns>
    static string SetAffinity(string[] processNames, IntPtr affinityMask)
    {
        foreach (string processName in processNames)
        {
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                try
                {
                    SetProcessAffinityMask(process.Handle, affinityMask);
                    Console.WriteLine($"Affinity set for {processName}.");
                    return processName; // Return the name of the process for which affinity was set
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }
        return null; // Return null if no process found for any of the process names
    }

    static void Main()
    {
        if (!IsAdmin())
        {
            MessageBox.Show("This application requires administrative privileges to set CPU affinity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }  

        string[] processNames = { "Hauptwerk", "Hauptwerk (alt config 1)", "Hauptwerk (alt config 2)", "Hauptwerk (alt config 3)" };
        int processorCount = Environment.ProcessorCount;
        IntPtr affinityMask = new IntPtr((1 << processorCount) - 1); // Use all available CPUs
        string processWithAffinity = SetAffinity(processNames, affinityMask);

        if (processWithAffinity != null)
        {
            MessageBox.Show($"Affinity set for process: {processWithAffinity}.", "Hauptwerk Affinity Setting");
        }
        else
        {
            string checkedProcessNames = string.Join("\n", processNames);
            MessageBox.Show($"Hauptwerk affinity not set. \nNone of the specified processes found.\n\nProcess names checked:\n{checkedProcessNames}", "Hauptwerk Affinity Setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
