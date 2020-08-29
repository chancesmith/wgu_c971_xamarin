﻿using System;
using SQLite;

namespace wguterms.Course
{
    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string CourseName { get; set; }
        public string CourseStatus { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string InstructorName { get; set; }
        public string InstructorEmail { get; set; }
        public string InstructorPhone { get; set; }
        public int Term { get; set; }
        public string Notes { get; set; }
        public int GetNotified { get; set; }
    }
}