using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieStore.Models;
using MovieStore.Repository;
using NuGet.Protocol.Core.Types;

namespace MovieStore.Controllers
{

    public class AdminController : Controller
    {

        private readonly IMovie _movieRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminController> _logger;
        public AdminController(IMovie movieRepository,IWebHostEnvironment env, ILogger<AdminController> logger)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _env = env;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        public IActionResult Dashboard()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            var model = new DashboardViewModel
            {
                TotalMovies = _movieRepository.GetTotalMovies(),
                TotalUsers = _movieRepository.GetTotalUsers() - 1,
                TotalOrders = _movieRepository.GetTotalOrders(),
                Movies = _movieRepository.GetAllMovies(),
                Users = _movieRepository.GetAllUsers(), 
                GenreTransactionData = _movieRepository.GetGenreTransactionDataAsync(),
                TrendingMovies = _movieRepository.GetTrendingMovies()
            };
            foreach (var genre in model.GenreTransactionData)
            {
                _logger.LogInformation($"Genre: {genre.Key}, Transactions: {genre.Value}");
            }
            return View(model);
        }

        public IActionResult ManageMovies()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            var movies =  _movieRepository.GetAllMovies();
            return View(movies);
        }

        [HttpGet]
        public IActionResult AddMovie()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMovie(AddMovieModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            if (ModelState.IsValid)
            {
                
                if (model.PosterImage != null && model.PosterImage.Length > 0)
                {
                    
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images/movies");

                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PosterImage.FileName;

                    
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.PosterImage.CopyTo(fileStream);
                    }

                    
                    var movie = new Movie
                    {
                        Title = model.Title,
                        Genre = model.SelectedGenre,
                        ReleaseDate = model.ReleaseDate,
                        Price = model.Price,
                        ImagePath = uniqueFileName 
                    };

                    
                    _movieRepository.AddMovie(movie);

                    
                    return RedirectToAction("ManageMovies");
                }
                else
                {
                    ModelState.AddModelError("PosterImage", "Movie poster upload failed.");
                }
            }


    
            return View(model);
        }

        [HttpGet]
        public IActionResult EditMovie(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            _logger.LogInformation($"Attempting to edit movie with ID: {id}");
            var movie = _movieRepository.GetMovieById(id); 

            if (movie == null)
            {
                return NotFound();
            }

            var model = new EditMovieModel
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                SelectedGenre = movie.Genre,
                ReleaseDate = movie.ReleaseDate,
                Price = movie.Price,
                ImagePath = movie.ImagePath,
               
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMovie(EditMovieModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            _logger.LogInformation("HERE IT IS");
            if (ModelState.IsValid)
            {
                
                var movie = _movieRepository.GetMovieById(model.MovieId);
                if (movie == null)
                {
                    return NotFound(); 
                }

                
                movie.Title = model.Title;
                movie.Genre = model.SelectedGenre; 
                movie.ReleaseDate = model.ReleaseDate;
                movie.Price = model.Price;


                
                if (model.PosterImage != null && model.PosterImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images/movies"); 
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PosterImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.PosterImage.CopyTo(fileStream);
                    }

                    // 
                    movie.ImagePath = uniqueFileName; // Adjust this to match your property
                }

                // Save changes in the repository
                _movieRepository.UpdateMovie(movie); // Assuming you have an UpdateMovie method

                // Redirect to the ManageMovies action after successful update
                return RedirectToAction("ManageMovies");
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError(error.ErrorMessage);
            }


            // If we get here, something went wrong; return the view with the current model
            // Ensure genres are populated again for the view
            return View(model);
        }
        // GET: Admin/DeleteMovie/5
        [HttpGet]
        public IActionResult DeleteMovie(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            // Retrieve the movie details based on the ID if needed for display
            var movie = _movieRepository.GetMovieById(id);
            if (movie == null)
            {
                return NotFound(); // Return a 404 if the movie is not found
            }
            // Return the view with the movie details for confirmation
            return View(movie);
        }

        // POST: Admin/DeleteMovie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMovieConfirm(int movieId)
        {

            var movie = _movieRepository.GetMovieById(movieId);
            if (movie == null)
            {
                return NotFound(); 
            }
            bool isPurchased = _movieRepository.IsMoviePurchased(movieId); 

            if (isPurchased)
            {

                TempData["ErrorMessage"] = "Cannot delete the movie as it has been purchased.";
                return RedirectToAction("DeleteMovie",new {id = movieId}); 
            }

            _movieRepository.DeleteMovie(movieId); 

            
            return RedirectToAction("ManageMovies");
        }


        // GET: Admin/DetailsMovie/5
        [HttpGet]
        public IActionResult DetailsMovie(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            var movie = _movieRepository.GetMovieById(id);
            if (movie == null)
            {
                return NotFound(); 
            }
            var ratings = _movieRepository.GetMovieReviews(id);

            
            double averageRating = 0;
            if (ratings.Any())
            {
                averageRating = ratings.Average();
            }


            ViewBag.AverageRating = averageRating;

            return View(movie); 
        }
        [HttpGet]
        public IActionResult Search(string query)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            // Call the SearchMovies method from the repository
            var movies = _movieRepository.SearchMovies(query);

            // If no movies were found or regex pattern is invalid, display error
            if (movies == null || !movies.Any())
            {
                ViewBag.ErrorMessage = "No movies found matching your search criteria.";
            }

            return View(movies);
        }

        public IActionResult TotalUsers()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            var users = _movieRepository.GetAllUsers();
            var nonAdminUsers = users.Where(u => u.Role != "Admin").ToList();
            return View(nonAdminUsers);
        }

        public IActionResult UserOwnedMovies(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            User user1 = _movieRepository.GetAllUsers().FirstOrDefault(u => u.UserId == id);
            var ownedMovies = _movieRepository.GetUserOwnedMovies(id);
            var totalSpent = _movieRepository.GetTotalMoneySpentByUser(id);
            var moviesByGenre = _movieRepository.GetMoviesByGenre(id);

            var viewModel = new UserOwnedMoviesViewModel
            {
                user = user1,
                OwnedMovies = ownedMovies,
                TotalSpent = totalSpent,
                MoviesByGenre = moviesByGenre
            };

            return View(viewModel);
        }
        // GET: Admin/DetailsMovie/5
        [HttpGet]
        public IActionResult Ownedby(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            var movie = _movieRepository.GetMovieById(id);
            if (movie == null)
            {
                return NotFound(); // Return a 404 if the movie is not found
            }

            var users = _movieRepository.GetUsersOwningMovie(id);

            ViewBag.Movie = movie; // Pass the movie directly via ViewBag
            ViewBag.Users = users; // Pass the list of users directly via ViewBag

            return View();
        }
        public IActionResult TotalOrders()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }
            // Fetching all the orders from the repository
            var orders = _movieRepository.GetAllOrders();

            // Passing the orders list to the view
            return View(orders);
        }


    }
}
