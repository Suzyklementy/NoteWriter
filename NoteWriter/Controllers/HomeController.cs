using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.IdentityModel.Tokens;
using NoteWriter.Data;
using NoteWriter.Models;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace NoteWriter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NoteWriterDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private static NoteModel currentNote;

        public HomeController(ILogger<HomeController> logger, NoteWriterDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

            if (!_context.Notes.Any(n => n.UserId == "0"))
            {
                _context.Add(
                    new NoteModel()
                    {
                        Title = "Note",
                        Text = null,
                        UserId = "0"
                    });
                _context.SaveChanges();
            }

            currentNote ??= _context.Notes.First(n => n.UserId == "0");
        }

        public IActionResult Index()
        {
            ViewBag.notes = _context.Notes.Where(n => n.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) || n.UserId == "0");
            return View(currentNote);
        }

        public IActionResult ChangeNote(int id)
        {
            currentNote = _context.Notes.First(n => n.Id == id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            if(_signInManager.IsSignedIn(User))
            {
                return View();
            }
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Title, Text, UserId")] NoteModel note)
        {
            note.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                _context.Add(note);
                await _context.SaveChangesAsync();

                currentNote = _context.Notes.First(n => n.Id == note.Id);
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var noteToRemove = _context.Notes.First(n => n.Id == id);
            _context.Notes.Remove(noteToRemove);
            await _context.SaveChangesAsync();

            currentNote = _context.Notes.First();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Update(string title, string text)
        {
            var updatedNote = _context.Notes.First(n => n.Id == currentNote.Id);
            updatedNote.Title = title;
            updatedNote.Text = text;
            _context.SaveChanges();

            currentNote = _context.Notes.First(n => n.Id == currentNote.Id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Email", "Password", "RememberMe")] LoginModel loginData)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginData.Email.Substring(0, loginData.Email.IndexOf("@")), loginData.Password, loginData.RememberMe, false);

                if(result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password is incorrect");
                    return View();
                }

            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            currentNote = null;
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("Email", "Password", "ConfirmPassword")] UserModel user)
        {
            if(ModelState.IsValid)
            {
                var newUser = new IdentityUser()
                {
                    UserName = user.Email.Substring(0, user.Email.IndexOf("@")),
                    Email = user.Email
                };

                var result = await _userManager.CreateAsync(newUser, user.Password);

                if(result.Succeeded) 
                {
                    await _signInManager.SignInAsync(newUser, false);
                    return RedirectToAction(nameof(Index));
                }

                foreach(var error in result.Errors)
                {
                    Debug.WriteLine(error.Description);
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}