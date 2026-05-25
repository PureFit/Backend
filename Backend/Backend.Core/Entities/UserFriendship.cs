namespace Backend.Core.Entities;

public enum FriendshipStatus { Pending, Accepted }

public class UserFriendship
{
    public Guid Id { get; set; }

    public Guid RequesterId { get; set; }
    public User Requester { get; set; } = null!;

    public Guid AddresseeId { get; set; }
    public User Addressee { get; set; } = null!;

    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
