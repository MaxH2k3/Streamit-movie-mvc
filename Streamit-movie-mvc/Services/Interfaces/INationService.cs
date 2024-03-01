using Streamit_movie_mvc.Models.Domain;

namespace Movies.Repository
{
    public interface INationService
    {
        IEnumerable<Nation> GetNations();
    }
}
