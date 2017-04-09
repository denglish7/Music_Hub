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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BBApp.Controllers
{
    public class SongController : Controller
    {
        private BBContext _context;
        public SongController(BBContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("/index")]
        public IActionResult Index()
        {
             return View("Index");
        }
        [HttpGet]
        [Route("/success")]
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
        [Route("songPage/{id}")]
        public IActionResult songPage(int id)
        {
            if(HttpContext.Session.GetInt32("user_id") == null){
                return Redirect("/index");
            }
            ViewBag.loggedUser = HttpContext.Session.GetInt32("user_id");
            ViewBag.thisSong = _context.songs.SingleOrDefault(i => i.id == id);
            List<Join> joins = _context.Joins.Where(j => j.SongId == id).ToList();
            ViewBag.allUsers = joins;
            foreach(Join thisJoin in joins)
            {
                User thisUser = _context.users.SingleOrDefault(user => user.Id == thisJoin.UserId);
                thisJoin.User = thisUser;
            }
            foreach(Join thisJoin in joins)
            {
                if(thisJoin.UserId == ViewBag.loggedUser)
                {
                    ViewBag.thisSong.added = true;
                    break;
                }
                ViewBag.thisSong.added = false;
            }
            return View("songPage");
        }
        [HttpPost]
        [Route("addSong")]
        public IActionResult addSong(Song model)
        {
            if(ModelState.IsValid)
            {
                model.created_at = DateTime.Now;
                model.updated_at = DateTime.Now;
                model.user_id = (int)HttpContext.Session.GetInt32("user_id");
                model.added = false;
                _context.songs.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Success");
            }
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
            
            return View("Success", model);
        }
        [HttpGet]
        [Route("userPage/{id}")]
        public IActionResult userPage(int id)
        {
            ViewBag.userInfo = _context.users.SingleOrDefault(u => u.Id == id);
            ViewBag.loggedUser = _context.users.SingleOrDefault(u => u.Id == (int)HttpContext.Session.GetInt32("user_id"));
            ViewBag.playlist = _context.Joins.Where(j => j.UserId == id).ToList();
            foreach(Join myJoin in ViewBag.playlist)
            {
                Song thisSong = _context.songs.SingleOrDefault(s => s.id == myJoin.SongId);
                myJoin.song = thisSong;
            }


            return View("userPage");
        } 
        [HttpGet]
        [Route("addToPlaylist/{id}")]
        public IActionResult addToPlaylist(int id)
        {
            Song mySong = _context.songs.SingleOrDefault(song => song.id == id); 
            Join myJoin = new Join();
            myJoin.SongId = id;
            myJoin.UserId = (int)HttpContext.Session.GetInt32("user_id");
            _context.Joins.Add(myJoin);
            _context.SaveChanges();
            return RedirectToAction("Success");
        }
        [HttpGet]
        [Route("addToPlaylistFromSong/{id}")]
        public IActionResult addToPlaylistFromSong(int id)
        {
            Song mySong = _context.songs.SingleOrDefault(song => song.id == id); 
            Join myJoin = new Join();
            myJoin.SongId = id;
            myJoin.UserId = (int)HttpContext.Session.GetInt32("user_id");
            _context.Joins.Add(myJoin);
            _context.SaveChanges();
            return Redirect("/songPage/" + mySong.id);
        }
        [HttpGet]
        [Route("removeFromPlaylist/{id}")]
        public IActionResult removeFromPlaylist(int id)
        {
            User myUser = _context.users.SingleOrDefault(user => user.Id == (int)HttpContext.Session.GetInt32("user_id"));
            Join myJoin = _context.Joins.SingleOrDefault(join => join.JoinId == id); 
            _context.Joins.Remove(myJoin);
            _context.SaveChanges();
            return Redirect("/userPage/"+ myUser.Id);
        }
    }
}