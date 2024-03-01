using Microsoft.IdentityModel.Tokens;
using Movies.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Streamit_movie_mvc.Models.DTO;

namespace Movies.Security
{
    public class JWTConfig : JWTGenerator
    {
        private readonly JWTSetting _jwtsetting;
        private readonly IUserService _userService;

        public JWTConfig(JWTSetting jwtsetting, IUserService userService)
        {
            _jwtsetting = jwtsetting;
            _userService = userService;
        }

        public JWTConfig(IUserService userService)
        {
            _jwtsetting = new JWTSetting();
            _userService = userService;
        }

        public string? GenerateToken(UserDTO userDTO)
        {
            var user = _userService.GetUser(userDTO.UserName);
            if (user == null)
            {
                return "Error! Unauthorized.";
            }
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(_jwtsetting.SecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("UserName", user.Username),
                        new Claim("Role", user.Role)
                    }
                ),
                Expires = DateTime.Now.AddMinutes((double) _jwtsetting.TokenExpiry),
                Issuer = _jwtsetting.Issuer,
                Audience = _jwtsetting.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            string finaltoken = tokenhandler.WriteToken(token);

            return finaltoken;
        }

        public IEnumerable<Claim>? GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return jwtToken?.Claims;
        }
    }
}
