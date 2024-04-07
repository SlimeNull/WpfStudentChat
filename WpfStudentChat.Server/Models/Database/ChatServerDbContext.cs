using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WpfStudentChat.Server.Models.Database
{
    public class ChatServerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<ImageBinary> Images { get; set; }
        public DbSet<FileBinary> Files { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<PrivateMessageImageAttachment> PrivateMessageImageAttachments { get; set; }
        public DbSet<GroupMessageImageAttachment> GroupMessageImageAttachments { get; set; }

        public ChatServerDbContext(DbContextOptions<ChatServerDbContext> options) : base(options)
        {

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
        }
    }
}
