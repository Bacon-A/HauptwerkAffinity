"""
This module looks for a running session of Hauptwerk (the main, or one of the alternate configs),
and if one is running it will set the affinity to all CPU's. This tends to fix small CPU spikes.
One way to run this is create a shortcut with the following target 
"C:\Windows\System32\cmd.exe /c start /min python "[ScriptLocation]\HW_Affinity.py""
In the shortcut, set "Run:" to "Minimized", and no terminal will show. If a running session is found,
it will indicate that the affinity was set. If no session is found, it will let you know and list the
process names it was searching for ("Hauptwerk.exe", "Hauptwerk (alt config 1).exe", etc..).
"""

import psutil
from tkinter import messagebox

def set_affinity(process_name, affinity_mask):
    """
    Set CPU affinity for a given process.

    Parameters:
        process_name (str): The name of the process to search for.
        affinity_mask (list): List of CPU IDs to set affinity to.

    Returns:
        str: A message indicating whether the affinity was set or if the process was not found.
    """
    for process in psutil.process_iter(['pid', 'name']):
        if process.info['name'] == process_name:
            try:
                process_obj = psutil.Process(process.info['pid'])
                process_obj.cpu_affinity(affinity_mask)
                return f"Affinity set for {process_name}."
            except psutil.NoSuchProcess as e:
                return f"Error: {e}"
    return f"Process {process_name} not found."

def main():
    """
    Main function to set CPU affinity for Hauptwerk processes.

    This function iterates through a list of process names associated with Hauptwerk
    and sets CPU affinity for any running process found. It then displays a message box
    indicating whether the affinity was set or if no processes were found.

    Returns:
        None
    """
    process_names = ["Hauptwerk.exe", "Hauptwerk (alt config 1).exe", "Hauptwerk (alt config 2).exe", "Hauptwerk (alt config 3).exe"]
    affinity_mask = list(range(psutil.cpu_count()))  # Use all available CPUs

    success_message_shown = False

    for process_name in process_names:
        result_message = set_affinity(process_name, affinity_mask)

        # Display a message box if one process is running and the affinity is set
        if "Affinity set" in result_message:
            messagebox.showinfo("Hauptwerk Affinity Setting", result_message)
            success_message_shown = True

    # Show a failure message if none of the processes are found
    if not success_message_shown:
        checked_process_names = "\n".join(process_names)
        messagebox.showerror("Hauptwerk Affinity Setting", 
                             f"Hauptwerk affinity not set. \nNone of the specified processes found. \
                             \n\nProcess names checked:\n{checked_process_names}")

if __name__ == "__main__":
    main()
