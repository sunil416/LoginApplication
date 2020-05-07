using LoginApplication.Models;
using System.Linq;
using System.Web.Mvc;

namespace LoginApplication.Controllers
{
    public class UserController : Controller
    {
        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            bool Status = false;
            string message = "In-valid Username and Password";
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                using (LoginApplicationDatabaseEntities1 dc = new LoginApplicationDatabaseEntities1())
                {
                    string password = Crypto.Hash(user.Password);
                    var v = dc.Users.Where(a => a.EmailId == user.EmailId && a.Password.Equals(password)).FirstOrDefault();
                    if (v != null)
                    {
                        Status = true;
                        message = "Login Successfull";
                    }
                }
            }
            else
            {
                message = "Enter Password";
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(User user)
        {
            bool Status = false;
            string message = "";
            if (ModelState.IsValid)
            {

                var isExist = IsEmailExist(user.EmailId);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }

                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);

                using (LoginApplicationDatabaseEntities1 dc = new LoginApplicationDatabaseEntities1())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();

                    message = "User Created ";
                    Status = true;
                }
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (LoginApplicationDatabaseEntities1 dc = new LoginApplicationDatabaseEntities1())
            {
                var v = dc.Users.Where(a => a.EmailId == emailID).FirstOrDefault();
                return v != null;
            }
        }
    }
}