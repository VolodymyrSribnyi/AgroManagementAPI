using System;
using System.Collections.Generic;
using System.Xml;

namespace AgroindustryManagementAPI.Models;

public class Worker
{
    public Worker(string firstName, string lastName, List<WorkerTask> tasks)
    {
        FirstName = firstName;
        LastName = lastName;
        Tasks = tasks;
    }
    public Worker()
    {
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; }
    public List<WorkerTask> Tasks { get; set; } = [];
    public decimal HoursWorked { get; set; }
    
    public List<WorkerTask> GetActiveTasks()
    {
        throw new NotImplementedException();
    }
}