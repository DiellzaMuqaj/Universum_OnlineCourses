using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universum_OnlineCourses.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;        

        public bool EmailConfirmed { get; set; } = false;
        [Column("role_id")]
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<UserCourseAccess> UserCourseAccesses { get; set; } = new List<UserCourseAccess>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
