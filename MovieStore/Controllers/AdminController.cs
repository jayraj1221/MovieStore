    using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieStore.Models;
using MovieStore.Repository;

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
            var model = new DashboardViewModel
            {
                TotalMovies = _movieRepository.GetTotalMovies(),
                TotalUsers = _movieRepository.GetTotalUsers(),
                TotalOrders = _movieRepository.GetTotalOrders(),
                Movies = _movieRepository.GetAllMovies(),
                Users = _movieRepository.GetAllUsers(), // Get users here
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
            var movies =  _movieRepository.GetAllMovies();
            return View(movies);
        }

        [HttpGet]
        public IActionResult AddMovie()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMovie(AddMovieModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle the file upload
                if (model.PosterImage != null && model.PosterImage.Length > 0)
                {
                    // Define the folder where the uploaded images will be saved
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images/movies");

                    // Generate a unique file name for the uploaded image
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PosterImage.FileName;

                    // Create the full file path
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.PosterImage.CopyTo(fileStream);
                    }

                    // Save movie details, including the image path
                    var movie = new Movie
                    {
                        Title = model.Title,
                        Genre = model.SelectedGenre,
                        ReleaseDate = model.ReleaseDate,
                        Price = model.Price,
                        ImagePath = uniqueFileName // Store the unique image file name
                    };

                    // Call the repository to save the movie (repository method needs to be implemented)
                    _movieRepository.AddMovie(movie);

                    // Redirect to a different action after the movie is added, e.g., movie list or manage page
                    return RedirectToAction("ManageMovies");
                }
                else
                {
                    ModelState.AddModelError("PosterImage", "Movie poster upload failed.");
                }
            }

            // If the model is invalid, return the view with validation errors
    
            return View(model);
        }

        [HttpGet]
        public IActionResult EditMovie(int id)
        {
            _logger.LogInformation($"Attempting to edit movie with ID: {id}");
            var movie = _movieRepository.GetMovieById(id); // Fetch movie from DB

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
                _logger.LogInformation("HERE IT IS");
            if (ModelState.IsValid)
            {
                // Retrieve the existing movie from the database
                var movie = _movieRepository.GetMovieById(model.MovieId);
                if (movie == null)
                {
                    return NotFound(); // Return a 404 if the movie is not found
                }

                // Update movie properties
                movie.Title = model.Title;
                movie.Genre = model.SelectedGenre; // Assuming SelectedGenre is a string matching the genre
                movie.ReleaseDate = model.ReleaseDate;
                movie.Price = model.Price;


                // Handle the poster image upload
                if (model.PosterImage != null && model.PosterImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images/movies"); // Adjust the path as needed
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PosterImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the new poster image
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.PosterImage.CopyTo(fileStream);
                    }

                    // Update the movie's poster path
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
            // Call the repository to delete the movie
            var movie = _movieRepository.GetMovieById(movieId);
            if (movie == null)
            {
                return NotFound(); // Return a 404 if the movie is not found
            }

            _movieRepository.DeleteMovie(movieId); // Assuming you have a DeleteMovie method in your repository

            // Redirect to the ManageMovies action after successful deletion
            return RedirectToAction("ManageMovies");
        }


        // GET: Admin/DetailsMovie/5
        [HttpGet]
        public IActionResult DetailsMovie(int id)
        {
            var movie = _movieRepository.GetMovieById(id);
            if (movie == null)
            {
                return NotFound(); // Return a 404 if the movie is not found
            }

            return View(movie); // Pass the movie model to the view
        }
        [HttpGet]
        public IActionResult Search(string query)
        {
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
            var users = _movieRepository.GetAllUsers();
            var nonAdminUsers = users.Where(u => u.Role != "Admin").ToList();
            return View(nonAdminUsers);
        }

        public IActionResult UserOwnedMovies(int id)
        {
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

    }
}
