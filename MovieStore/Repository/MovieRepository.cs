using MovieStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MovieStore.Repository
{
    public class MovieRepository : IMovie
    {
        private readonly MovieStoreContext _context;

        public MovieRepository(MovieStoreContext context)
        {
            _context = context;
        }
        public int GetTotalMovies()
        {
            return _context.Movies.Count(); // Assuming Movies is your DbSet<Movie>
        }

        public int GetTotalUsers()
        {
            return _context.Users.Count(); // Assuming Users is your DbSet<User>
        }

        public List<Movie> GetAllMovies()
        {
            return _context.Movies.ToList();
        }

        public Movie GetMovieById(int id)
        {
            return _context.Movies.Find(id);
        }

        public void AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges(); // Make sure to save changes to the database
        }

        public void UpdateMovie(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteMovie(int id)
        {
            var movie = GetMovieById(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
            }
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList(); // Adjust based on your context
        }

        

        // Method to get total movies
       

        // Method to get all movies
        

    }
}