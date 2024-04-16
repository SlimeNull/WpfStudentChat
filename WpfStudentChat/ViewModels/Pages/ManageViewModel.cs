using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using NPOI.XSSF.UserModel;
using StudentChat.Models;
using WpfStudentChat.Services;

namespace WpfStudentChat.ViewModels.Pages;

public partial class ManageViewModel : ObservableObject
{
    private readonly ChatClientService _chatClientService;
    [ObservableProperty] private ObservableCollection<User> _users = [];

    public ManageViewModel(ChatClientService chatClientService)
    {
        _chatClientService = chatClientService;
    }

    [RelayCommand]
    public async Task LoadUsers()
    {
        var users = await _chatClientService.Client.GetUsers(null, null, 0, 1000000);

        Users = new(users.Users);
    }

    [RelayCommand]
    public async Task ImportUsers() // Id UserName Nickname Password
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            Title = "选择文件",
        };

        if (dialog.ShowDialog() is not true)
            return;

        try
        {
            XSSFWorkbook workbook = new(dialog.FileName);
            var sheet = workbook.GetSheetAt(0);

            var users = new List<(User User, string Password)>();

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row is null)
                    continue;

                var user = new User()
                {
                    Id = (int)(row.GetCell(0)?.NumericCellValue ?? -1),
                    UserName = row.GetCell(1)?.ToString() ?? string.Empty,
                    Nickname = row.GetCell(2)?.ToString() ?? string.Empty,
                };

                var passwordCell = row.GetCell(3);
                var password = passwordCell.ToString();

                if (user.Id < 0)
                {
                    MessageBox.Show("Id 为空或小于0");
                    return;
                }

                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    MessageBox.Show("用户名为空");
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show($"密码为空  Id: {user.Id}");
                    return;
                }

                users.Add((user, password));
            }

            foreach (var (user, password) in users)
            {
                try
                {
                    await _chatClientService.Client.AddUser(user, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password))));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"添加用户失败: {ex.Message}");
                    break;
                }
            }

            await LoadUsers();
            MessageBox.Show("导入成功");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "导入失败");
            return;
        }
    }

}
