using Microsoft.EntityFrameworkCore;

namespace WpfStudentChat.Models.Database;

public class ChatDbContext(DbContextOptions<ChatDbContext> dbContextOptions) : DbContext(dbContextOptions)
{




}
