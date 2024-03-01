using Movies.Business.globals;
using Movies.Business.users;
using Streamit_movie_mvc.Models.Domain;
using Streamit_movie_mvc.Models.DTO;

namespace Movies.Interface
{
    public interface IUserService
    {
        User? GetUser(string username);
        IEnumerable<User> GetUsers();
        Task<ResponseDTO> Register(RegisterUser registerUser);
        ResponseDTO? Login(UserDTO userDTO);
        Task<ResponseDTO> VerifyAccount(string token, Guid userId, int type);
        Task<ResponseDTO> ResendToken(Guid userId);
    }
}
