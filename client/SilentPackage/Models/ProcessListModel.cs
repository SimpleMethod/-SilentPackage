using System;
using System.Collections.Generic;
using System.Text;

namespace SilentPackage.Models
{
    public class ProcessList
    {
        public List<Process> ProcessObjectList { get; set; }
        public long Timestamp { get; set; }
    }

    public class Process
    {
        public Process(string name, int id, string startTime)
        {
            Name = name;
            Id = id;
            StartTime = startTime;
        }

        public string Name { get; set; }
        public int Id { get; set; }
        public string StartTime { get; set; }
        
    }
}
