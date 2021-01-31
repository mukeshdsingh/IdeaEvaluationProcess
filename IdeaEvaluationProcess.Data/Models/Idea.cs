using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdeaEvaluationProcess.Data.Models
{
  public class Idea : IEntity
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public IList<IdeaEvaluator> IdeaEvaluators { get; set; }
  }
}
