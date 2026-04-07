namespace Application.DTOs.Auth
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public List<string>? Roles { get; set; }
    }
}
