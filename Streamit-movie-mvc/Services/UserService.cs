using AutoMapper;
using MimeKit;
using MongoDB.Driver;
using Movies.Business.globals;
using Movies.Business.users;
using Movies.Interface;
using Movies.Models;
using Movies.Security;
using Streamit_movie_mvc.Models.Domain;
using Streamit_movie_mvc.Utilities;
using System.Net;
using Streamit_movie_mvc.Models.DTO;
using Newtonsoft.Json.Linq;

namespace Movies.Repository;

public class UserService : IUserService
{
    private readonly MOVIESContext _context;
    private readonly IAuthentication _authentication;
    private readonly MovieMongoContext _MongoContext;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;

    public UserService(MOVIESContext context, IAuthentication authentication, 
        IMapper mapper, MovieMongoContext mongoContext, IMailService mailRepository)
    {
        _context = context;
        _authentication = authentication;
        _mapper = mapper;
        _MongoContext = mongoContext;
        _mailService = mailRepository;
    }

    public UserService(IAuthentication authentication, IMapper mapper, IMailService mailRepository)
    {
        _context = new MOVIESContext();
        _MongoContext = new MovieMongoContext();
        _authentication = authentication;
        _mapper = mapper;
        _mailService = mailRepository;
    }

    public User? GetUser(string username)
    {
        return GetUsers().FirstOrDefault(o => o.Username.Equals(username) || o.Email.Equals(username));
    }

    public IEnumerable<User> GetUsers()
    {
        return _context.Users.ToList();
    }

    public async Task<ResponseDTO> Register(RegisterUser registerUser)
    {
        if(isExisted("username", registerUser.Username))
        {
            return new ResponseDTO(HttpStatusCode.Conflict, "UserName have been existed!");
        } else if (isExisted("email", registerUser.Email))
        {
            return new ResponseDTO(HttpStatusCode.Conflict, "Email have been existed!");
        }

        _authentication.CreatePasswordHash(registerUser.Password, out byte[] passwordHash, out byte[] passwordSalt);

        //create temporary user
        Guid id = Guid.NewGuid();
        await _MongoContext.Users.InsertOneAsync(new UserTemporary()
        {
            Email = registerUser.Email,
            Role = registerUser.Role?.ToUpper(),
            UserId = id,
            Password = passwordHash,
            PasswordSalt = passwordSalt,
            Username = registerUser.Username,
            Status = Constraint.StatusUser.PENDING,
            Avatar = Constraint.Avatar.DEFAULT,
            DisplayName = registerUser.FirstName + " " + registerUser.LastName,
            ExpiredDate = DateTime.UtcNow.AddMinutes(30)
        });
        //create token verify
        string token = _authentication.CreateRandomToken();
        int code = new Random().Next(1000, 10000);
        await _MongoContext.Tokens.InsertOneAsync(new VerifyToken()
        {
            UserId = id,
            Token = token,
            Code = code,
            CreatedDate = DateTime.UtcNow.AddMinutes(5),
            ExpiredDate = DateTime.UtcNow.AddMinutes(30)
        }); ;

        var result = await _mailService.SendUsingTemplateFromFile(Constraint.Resource.VERIFY_CODE_MAIL, "Verify your account", new UserMail()
        {
            UserName = registerUser.Username,
            UserId = id.ToString(),
            Token = token,
            Email = registerUser.Email,
            Code = code
        });

        if (result)
        {
            return new ResponseDTO(HttpStatusCode.Created, "Register successfully! Please check your email to verify your account!", id);
        }

        return new ResponseDTO(HttpStatusCode.ServiceUnavailable, "Fail to send an email!");

    }

    public bool isExisted(string? field, string? value)
    {
        if(field.ToLower().Equals("username"))
        {
            return GetUsers().Any(u => u.Username.ToLower().Equals(value.ToLower()));
        } else if(field.ToLower().Equals("email"))
        {
            return GetUsers().Any(u => u.Email.ToLower().Equals(value.ToLower()));
        }
        return false;
        
    }

    public ResponseDTO? Login(UserDTO userDTO)
    {
        var user = GetUser(userDTO.UserName);
        if(user == null)
        {
            return new ResponseDTO(HttpStatusCode.NotFound, "UserName or Email not existed!");
        }
        if(!_authentication.VerifyPasswordHash(userDTO.Password, user.Password, user.PasswordSalt))
        {
            return new ResponseDTO(HttpStatusCode.BadRequest, "Wrong password!");
        }
        if (user.Status.Equals(Constraint.StatusUser.BLOCK))
        {
            return new ResponseDTO(HttpStatusCode.Forbidden, "Your account is blocked by admin!");
        }
        return new ResponseDTO(HttpStatusCode.OK, "Login successfully!", user);
    }

    public async Task<ResponseDTO> VerifyAccount(string token, Guid userId, int type)
    {
        if(type == (int)Constraint.TypeEmail.CODE)
        {
            return await VerifyCode(Int32.Parse(token), userId);
        } else if (type == (int)Constraint.TypeEmail.TOKEN)
        {
            return await VerifyToken(token, userId);
        }
        return new ResponseDTO(HttpStatusCode.BadRequest, "Type not found!");
        
    }

