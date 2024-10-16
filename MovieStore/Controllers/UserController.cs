using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieStore.Models;
using MovieStore.Repository;
using System.Security.Claims;
namespace MovieStore.Controllers
{
    public class UserController : Controller
    {
        private readonly IUser _userRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMovie _movieRepository;
        private readonly ILogger<UserController> _log;
        public UserController(IUser userRepository,IWebHostEnvironment env,IMovie repo,ILogger<UserController> log)
        {
            _userRepository = userRepository;
            _env = env;
            _movieRepository = repo;
            _log = log;
        }
        public IActionResult Index()
        {
            // Check if the user is logged in by checking if the session exists
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                // If the session is empty or null, redirect to the Login page
                return RedirectToAction("Login", "User");
            }

            // If logged in, retrieve session data and pass it to the view
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;
            var movies = _movieRepository.GetAllMovies();
            return View(movies);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Create a Register.cshtml view for user registration
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle the file upload
                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images"); // Adjust the path as needed
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    model.ProfileImage.CopyTo(new FileStream(filePath,FileMode.Create));
                   

                    // Save user info, including the path to the image
                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Password = model.Password, // Note: Use hashing in a real application
                        Role = model.Role,
                        ProfileImagePath =  uniqueFileName // Save the image path
                    };

                    // Call the repository to save the user (implement this in your repository)
                    await _userRepository.RegisterAsync(user); // Assuming you have a method for user registration
                    return RedirectToAction("Login","User"); // Redirect to Login after successful registration
                }
                else
                {
                    ModelState.AddModelError("ProfileImage", "Profile image upload failed.");
                }
            }
            return View(model); // Return to the view with validation messages if the model is not valid
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Create a Login.cshtml view for user login
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userRepository.LoginAsync(username, password);
            if (user != null)
            {
                // Here, you can set up authentication (for example, using cookies)
                // For demonstration, we are simply redirecting to the home page
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("UserRole", user.Role);
                if (user.Role == "Admin")
                {
                    // Redirect to Admin controller
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index");
            }

            // If login fails, show an error message
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View(); // Return the login view if login fails
        }
        public IActionResult ConfirmPurchase(int movieId)
        {
            // Fetch the movie details by ID to display on the confirmation page
            var movie = _movieRepository.GetMovieById(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie); // Pass the movie to the view
        }
        [HttpPost]
        public IActionResult PurchaseConfirmed(int movieId)
        {
            // Fetch the movie details by ID to proceed with the purchase
            _log.LogInformation("HERE IT IS _______________________>>>>");
            var movie = _movieRepository.GetMovieById(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            // Logic to handle the purchase (e.g., adding the movie to the user's purchase history)
            var userName = HttpContext.Session.GetString("UserName");
            User user = _userRepository.GetUserByUsername(userName); // Assuming a method that retrieves the user by username
            if (User == null)
            {
                return RedirectToAction("Login", "User");
            }

            var hasPurchased = _userRepository.HasUserPurchasedMovie(user.UserId, movieId);
            if (hasPurchased)
            {
                // You can return a view or message indicating that the movie has already been purchased
                TempData["ErrorMessage"] = "You have already purchased this movie.";
                return RedirectToAction("MovieDetails", new {movieId=movieId});
            }
            // Create a new order object for the user
            var order = new Order
            {
                UserId = (user.UserId),
                OrderDate = DateTime.Now,
                TotalAmount = movie.Price, // Assuming a single movie purchase
                MovieId = movieId, // Add MovieId to your Order model if not already there
                Price = movie.Price,
                Rating = 0
            };
          
            // Add the order to the user's purchase history (repository method)
            _userRepository.AddOrder(order);

            // Save changes to the database

            // Redirect to a purchase success page or back to the movie 
            // Redirect to a purchase success page or back to the movie list
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Orders()
        {
            // Get the current logged-in user name from session or any other source (e.g., HttpContext)
            var userName = HttpContext.Session.GetString("UserName");

            // Retrieve the user by userName
            var user = _userRepository.GetUserByUsername(userName);
            if (user == null)
            {
                // Redirect to login if the user is not found or not logged in
                return RedirectToAction("Login", "User");
            }

            // Retrieve the orders for the user
            var orders = _userRepository.GetOrdersByUserId(user.UserId).ToList();

            // Pass the orders to the view
            return View(orders);
        }
        private int GetCurrentUserId()
        {
            // Assuming you're using session or identity to get the logged-in user ID
            var userName = HttpContext.Session.GetString("UserName");
            var user = _userRepository.GetUserByUsername(userName);
            return user.UserId;
        }
        [HttpGet]
        public IActionResult RateMovie(int orderId)
        {
            var order = _userRepository.GetOrderById(orderId);
            if (order == null || order.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            var viewModel = new RateMovieViewModel
            {
                OrderId = orderId,
                MovieTitle = order.Movie.Title,
                Rating = order.Rating // Existing rating or default to 0
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult RateMovie(RateMovieViewModel model)
        {
            var order = _userRepository.GetOrderById(model.OrderId);
            if (order == null || order.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            // Update the rating
            order.Rating = model.Rating;
            _userRepository.UpdateOrder(order);

            return RedirectToAction("Orders");
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userName = HttpContext.Session.GetString("UserName"); // Assuming you're using session for user management
            var user = _userRepository.GetUserByUsername(userName);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImagePath = user.ProfileImagePath // Show current profile image
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            _log.LogInformation("Attempting to edit user profile");

            if (ModelState.IsValid)
            {
                // Retrieve the existing user from the database
                var user = _userRepository.GetUserByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound(); // Return a 404 if the user is not found
                }

                // Update user properties
                user.UserName = model.UserName;
                user.Email = model.Email;

                // Handle the profile image upload
                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images"); // Adjust the path as needed
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the new profile image
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfileImage.CopyTo(fileStream);
                    }

                    // Update the user's profile image path
                    user.ProfileImagePath = uniqueFileName; // Adjust this to match your property
                }

                // Save changes in the repository
                _userRepository.UpdateUser(user); // Assuming you have an UpdateUser method

                // Redirect to the Profile action after successful update
                TempData["SuccessMessage"] = "Profile edited successfully.";
                return RedirectToAction("Profile", new { id = model.UserId });
            }

            // Log any errors
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _log.LogError(error.ErrorMessage);
            }

            // If we get here, something went wrong; return the view with the current model
            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            // Call the SearchMovies method from the repository
            var movies = _userRepository.SearchMovies(query);

            // If no movies were found or regex pattern is invalid, display error
            if (movies == null || !movies.Any())
            {
                ViewBag.ErrorMessage = "No movies found matching your search criteria.";
            }

            return View("SearchResults", movies);
        }
        [HttpGet]
        public IActionResult MovieDetails(int movieId)
        {
            // Find the movie by ID
            var movie = _userRepository.GetMovieById(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            // Assuming you have the user's ID from the session or authentication context
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login"); 
            }
            var user = _userRepository.GetUserByIdAsync(userId.Value);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

    }
}
