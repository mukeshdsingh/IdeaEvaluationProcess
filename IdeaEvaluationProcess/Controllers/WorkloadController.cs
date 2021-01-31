using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeaEvaluationProcess.Data;
using IdeaEvaluationProcess.Data.Models;
using IdeaEvaluationProcess.Helper;
using IdeaEvaluationProcess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeaEvaluationProcess.Controllers
{
  public class WorkloadController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly int _NoOfJudge = 3;

    public WorkloadController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: WorkloadController
    public IActionResult Index()
    {
      var tableRecordsCount = new TableRecordsCountViewModel
      {
        IdeaCount = _context.Ideas.Count(),
        EvaluatorCount = _context.Evaluators.Count(),
        IdeaEvaluatorCount = _context.IdeaEvaluators.Count()
      };

      return View(tableRecordsCount);
    }

    // GET: WorkloadController/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: WorkloadController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(WorkloadViewModel model)
    {
      if (model.NoOfIdeas <= 0 && model.NoOfEvaluators <= 0)
      {
        return BadRequest();
      }

      var ideas = new List<Idea>();
      for (int i = 0; i < model.NoOfIdeas; i++)
      {
        ideas.Add(new Idea()
        {
          Name = "Idea - " + i,
          Description = "Idea Description - " + i
        });
      }

      var evaluators = new List<Evaluator>();
      for (int i = 0; i < model.NoOfEvaluators; i++)
      {
        evaluators.Add(new Evaluator()
        {
          Name = "Evaluator - " + i
        });
      }

      _context.Ideas.AddRange(ideas);
      _context.Evaluators.AddRange(evaluators);
      _context.SaveChanges();

      return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Process()
    {
      var ideasToBeAssign = _context.Ideas
                          .ToList()
                          .LeftExcludingJoin(_context.IdeaEvaluators,idea => idea.Id,ideaEvaluator => ideaEvaluator.IdeaId,(idea, ideaEvaluator) => new { PendingIdea = idea }).ToList();

      foreach (var idea in ideasToBeAssign)
      {
        var evaluators = _context.Evaluators.Include(_ => _.IdeaEvaluators)
                  .Select(g => new { GroupId = g.Id, Count = g.IdeaEvaluators.Count() })
                  .OrderBy(_ => _.Count)
                  .Select(_ => new { Id = _.GroupId })
                  .Take(_NoOfJudge)
                  .ToList();

        List<int> tempEvaluator = new List<int>();
        foreach (var evaluator in evaluators)
        {
          tempEvaluator.Add(evaluator.Id);
        }

        var random = new Random();

        for (int i = 0; i < _NoOfJudge; i++)
        {
          int evaluatorId = random.Next(tempEvaluator.Count);
          var assignJudge = new IdeaEvaluator()
          {
            IdeaId = idea.PendingIdea.Id,
            EvaluatorId = tempEvaluator[evaluatorId]
          };
          _context.Add(assignJudge);

          if (tempEvaluator.Count > 0)
          {
            tempEvaluator.Remove(tempEvaluator[evaluatorId]);
          }
        }

        await _context.SaveChangesAsync();
      }
      
      return RedirectToAction(nameof(Index), "Ideas");
    }
  }
}
