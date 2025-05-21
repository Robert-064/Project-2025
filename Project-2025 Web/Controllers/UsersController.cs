using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;

namespace Project_2025_Web.Controllers
{
	public class UsersController : Controller
	{
		private readonly DataContext _context;

		public UsersController(DataContext context)
		{
			_context = context;
		}

		// GET: /Users
		public IActionResult Index()
		{
			// Incluye el rol para mostrar su nombre
			var users = _context.Users
				.Include(u => u.Role)
				.ToList();
			return View(users);
		}

		// GET: /Users/Create
		public IActionResult Create()
		{
			// Lista de roles para el dropdown
			ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name");
			return View();
		}

		// POST: /Users/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(User user)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
				return View(user);
			}

			// TODO: aquí debes hashear la contraseña antes de guardarla
			_context.Users.Add(user);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		// GET: /Users/Edit/5
		public IActionResult Edit(int id)
		{
			var user = _context.Users.Find(id);
			if (user == null) return NotFound();

			ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
			return View(user);
		}

		// POST: /Users/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(User user)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
				return View(user);
			}

			_context.Users.Update(user);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		// GET: /Users/Delete/5
		public IActionResult Delete(int id)
		{
			var user = _context.Users
				.Include(u => u.Role)
				.FirstOrDefault(u => u.Id == id);
			if (user == null) return NotFound();
			return View(user);
		}

		// POST: /Users/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(int id)
		{
			var user = _context.Users.Find(id);
			if (user != null)
			{
				_context.Users.Remove(user);
				_context.SaveChanges();
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
