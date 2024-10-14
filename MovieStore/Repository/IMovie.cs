using MovieStore.Models;

namespace MovieStore.Repository
{
    public interface IMovie
    {
        List<Movie> GetAllMovies();
        Movie GetMovieById(int id);
        void AddMovie(Movie movie);
        void UpdateMovie(Movie movie);
        void DeleteMovie(int id);
        public List<User> GetAllUsers();

        public int GetTotalMovies();

        public int GetTotalUsers();

    }
}
