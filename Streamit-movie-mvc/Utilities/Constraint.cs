namespace Streamit_movie_mvc.Utilities;

public static class Constraint
{
    public static IEnumerable<string> Actions = new List<string>
    {
        "Index",
        "Genres",
        "Cast",
        "Movie",
        "AccountDetail",
        "PricingPlan"
    };

    public static class Resource
    {
        public static string VERIFY_CODE_MAIL = "Views/Mail/VerifyCode.cshtml";
        public static string VERIFY_TOKEN_MAIL = "Views/Mail/VerifyToken.cshtml";
        public static string ERROR_MAIL = "Views/Mail/ErrorMail.cshtml";
    }

    public enum TypeEmail {
        CODE = 1,
        TOKEN = 2
    }

    public static class StatusUser
    {
        public static string ACTIVE = "ACTIVE";
        public static string PENDING = "PENDING";
        public static string BLOCK = "BLOCK";
    }

    public static class RoleUser
    {
        public const string ADMIN = "ADMIN";
        public const string USER = "USER";
    }

    public static class RolePerson
    {
        public static string ACTOR = "ACTOR";
        public static string PRODUCER = "PRODUCER";
    }

    public static class StatusMovie
    {
        public static string UPCOMING = "upcoming";
        public static string PENDING = "pending";
        public static string RELEASE = "released";
        public static string DELETED = "deleted";
        public static string ALL_STATUS = "all";
        public static string REVERT = "revert";
        public static IEnumerable<string> ALL = new List<string> { UPCOMING.ToLower(), PENDING.ToLower(), RELEASE.ToLower(), ALL_STATUS.ToLower() };
        public static IEnumerable<string> FILTER = new List<string> { UPCOMING.ToLower(), PENDING.ToLower(), RELEASE.ToLower(), REVERT.ToLower() };

    }

    public static class Avatar
    {
        public static string DEFAULT = "~/home/images/user/user1.html";
    }

}
