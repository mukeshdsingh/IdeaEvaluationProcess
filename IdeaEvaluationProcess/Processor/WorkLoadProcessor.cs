using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdeaEvaluationProcess.Processor
{
  public class WorkLoadProcessor
  {
    public string AssignEvaluators()
    {
      var random = new Random();

      var list = new List<string> { "1", "5", "7", "8" };
      int index = random.Next(list.Count);

      return list[index];
    }
  }
}
