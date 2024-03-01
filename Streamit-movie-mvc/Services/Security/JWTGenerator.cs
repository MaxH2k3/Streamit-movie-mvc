using Streamit_movie_mvc.Models.DTO;
using System.Security.Claims;

namespace Movies.Security;

public interface JWTGenerator
{
    string? GenerateToken(UserDTO user);
    IEnumerable<Claim>? GetClaimsFromToken(string token);
}
