using Backend.Application.DTOs.Social;
using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Core.Entities.TrainingRelated;
using Backend.Core.Enums;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly AppDbContext _db;

    public FriendshipRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<UserFriendship?> GetAsync(Guid userId1, Guid userId2)
    {
        return await _db.UserFriendships.FirstOrDefaultAsync(f =>
            (f.RequesterId == userId1 && f.AddresseeId == userId2) ||
            (f.RequesterId == userId2 && f.AddresseeId == userId1));
    }

    public async Task<UserFriendship?> GetByIdAsync(Guid friendshipId)
    {
        return await _db.UserFriendships.FindAsync(friendshipId);
    }

    public async Task<List<UserFriendship>> GetFriendsAsync(Guid userId)
    {
        return await _db.UserFriendships
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .Where(f => f.Status == FriendshipStatus.Accepted &&
                        (f.RequesterId == userId || f.AddresseeId == userId))
            .ToListAsync();
    }

    public async Task<List<UserFriendship>> GetPendingRequestsAsync(Guid addresseeId)
    {
        return await _db.UserFriendships
            .Include(f => f.Requester)
            .Where(f => f.Status == FriendshipStatus.Pending && f.AddresseeId == addresseeId)
            .ToListAsync();
    }

    public async Task AddAsync(UserFriendship friendship)
    {
        _db.UserFriendships.Add(friendship);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserFriendship friendship)
    {
        if (_db.Entry(friendship).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
            _db.UserFriendships.Update(friendship);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserFriendship friendship)
    {
        _db.UserFriendships.Remove(friendship);
        await _db.SaveChangesAsync();
    }

    public async Task<List<LeaderboardEntryDto>> GetFriendsLeaderboardAsync(Guid userId)
    {
        var friendUserIds = await _db.UserFriendships
            .Where(f => f.Status == FriendshipStatus.Accepted &&
                        (f.RequesterId == userId || f.AddresseeId == userId))
            .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
            .ToListAsync();

        friendUserIds.Add(userId);
        return await BuildLeaderboardAsync(friendUserIds);
    }

    public async Task<List<LeaderboardEntryDto>> GetGlobalLeaderboardAsync(int top = 50)
    {
        var userIds = await _db.UserInfos
            .Join(_db.UserWorkloadStats, ui => ui.Id, s => s.UserInfoId, (ui, s) => ui.UserId)
            .Distinct()
            .Take(top * 2)
            .ToListAsync();

        return (await BuildLeaderboardAsync(userIds)).Take(top).ToList();
    }

    private async Task<List<LeaderboardEntryDto>> BuildLeaderboardAsync(List<Guid> userIds)
    {
        var users = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Username, u.AvatarUrl })
            .ToListAsync();

        var volumes = await _db.UserInfos
            .Where(ui => userIds.Contains(ui.UserId))
            .Select(ui => new
            {
                ui.UserId,
                TotalVolume = _db.UserWorkloadStats
                    .Where(s => s.UserInfoId == ui.Id && s.Category == WorkloadStatCategory.Muscle)
                    .Sum(s => (float?)s.AccumulatedVolume) ?? 0f
            })
            .ToListAsync();

        var volumeMap = volumes.ToDictionary(v => v.UserId, v => v.TotalVolume);

        return users
            .Select(u => new LeaderboardEntryDto
            {
                UserId = u.Id,
                Username = u.Username,
                AvatarUrl = u.AvatarUrl,
                TotalVolume = volumeMap.GetValueOrDefault(u.Id, 0f)
            })
            .OrderByDescending(e => e.TotalVolume)
            .Select((e, i) => { e.Rank = i + 1; return e; })
            .ToList();
    }
}
