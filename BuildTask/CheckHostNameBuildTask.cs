using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace BuildTask
{
public class CheckHostNameBuildTask : Task
{
    /// <summary>
    /// Comma-separated list of hostnames to check. 
    /// </summary>
    [Required]
    public ITaskItem HostNamesToCheck { get; set; }

    public override bool Execute()
    {
        var missingHostNames = new List<string>();

        foreach (string hostname in GetHostNames())
        {
            try
            {
                Dns.GetHostAddresses(hostname);
            }
            catch (Exception)
            {
                missingHostNames.Add(hostname);
            }
        }

        if (missingHostNames.Count > 0)
        {
            string errorMsg = string.Format("Failed to find host name: [{0}]",
                                            string.Join(",", missingHostNames));

            BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
                    "CheckHostNameBuildTask", string.Empty, "CheckHostNameBuildTask.dll", 
                    0, 0, 0, 0, errorMsg, string.Empty, string.Empty));

            return false;
        }

        return true;
    }

    private IEnumerable<string> GetHostNames()
    {
        return HostNamesToCheck.ItemSpec.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
    }
}
}