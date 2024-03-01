using Movies.Interface;
using Streamit_movie_mvc.Models.Domain;

namespace Movies.Repository
{
    public class FeatureService : IFeatureService
    {
        private readonly MOVIESContext _context;

        public FeatureService(MOVIESContext context)
        {
            _context = context;
        }

        public FeatureService()
        {
            _context = new MOVIESContext();
        }

        public IEnumerable<FeatureFilm> GetFeatures()
        {
            return _context.FeatureFilms.ToList();
        }
    }
}
