# Nodatime: Calculate duration between 2 LocalTime without allocation

### The problem:
When we want to calculate the time between 2 LocalTime inputs, 
typically we need to do this operation:
```
((LocalTime)x - (LocalTime)y).ToDuration()...
```

The issue is not so obvious to me, until I recently profiled the app I work on:

> By simply doing the above calculation repeatedly over multiple time fields and for a large array of business objects,
the allocation that count on the library can easily go over :exclamation: 200MB :exclamation: in just 30s.

... and why is that?

It is just because the overloaded `-` operator will create a `Period` object that takes 80 bytes in size.

---
### Fix:
It turnout to be super simple... Just don't create the Period object!

However note that the time spent is not a main concern for me but the massive amount of allocation has added pressure onto our GC load.

A static method has now been added to `Period` class to compute the duration between
2 `LocalTime` structs straight ahead without the need to instantiate the class at all.

As of writing of this document, the fix has been merged but not yet released.

If you have a similar concern now, or in the future for some reason you cannot upgrade 
to the latest `nodatime` package,
I will recommend to just create your own method and do the same calculation as shown in this benchmark.

For example:
```csharp
// Get nanoseconds between:
(LocalTime)x.NanosecondOfDay - (LocalTime)y.NanosecondOfDay

// Get seconds between:
((LocalTime)x.NanosecondOfDay - (LocalTime)y.NanosecondOfDay) / 1_000_000_000
```
---
### Benchmark:
:white_check_mark: ~1600x faster 
:white_check_mark: 0 allocation

`13th Gen Intel Core i9-13900H | .NET SDK 9.0.100`

| Method             | Mean       | Error     | StdDev    | Median     | Gen0   | Allocated | Alloc Ratio |
|------------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|------------:|
| GetTimeBetween     | 15.1449 ns | 1.5887 ns | 4.6842 ns | 11.7431 ns | 0.0064 |      80 B |        1.00 |
| GetTimeBetweenFast |  0.0090 ns | 0.0122 ns | 0.0341 ns |  0.0000 ns |      - |         - |        0.00 |


---
### Links:
- [Link to Issue (#1835)](https://github.com/nodatime/nodatime/pull/1835)
- [Link to Fix (#1836)](https://github.com/nodatime/nodatime/pull/1836)
