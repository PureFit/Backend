using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Entities
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public Sex Sex { get; set; }
        public Level Level { get; set; }
        public decimal WeightKg { get; set; }
        public decimal HeightCm { get; set; }
        public int Age { get; set; }

        public User User { get; set; }
        public Guid UserId { get; set; }
    }

    public enum Sex
    {
        Man,
        Woman
    }

    public enum Level
    {
        Beginner,
        Returning,
        Intermediate,
        Advanced,
        Elite
    }
}
