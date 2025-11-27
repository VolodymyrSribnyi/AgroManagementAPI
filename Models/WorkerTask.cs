using System;

namespace AgroindustryManagementAPI.Models;

public class WorkerTask
{
    public WorkerTask()
    {
    }
    public WorkerTask(string description)
    {
        Description = description;
    }

    
        public int Id { get; set; }
        public string Description { get; set; }

        public int WorkerId { get; set; }
        public Worker? Worker { get; set; }

        public int FieldId { get; set; }
        public Field? Field { get; set; }

        public TaskType TaskType { get; set; }
        public double Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? RealEndDate { get; set; }
        public DateTime EstimatesEndDate { get; set; }
    }