    private async Task<ResponseDTO> VerifyCode(int code, Guid userId)
    {
        var verifyCode = await _MongoContext.Tokens.FindAsync(o => o.UserId.Equals(userId)).Result.FirstOrDefaultAsync();
        if (verifyCode == null)
        {
            return new ResponseDTO(HttpStatusCode.NotFound, "Not found Account need to verify!");
        } else if (verifyCode.Code != code)
        {
            return new ResponseDTO(HttpStatusCode.BadRequest, "Fail to validate number!");
        } else if (verifyCode.CreatedDate < DateTime.UtcNow)
        {
            return new ResponseDTO(HttpStatusCode.BadRequest, "Token expired!");
        }
        UserTemporary userTemporary = await _MongoContext.Users.FindAsync(o => o.UserId.Equals(verifyCode.UserId)).Result.FirstOrDefaultAsync();
        if (userTemporary == null)
        {
            return new ResponseDTO(HttpStatusCode.NotFound, "User not found!");
        }

        User user = _mapper.Map<User>(userTemporary);
        //active account
        if (await ActiveAccount(user))
        {
            //delete token
            await _MongoContext.Tokens.DeleteOneAsync(o => o.UserId.Equals(verifyCode.UserId));
            //delete user temporary
            await _MongoContext.Users.DeleteOneAsync(o => o.UserId.Equals(verifyCode.UserId));
            return new ResponseDTO(HttpStatusCode.OK, "Verify successfully!");
        }
        return new ResponseDTO(HttpStatusCode.BadRequest, "Fail to verify!");
    }

    private async Task<ResponseDTO> VerifyToken(string token, Guid userId)
    {
        var verifyToken = await _MongoContext.Tokens.FindAsync(o => o.UserId.Equals(userId)).Result.FirstOrDefaultAsync();
        if (verifyToken == null)
        {
            return new ResponseDTO(HttpStatusCode.NotFound, "Not found Account need to verify!");
        } else if (!verifyToken.Token.Equals(token))
        {
            return new ResponseDTO(HttpStatusCode.BadRequest, "Fail to validate token!");
        } else if (verifyToken.CreatedDate < DateTime.UtcNow)
        {
            return new ResponseDTO(HttpStatusCode.BadRequest, "Token expired!");
        }
        UserTemporary userTemporary = await _MongoContext.Users.FindAsync(o => o.UserId.Equals(verifyToken.UserId)).Result.FirstOrDefaultAsync();
        if (userTemporary == null)
        {
            return new ResponseDTO(HttpStatusCode.NotFound, "User not found!");
        }

        User user = _mapper.Map<User>(userTemporary);
        //active account
        if (await ActiveAccount(user))
        {
            //delete token
            await _MongoContext.Tokens.DeleteOneAsync(o => o.UserId.Equals(verifyToken.UserId));
            //delete user temporary
            await _MongoContext.Users.DeleteOneAsync(o => o.UserId.Equals(verifyToken.UserId));
            return new ResponseDTO(HttpStatusCode.OK, "Verify successfully!");
        }
        return new ResponseDTO(HttpStatusCode.BadRequest, "Fail to verify!");
    }

    public async Task<bool> ActiveAccount(User user)
    {
        if(GetUser(user.Username) != null || GetUser(user.Email) != null)
        {
            return false;
        }
        user.Status = Constraint.StatusUser.ACTIVE;
        user.DateCreated = DateTime.UtcNow;
        await _context.Users.AddAsync(user);
        if(await _context.SaveChangesAsync() > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<ResponseDTO> ResendToken(Guid userId)
    {
        //delete token
        var verifyToken = await _MongoContext.Tokens.FindAsync(o => o.UserId.Equals(userId)).Result.FirstOrDefaultAsync();
        if(verifyToken == null)
        {
            return new ResponseDTO(HttpStatusCode.NotFound, "Your token not existed! You need to register first!");
        }
        verifyToken.Token = _authentication.CreateRandomToken();
        verifyToken.CreatedDate = DateTime.UtcNow.AddMinutes(5);
        await _MongoContext.Tokens.ReplaceOneAsync(o => o.UserId.Equals(userId), verifyToken);


        var userTemporary = await _MongoContext.Users.FindAsync(o => o.UserId.Equals(userId)).Result.FirstOrDefaultAsync();
        if(userTemporary == null)
        {
            return new ResponseDTO(HttpStatusCode.NotFound, "User not found!");
        }

        var result = await _mailService.SendUsingTemplateFromFile(Constraint.Resource.VERIFY_TOKEN_MAIL, "Verify your account", new UserMail()
        {
            UserName = userTemporary.Username,
            UserId = userId.ToString(),
            Token = verifyToken.Token,
            Email = userTemporary.Email
        });

        if(result)
            return new ResponseDTO(HttpStatusCode.OK, "Check your email!");

        return new ResponseDTO(HttpStatusCode.ServiceUnavailable, "Fail to send an email!");
    }
}
