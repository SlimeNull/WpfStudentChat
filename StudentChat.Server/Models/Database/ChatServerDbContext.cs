using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StudentChat.Server.Models.Database
{
    public class ChatServerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<ImageBinary> Images { get; set; }
        public DbSet<FileBinary> Files { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<GroupRequest> GroupRequests { get; set; }
        public DbSet<PrivateMessageImageAttachment> PrivateMessageImageAttachments { get; set; }
        public DbSet<PrivateMessageFileAttachment> PrivateMessageFileAttachments { get; set; }
        public DbSet<GroupMessageImageAttachment> GroupMessageImageAttachments { get; set; }
        public DbSet<GroupMessageFileAttachment> GroupMessageFileAttachments { get; set; }

        public ChatServerDbContext(DbContextOptions<ChatServerDbContext> options) : base(options)
        {

        }

        public Task<bool> CheckUserHasFriendAsync(int userId, int friendUserId)
        {
            return UserFriends.AnyAsync(f => f.FromUserId == userId && f.ToUserId == friendUserId || f.FromUserId == friendUserId && f.ToUserId == userId);
        }

        public async Task<bool> CheckUserHasGroupAsync(int userId, int groupId)
        {
            bool owned = await Groups.AnyAsync(g => g.OwnerId == userId && g.Id == groupId);
            if (owned)
                return true;

            bool joined = await GroupMembers.AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
            return joined;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Group.Onwer - User.OwnerGroups
            modelBuilder.Entity<Group>()
                .HasOne(e => e.Owner)
                .WithMany(e => e.OwnedGroups)
                .HasForeignKey(e => e.OwnerId)
                .IsRequired();

            // Group.Members - User.JoinedGroups
            modelBuilder.Entity<Group>()
                .HasMany(e => e.Members)
                .WithMany(e => e.JoindGroups)
                .UsingEntity<GroupMember>(
                    l => l.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict),
                    r => r.HasOne(e => e.Group).WithMany().HasForeignKey(e => e.GroupId).OnDelete(DeleteBehavior.Restrict));

            // User.AddedFriends - User.AcceptedFriends
            modelBuilder.Entity<User>()
                .HasMany(e => e.AddedFriends)
                .WithMany(e => e.AcceptedFriends)
                .UsingEntity<UserFriend>(
                    l => l.HasOne(e => e.FromUser).WithMany().HasForeignKey(e => e.FromUserId).OnDelete(DeleteBehavior.Restrict),
                    r => r.HasOne(e => e.ToUser).WithMany().HasForeignKey(e => e.ToUserId).OnDelete(DeleteBehavior.Restrict));

            modelBuilder.Entity<User>()
                .HasMany(e => e.SentFriendRequests)
                .WithOne(e => e.Sender)
                .HasForeignKey(e => e.SenderId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(e => e.ReceivedFriendRequests)
                .WithOne(e => e.Receiver)
                .HasForeignKey(e => e.ReceiverId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(e => e.SentGroupRequests)
                .WithOne(e => e.Sender)
                .HasForeignKey(e => e.SenderId)
                .IsRequired();

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Requests)
                .WithOne(e => e.Group)
                .HasForeignKey(e => e.GroupId)
                .IsRequired();

            // Group.Messages
            modelBuilder.Entity<Group>()
                .HasMany(e => e.Messages)
                .WithOne(e => e.Group)
                .HasForeignKey(e => e.GroupId)
                .IsRequired();

            // User.SentMessages
            modelBuilder.Entity<User>()
                .HasMany(e => e.SentPrivateMessages)
                .WithOne(e => e.Sender)
                .HasForeignKey(e => e.SenderId)
                .IsRequired();

            // User.ReceivedMessages
            modelBuilder.Entity<User>()
                .HasMany(e => e.ReceivedPrivateMessages)
                .WithOne(e => e.Receiver)
                .HasForeignKey(e => e.ReceiverId)
                .IsRequired();

            // User.SentGroupMessages
            modelBuilder.Entity<User>()
                .HasMany(e => e.SentGroupMessages)
                .WithOne(e => e.Sender)
                .HasForeignKey(e => e.SenderId)
                .IsRequired();

            // PrivateMessage.ImageAttachments
            modelBuilder.Entity<PrivateMessage>()
                .HasMany(e => e.ImageAttachments)
                .WithOne(e => e.Message)
                .HasForeignKey(e => e.MessageId)
                .IsRequired();

            // PrivateMessage.FileAttachments
            modelBuilder.Entity<PrivateMessage>()
                .HasMany(e => e.FileAttachments)
                .WithOne(e => e.Message)
                .HasForeignKey(e => e.MessageId)
                .IsRequired();

            // GroupMessage.ImageAttachments
            modelBuilder.Entity<GroupMessage>()
                .HasMany(e => e.ImageAttachments)
                .WithOne(e => e.Message)
                .HasForeignKey(e => e.MessageId)
                .IsRequired();

            // GroupMessage.FileAttachments
            modelBuilder.Entity<GroupMessage>()
                .HasMany(e => e.FileAttachments)
                .WithOne(e => e.Message)
                .HasForeignKey(e => e.MessageId)
                .IsRequired();

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
                // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
                // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
                // use the DateTimeOffsetToBinaryConverter
                // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
                // This only supports millisecond precision, but should be sufficient for most use cases.
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                        || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        modelBuilder
                            .Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }
                }
            }

        }
    }
}
