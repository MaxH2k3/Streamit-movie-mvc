using Streamit_movie_mvc.Models.Domain;

namespace Movies.Interface
{
    public interface IFeatureService
    {
        IEnumerable<FeatureFilm> GetFeatures();
    }
}
