using Microsoft.EntityFrameworkCore;
using StudentChat.Models;

namespace WpfStudentChat.Models.Database;

public class ChatDbContext(DbContextOptions<ChatDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<User> User { get; set; }
    public DbSet<GroupMessage> GroupMessages { get; set; }
    public DbSet<PrivateMessage> PrivateMessages { get; set; }
    public DbSet<Attachment> Attachment { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO

        modelBuilder.Entity<User>()
            .HasKey(v => v.Id);

        modelBuilder.Entity<Attachment>()
            .HasKey(v => v.AttachmentHash);

        modelBuilder.Entity<GroupMessage>()
            .HasKey(v => v.Id);

        modelBuilder.Entity<GroupMessage>()
            .HasMany(v => v.FileAttachments);
        modelBuilder.Entity<GroupMessage>()
            .HasMany(v => v.ImageAttachments);

        modelBuilder.Entity<PrivateMessage>()
            .HasKey(v => v.Id);

        modelBuilder.Entity<PrivateMessage>()
            .HasMany(v => v.FileAttachments);
        modelBuilder.Entity<PrivateMessage>()
            .HasMany(v => v.ImageAttachments);
    }
}
