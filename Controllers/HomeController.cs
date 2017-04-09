using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BBApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace BBApp.Controllers
{
    public class HomeController : Controller
    {
        private BBContext _context;
        public HomeController(BBContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("new")]
        public IActionResult Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(_context.users.SingleOrDefault(user => user.email == model.email)  != null)
                {
                    TempData["usernameExists"] = "Username taken";
                    return RedirectToAction("Success");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    User NewUser = new User
                    {
                        first_name = model.first_name,
                        last_name = model.last_name,
                        email = model.email,
                        password = model.password,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    NewUser.password = Hasher.HashPassword(NewUser, NewUser.password);
                    _context.users.Add(NewUser);
                    _context.SaveChanges();
                }
                TempData["regSuccess"] = "You have been successfully registered.";
                return RedirectToAction("Success");
            } 
            return View("Index", model);
        }
    

        [HttpPost]
        [Route("login")]
        public IActionResult LoginMethod(string loginEmail, string PasswordToCheck)
        {
            List<string> logErrors = new List<string>();

            if(ModelState.IsValid)
            {
                // User user = userRepository.FindByUsername(loginUsername);
                User user = _context.users.SingleOrDefault(loguser => loguser.email == loginEmail);
                if(user != null && user.password != null)
                {
                    if(PasswordToCheck == null)
                    {
                        TempData["invalid"] = "Please enter a password";
                        return RedirectToAction("Success");
                    }
                    var Hasher = new PasswordHasher<User>();
                    if(0 != Hasher.VerifyHashedPassword(user, user.password, PasswordToCheck))
                    {
                        HttpContext.Session.SetInt32("user_id", user.Id);
                        return RedirectToAction("Success");
                    }
                    else
                    {
                        TempData["invalid"] = "Incorrect Password";
                        return RedirectToAction("Success");
                    }
                }
                else if(loginEmail == null)
                {
                    TempData["invalid"] = "Please enter an email";
                }
                else if(user == null)
                {
                    TempData["invalid"] = "Email not found";
                }
            }
            return RedirectToAction("Success");
        }
        [HttpGet]
        [Route("")]
        public IActionResult Success()
        {
            if(HttpContext.Session.GetInt32("user_id") == null)
            {
                ViewBag.invalid = TempData["invalid"];
                ViewBag.usernameExists = TempData["usernameExists"];
                ViewBag.errors = TempData["errors"];
                ViewBag.regSuccess = TempData["regSuccess"];
                ViewBag.logErrors = TempData["logErrors"];
                return View("Index");
            }
            else
            {
                ViewBag.userId = HttpContext.Session.GetInt32("user_id");
                ViewBag.user = _context.users.SingleOrDefault(user => user.Id == (int)HttpContext.Session.GetInt32("user_id"));
                ViewBag.allSongs = _context.songs.Include(song => song.Joins).ThenInclude(k => k.User).ToList().OrderByDescending(song => song.Joins.Count);
                foreach(Song thisSong in ViewBag.allSongs)
                {
                    foreach(Join thisJoin in thisSong.Joins)
                    {
                        if(thisJoin.UserId == ViewBag.userId)
                        {
                            thisSong.added = true;
                            break;
                        }
                        thisSong.added = false;
                    }
                }
                return View("Success");
            }
        }     
        [HttpGet]
        [Route("logoff")]
        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Success");
        }
    }
}
