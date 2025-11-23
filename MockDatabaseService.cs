using Prog_Poe.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMCS.Services
{
    public class MockDatabaseService
    {
        public List<User> Users { get; private set; }
        public List<Claim> Claims { get; private set; }

        public MockDatabaseService()
        {
            // Seed Users
            Users = new List<User>
            {
                new User { Id = "LEC-78901", Email = "lecturer@cmcs.edu", Name = "John Doe", Role = UserRole.Lecturer, Password = "password123" },
                new User { Id = "COORD-22345", Email = "coordinator@cmcs.edu", Name = "Sarah Admin", Role = UserRole.Coordinator, Password = "password123" },
                new User { Id = "MGR-11234", Email = "manager@cmcs.edu", Name = "Mike Manager", Role = UserRole.Manager, Password = "password123" }
            };

            // Seed Claims
            Claims = new List<Claim>
            {
                new Claim
                {
                    Id = "CLM-2023-001",
                    LecturerId = "LEC-78901",
                    LecturerName = "John Doe",
                    SubmissionDate = DateTime.Parse("2023-10-05"),
                    HoursWorked = 20,
                    HourlyRate = 50,
                    Description = "Software Engineering Lectures",
                    Status = ClaimStatus.Approved,
                    FileName = "timesheet.pdf"
                },
                new Claim
                {
                    Id = "CLM-2023-002",
                    LecturerId = "LEC-78901",
                    LecturerName = "John Doe",
                    SubmissionDate = DateTime.Parse("2023-10-12"),
                    HoursWorked = 10,
                    HourlyRate = 50,
                    Description = "Marking papers",
                    Status = ClaimStatus.Pending,
                    FileName = null
                }
            };
        }
    }
}