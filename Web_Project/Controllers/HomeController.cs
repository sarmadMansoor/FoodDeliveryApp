using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Web_Project.Models;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Web_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment Environment;


        // Constructor
        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _userManager = userManager;
            Environment = webHostEnvironment;
        }

        // Index Action
        public IActionResult Index()
        {
            return View();
        }

        // Profile Action
            public IActionResult Profile()
            {
                if (!User.Identity.IsAuthenticated)
                {
                    // Redirect to login page if the user is not authenticated
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                // Get the currently logged-in user synchronously
                var currentUser = _userManager.GetUserAsync(User).Result;

                // Handle case where the user might not exist in the database
                if (currentUser == null)
                {
                    // Log an error and redirect to login
                    _logger.LogWarning("Authenticated user not found in the database.");
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                // Prepare user information
                var userInfo = new UserProfileViewModel
                {
                    FullName = currentUser.FullName,
                    Email = currentUser.Email,
                    MobileNumber = currentUser.MobileNumber,
                    Address = currentUser.Address,
                    ZipCode = currentUser.ZipCode,
                    ProfilePicturePath = currentUser.ProfilePicturePath
                };

                // Pass the user information to the view
                return View(userInfo);
            }

        public IActionResult EditProfile()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EditProfile(UserProfileViewModel model, IFormFile ProfilePicturePath)
        {
            // Get the currently logged-in user
            var currentUser = _userManager.GetUserAsync(User).Result; // Synchronous execution
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Update user information
            currentUser.FullName = model.FullName;
            currentUser.Email = model.Email;
            currentUser.MobileNumber = model.MobileNumber;
            currentUser.Address = model.Address;
            currentUser.ZipCode = model.ZipCode;

            // Handle profile picture upload if provided
            if (ProfilePicturePath != null && ProfilePicturePath.Length > 0)
            {
                string wwwRootPath = Environment.WebRootPath; // Ensure `Environment` is correctly injected
                string uploadFolderPath = Path.Combine(wwwRootPath, "Uploads");

                // Create the directory if it does not exist
                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                // Create a unique file name to prevent conflicts
                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfilePicturePath.FileName)}";
                string filePath = Path.Combine(uploadFolderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePicturePath.CopyTo(stream); // Save the file synchronously
                }

                // Update the profile picture path in the user record
                currentUser.ProfilePicturePath = $"/Uploads/{uniqueFileName}";

            }
            // Update the user in the database
            var result = _userManager.UpdateAsync(currentUser).Result; // Persist changes to the database
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile", "Home");
            }

            // Add errors to the ModelState if the update fails
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // Return the view with the current data if an error occurs
            return View("EditProfile", model);
        }


        // Privacy Action
        public IActionResult Privacy()
        {
            return View();
        }

        // About Action
        public IActionResult About()
        {
            return View();
        }

        // BookTable Action
        public IActionResult BookTable()
        {
            return View();
        }

        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Web_Project;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        [HttpPost]
        public IActionResult BookTable(BookingModel booking)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                INSERT INTO bookings (Name, Phone, Email, Persons, Date)
                VALUES (@Name, @Phone, @Email, @Persons, @Date)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", booking.Name);
                        command.Parameters.AddWithValue("@Phone", booking.Phone);
                        command.Parameters.AddWithValue("@Email", booking.Email);
                        command.Parameters.AddWithValue("@Persons", booking.Persons);
                        command.Parameters.AddWithValue("@Date", booking.Date);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                ViewBag.Message = "Booking successful!";
                return View();
            }

            // If model state is invalid, reload the form with errors
            return View(booking);
        }

        // Error Action
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // ViewModel for the Profile View
    public class UserProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
