using System;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;

while (true)
{
  Console.ForegroundColor = ConsoleColor.Red;
  Console.WriteLine("************** Welcome to the DOE Joiner MOE Configuration Tool *************");
  Console.WriteLine("**************This tool is only permitted to be used by the DOE team.***********");
  Console.WriteLine();
  Console.WriteLine();
  System.Threading.Thread.Sleep(2000);
  Console.Clear();
  Console.WriteLine("Loading.... Please Wait....");
  System.Threading.Thread.Sleep(2500);
  Console.ResetColor();
  Console.Clear();
  Console.WriteLine("1. Check system configuration");
  Console.WriteLine("2. Start MOE Joiner procedure");
  Console.WriteLine("3. Update Group Policy");
  Console.WriteLine("4. Reboot system");
  Console.WriteLine("5. Exit");
  Console.Write("Enter the choice: ");
  string choice = Console.ReadLine();

  switch (choice)
  {
    case "1":
      CheckSystemConfiguration();
      break;
    case "2":
      StartMOEJoinerProcedure();
      break;
    case "3":
      UpdateGroupPolicy();
      break;
    case "4":
      RebootSystem();
      break;
    case "5":
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Exiting...");
      System.Threading.Thread.Sleep(2000);
      Console.ResetColor();
      return; // Exit the program
    default:
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid choice. Please try again.");
      System.Threading.Thread.Sleep(1000);
      Console.ResetColor();
      break;
  }
}

void CheckSystemConfiguration()
{
  Console.Clear();
  Console.WriteLine("Checking system configuration...\n");

  Console.WriteLine($"CPU: {GetCPUName()}");
  Console.WriteLine($"Installed RAM: {GetInstalledRAM()}");
  Console.WriteLine();

  PrintNetworkInfo();

  Console.WriteLine("\nSystem configuration check completed.");
  Console.WriteLine("Press any key to continue...");
  Console.ReadKey(true);
  Console.Clear();
}

string GetCPUName()
{
  try
  {
    var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
    foreach (ManagementObject obj in searcher.Get())
    {
      return obj["Name"]?.ToString() ?? "Unknown CPU";
    }
  }
  catch { }
  return "Unknown CPU";
}

string GetInstalledRAM()
{
  try
  {
    var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
    ulong totalBytes = 0;
    foreach (ManagementObject obj in searcher.Get())
    {
      totalBytes += (ulong)obj["Capacity"];
    }
    return $"{totalBytes / (1024 * 1024 * 1024)} GB";
  }
  catch { }
  return "Unknown RAM Size";
}

void PrintNetworkInfo()
{
    try
    {
        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus == OperationalStatus.Up &&
                ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                var ipProps = ni.GetIPProperties();

                // IP Address (IPv4)
                foreach (var addr in ipProps.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        Console.WriteLine($"IP Address: {addr.Address}");
                        break; // Only show first one
                    }
                }

                // DNS Servers
                foreach (var dns in ipProps.DnsAddresses)
                {
                    Console.WriteLine($"DNS: {dns}");
                }

                break; // Only show for the first active interface
            }
        }
    }
    catch
    {
        Console.WriteLine("Unable to retrieve network information.");
    }
}

void StartMOEJoinerProcedure()
{
  Console.Clear();
  Console.ForegroundColor = ConsoleColor.Green;
  Console.WriteLine("Triggerig system configuration with DOE....");
var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "powershell.exe",
        Arguments = "-ExecutionPolicy Bypass -File \"Join.ps1\"",
        UseShellExecute = false,
        RedirectStandardOutput = false,
        RedirectStandardError = false,
        CreateNoWindow = false,
        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory // Ensure it runs from your app's folder
    }
};

process.Start();
process.WaitForExit();
  Console.WriteLine("System is joined to domain and configured with company");
  System.Threading.Thread.Sleep(1000);
  Console.ResetColor();
  Console.Clear();
}
void UpdateGroupPolicy()
{
  Console.Clear();
  Console.ForegroundColor = ConsoleColor.Green;
  Console.WriteLine("Triggering Group Policy Update...");
  var process = new Process
  {
    StartInfo = new ProcessStartInfo
    {
      FileName = "gpupdate",
      Arguments = "/force",
      UseShellExecute = false,
      CreateNoWindow = false
    }
  };
  process.Start();
  process.WaitForExit();
  Console.WriteLine("Group Policy update completed.");
  System.Threading.Thread.Sleep(1000);
  Console.ResetColor();
  Console.Clear();
}
void RebootSystem()
{
  Console.WriteLine("Rebooting system now...");
  System.Threading.Thread.Sleep(1000);
  Process.Start(new ProcessStartInfo
  {
    FileName = "shutdown",
    Arguments = "/r /t 0", // /r = restart, /t 0 = no delay
    CreateNoWindow = false,
    UseShellExecute = false
  });
  Console.Clear();
}
