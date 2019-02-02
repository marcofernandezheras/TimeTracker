namespace TimeTracker.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkItem")]
    public partial class WorkItem
    {
        public int Id { get; set; }

        public int IdTask { get; set; }

        [Column(TypeName = "date")]
        public DateTime WorkDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Column(TypeName = "ntext")]
        public string Comment { get; set; }

        public virtual Task Task { get; set; }
    }
}
