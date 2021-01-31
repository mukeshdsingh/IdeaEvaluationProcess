using IdeaEvaluationProcess.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdeaEvaluationProcess.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
      Database.EnsureCreated();
    }

    public DbSet<Idea> Ideas { get; set; }
    public DbSet<Evaluator> Evaluators { get; set; }
    public DbSet<IdeaEvaluator> IdeaEvaluators { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      //modelBuilder.Entity<IdeaEvaluator>().HasKey(_ => new { _.IdeaId, _.EvaluatorId });

      //modelBuilder.Entity<IdeaEvaluator>()
      //    .HasOne<Idea>(sc => sc.Idea)
      //    .WithMany(s => s.IdeaEvaluators)
      //    .HasForeignKey(sc => sc.Id);

      base.OnModelCreating(modelBuilder);
    }
  }
}
