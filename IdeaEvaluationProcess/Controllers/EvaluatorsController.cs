using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdeaEvaluationProcess.Data;
using IdeaEvaluationProcess.Data.Models;
using IdeaEvaluationProcess.Models;

namespace IdeaEvaluationProcess.Controllers
{
  public class EvaluatorsController : Controller
  {
    private readonly ApplicationDbContext _context;

    public EvaluatorsController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: Evaluators
    public async Task<IActionResult> Index()
    {
      return View(await _context.Evaluators.ToListAsync());
    }

    public new async Task<ActionResult<IEnumerable<EvaluatorCountViewModel>>> GetCountAsync()
    {
      var result = new List<EvaluatorCountViewModel>();
      var evaluators = await _context.Evaluators.Include(_ => _.IdeaEvaluators).ToListAsync();
      foreach (var item in evaluators)
      {
        result.Add(new EvaluatorCountViewModel()
        {
          Id = item.Id,
          Name = item.Name,
          Count = item.IdeaEvaluators.Count
        });
      }

      return View(result);
    }

    // GET: Evaluators/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var evaluator = await _context.Evaluators
          .FirstOrDefaultAsync(m => m.Id == id);
      if (evaluator == null)
      {
        return NotFound();
      }

      return View(evaluator);
    }

    // GET: Evaluators/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Evaluators/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] Evaluator evaluator)
    {
      if (ModelState.IsValid)
      {
        _context.Add(evaluator);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(evaluator);
    }

    // GET: Evaluators/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var evaluator = await _context.Evaluators.FindAsync(id);
      if (evaluator == null)
      {
        return NotFound();
      }
      return View(evaluator);
    }

    // POST: Evaluators/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Evaluator evaluator)
    {
      if (id != evaluator.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(evaluator);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!EvaluatorExists(evaluator.Id))
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
      return View(evaluator);
    }

    // GET: Evaluators/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var evaluator = await _context.Evaluators
          .FirstOrDefaultAsync(m => m.Id == id);
      if (evaluator == null)
      {
        return NotFound();
      }

      return View(evaluator);
    }

    // POST: Evaluators/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var evaluator = await _context.Evaluators.FindAsync(id);
      _context.Evaluators.Remove(evaluator);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool EvaluatorExists(int id)
    {
      return _context.Evaluators.Any(e => e.Id == id);
    }
  }
}
