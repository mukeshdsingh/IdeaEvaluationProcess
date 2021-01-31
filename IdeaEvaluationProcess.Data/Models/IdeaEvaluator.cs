using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdeaEvaluationProcess.Data.Models
{
  public class IdeaEvaluator : IEntity
  {
    [Key]
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public Idea Idea { get; set; }

    public int EvaluatorId { get; set; }
    public Evaluator Evaluator { get; set; }
  }
}
