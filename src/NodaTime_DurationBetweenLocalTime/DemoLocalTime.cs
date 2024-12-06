using BenchmarkDotNet.Attributes;
using NodaTime;

namespace NodaTimeLocalTime;

[MemoryDiagnoser]
public class DemoLocalTime
{
    public LocalTime StartTime { get;  } = LocalTime.FromTimeOnly(TimeOnly.FromDateTime(DateTime.Now));
    public LocalTime EndTime => LocalTime.Midnight;

    [Benchmark(Baseline = true)]
    public double GetTimeBetween()
    {
        return (EndTime - StartTime).ToDuration().TotalNanoseconds;
    }

    [Benchmark]
    public double GetTimeBetweenFast()
    {
        unchecked
        {
            return EndTime.NanosecondOfDay - StartTime.NanosecondOfDay;
        }
    }
}