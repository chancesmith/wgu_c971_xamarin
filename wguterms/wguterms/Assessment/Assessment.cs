using System;
using SQLite;

namespace C971_WGU.Assessment
{
    [Table("Assessments")]
    internal class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string AssessmentName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string AssessType { get; set; }
        public int Course { get; set; }
        public int GetNotified { get; set; }
    }
}