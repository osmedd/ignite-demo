using Apache.Ignite.Core;
using Apache.Ignite.Core.Compute;
using Apache.Ignite.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfg.Common;

public class WorkerJob : IComputeFunc<string, int>
{
    [InstanceResource] private IIgnite? _ignite;

    public int Invoke(string arg)
    {
        Console.WriteLine("WorkerJob: got arg: '{0}'", arg);
        return -26;
    }

    // TODO Manually inject workerstatus and use it to update current topic
    public static IWorkerStatus? WorkerStatus { get; set; }
}
