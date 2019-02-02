namespace TimeTracker.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TimeTrackerModel : DbContext
    {
        public TimeTrackerModel()
            : base("name=TimeTrackerModel")
        {
        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<WorkItem> WorkItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.Project)
                .HasForeignKey(e => e.IdProject);

            modelBuilder.Entity<Task>()
                .HasMany(e => e.Task1)
                .WithOptional(e => e.Task2)
                .HasForeignKey(e => e.IdParent);

            modelBuilder.Entity<Task>()
                .HasMany(e => e.WorkItems)
                .WithRequired(e => e.Task)
                .HasForeignKey(e => e.IdTask)
                .WillCascadeOnDelete(false);
        }
    }
}
