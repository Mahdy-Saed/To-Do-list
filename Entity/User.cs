using To_Do.Entity;

namespace To_Do.Entity
{
    public class User
    {
        public Guid Id{ get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? Role { get; set; } = "user";

        public List<ToDoTask>? Tasks { get; set; }
    }
}
