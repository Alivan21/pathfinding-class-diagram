using System;
using System.Diagnostics;

namespace PathFindingClassDiagram.Helpers
{
    public class PerformanceTracker: IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly long _memoryBefore;
        public PerformanceTracker()
        {
            _memoryBefore = GC.GetTotalMemory(true);
            _stopwatch = Stopwatch.StartNew();
        }
        public TimeSpan ElapsedTime => _stopwatch.Elapsed;
        public long MemoryUsed => GC.GetTotalMemory(true) - _memoryBefore;
        public string ElapsedTimeString => $"{_stopwatch.Elapsed.TotalSeconds:F2} seconds";
        public string MemoryUsedString => $"{MemoryUsed} bytes";
        public void Dispose()
        {
            _stopwatch?.Stop();
        }
    }
}
