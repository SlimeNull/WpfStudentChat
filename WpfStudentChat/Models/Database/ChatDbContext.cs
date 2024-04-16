using Microsoft.EntityFrameworkCore;
using StudentChat.Models;

namespace WpfStudentChat.Models.Database;

public class ChatDbContext(DbContextOptions<ChatDbContext> dbContextOptions) : DbContext(dbContextOptions)
{

}
