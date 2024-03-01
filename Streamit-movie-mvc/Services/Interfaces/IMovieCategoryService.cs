using Movies.Business.globals;
using Streamit_movie_mvc.Models.Domain;

namespace Movies.Repository
{
    public interface IMovieCategoryService
    {
        Task<ResponseDTO> CreateMovieCategory(Guid movieId, IEnumerable<int> MovieCategories);
        IEnumerable<MovieCategory> GetMovieCategories(Guid movieId);
        Task<ResponseDTO> UpdateMovieCategory(Guid movieId, IEnumerable<int> MovieCategories);
        Task<ResponseDTO> DeleteCategoryByMovie(Guid movieId);
    }
}
