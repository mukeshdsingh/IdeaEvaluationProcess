using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdeaEvaluationProcess.Data;
using IdeaEvaluationProcess.Data.Models;

namespace IdeaEvaluationProcess.Controllers
{
  public class IdeasController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly int _NoOfJudge = 3;

    public IdeasController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: Ideas
    public async Task<IActionResult> Index()
    {
      return View(await _context.Ideas.ToListAsync());
    }

    // GET: Ideas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var idea = await _context.Ideas
          .FirstOrDefaultAsync(m => m.Id == id);
      if (idea == null)
      {
        return NotFound();
      }

      return View(idea);
    }

    // GET: Ideas/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Ideas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description,Status")] Idea idea)
    {
      if (ModelState.IsValid)
      {
        _context.Add(idea);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(idea);
    }

    // GET: Ideas/CreateAndAssign
    public IActionResult CreateAndAssign()
    {
      return View();
    }

    // POST: Ideas/CreateAndAssign
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAndAssign([Bind("Id,Name,Description,Status")] Idea idea)
    {
      if (ModelState.IsValid)
      {
        _context.Add(idea);
        await _context.SaveChangesAsync();

        var evaluators = _context.Evaluators.Include(_=>_.IdeaEvaluators)
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
            IdeaId = idea.Id,
            EvaluatorId = tempEvaluator[evaluatorId]
          };
          _context.Add(assignJudge);

          if (tempEvaluator.Count > 0)
          {
            tempEvaluator.Remove(tempEvaluator[evaluatorId]);
          }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

      }
      return View(idea);
    }

    // GET: Ideas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var idea = await _context.Ideas.FindAsync(id);
      if (idea == null)
      {
        return NotFound();
      }
      return View(idea);
    }

    // POST: Ideas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Status")] Idea idea)
    {
      if (id != idea.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(idea);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!IdeaExists(idea.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      return View(idea);
    }

    // GET: Ideas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var idea = await _context.Ideas
          .FirstOrDefaultAsync(m => m.Id == id);
      if (idea == null)
      {
        return NotFound();
      }

      return View(idea);
    }

    // POST: Ideas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var idea = await _context.Ideas.FindAsync(id);
      _context.Ideas.Remove(idea);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool IdeaExists(int id)
    {
      return _context.Ideas.Any(e => e.Id == id);
    }
  }
}
