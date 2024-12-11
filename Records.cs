using System;

namespace sokoban;

public class Records
{
    public string Name { get; }
    public int Steps { get; set; }
    public TimeSpan Time { get; set; }

    public Records(string map) : this(map, 9999, new TimeSpan(0, 59, 59))
    { }

    public Records(string map, int steps, TimeSpan time)
    {
        Name = map;
        Steps = steps;
        Time = time;
    }
}
