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
                ViewBag.allSongs = _context.songs.Include(song => song.Joins).ThenInclude(j => j.User).ToList();
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

        [HttpGet]
        [Route("songPage/{id}")]
        public IActionResult songPage(int id)
        {
            ViewBag.songInfo = _context.songs.SingleOrDefault(i => i.id == id);
            ViewBag.allUsers = _context.users.Include(p => p.Joins).ThenInclude(s => s.song).ToList();
            return View("songPage");
        }
        [HttpPost]
        [Route("addSong")]
        public IActionResult addSong(Song model)
        {
            User user = _context.users.SingleOrDefault(User => User.Id == (int)HttpContext.Session.GetInt32("user_id"));
            if(ModelState.IsValid)
            {
                model.created_at = DateTime.Now;
                model.updated_at = DateTime.Now;
                model.userid = (int)HttpContext.Session.GetInt32("user_id");
                model.times_added = 1;
                _context.songs.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Success");
            }
            return View("Success", model);
        }
        [HttpGet]
        [Route("userPage/{id}")]
        public IActionResult userPage(int id)
        {
            ViewBag.userInfo = _context.users.SingleOrDefault(u => u.Id == id);
            ViewBag.allUserSongs = _context.songs.Include(p => p.Joins).ThenInclude(s => s.User).ToList();
            return View("userPage");
        } 


        [HttpGet]
        [Route("addToPlaylist/{id}")]
        public IActionResult addToPlaylist(int id)
        {
            Song mySong = _context.songs.SingleOrDefault(song => song.id == id); 
            mySong.times_added +=1;
            Join myJoin = new Join();
            myJoin.SongId = id;
            myJoin.UserId = (int)HttpContext.Session.GetInt32("user_id");
            _context.Joins.Add(myJoin);
            _context.SaveChanges();
           
            return RedirectToAction("Success");
        }
    }
}
