using Streamit_movie_mvc.Models.Domain;

namespace Movies.Interface
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();
    }
}
