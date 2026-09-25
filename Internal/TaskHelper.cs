using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DinaCSharp.Internal
{
    internal static class TaskHelper
    {
        internal static void FireAndForget(Task task)
        {
            ArgumentNullException.ThrowIfNull(task);
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                    Trace.WriteLine("Exception in fire-and-forget task: " + t.Exception);
            }, TaskScheduler.Default);
        }
    }
}
