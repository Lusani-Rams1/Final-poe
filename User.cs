namespace Prog_Poe.Models
{
    public enum UserRole
    {
        Lecturer,
        Coordinator,
        Manager
    }

    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public UserRole Role { get; set; }
        public string Password { get; set; } // In a real app, hash this!
    }
}