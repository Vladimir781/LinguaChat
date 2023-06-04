using Chat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
namespace Chat.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<AspNetUserCredit> AspNetUserCredits { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public async Task<AspNetUserCredit> GetAspNetUserCredit(ClaimsPrincipal user, ApplicationDbContext context)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return null;
            }

            if (context.Database.GetDbConnection().State != ConnectionState.Open)
            {
                await context.Database.OpenConnectionAsync();
            }

            string query = $"SELECT * FROM AspNetUserCredit WHERE UserId = '{userId}'";
            using (var command = new SqlCommand(query, context.Database.GetDbConnection() as SqlConnection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        // Создаем новый объект AspNetUserCredit и заполняем его данными из строки результата
                        var userCredit = new AspNetUserCredit
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetString(1),
                            TotalUsedTokens = reader.GetInt32(2),
                            CreditGranted = reader.GetInt32(3)
                        };
                        Console.WriteLine(userCredit);
                        reader.Close();
                        return userCredit;
                    }
                    else
                    {
                        // Создаем новый объект AspNetUserCredit со значениями по умолчанию и добавляем его в базу данных
                        var userCredit = new AspNetUserCredit
                        {
                            UserId = userId,
                            TotalUsedTokens = 0,
                            CreditGranted = 0
                        };
                        reader.Close();
                        // Добавляем новый объект в базу данных
                        var insertQuery = $"INSERT INTO AspNetUserCredit (UserId, TotalUsedTokens, CreditGranted) VALUES ('{userCredit.UserId}', {userCredit.TotalUsedTokens}, {userCredit.CreditGranted})";
                        using (var insertCommand = new SqlCommand(insertQuery, context.Database.GetDbConnection() as SqlConnection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                        }

                        Console.WriteLine(userCredit);
                        return userCredit;
                    }
                }
            }
        }


        public void SaveAspNetUserCredit(AspNetUserCredit userCredit, ApplicationDbContext context, int usedTokens)
        {
            if (userCredit == null)
            {
                return;
            }

            var userId = userCredit.UserId;
            var totalUsedTokens = usedTokens;

            if (userId == null)
            {
                return;
            }

            var query = "UPDATE AspNetUserCredit SET TotalUsedTokens +=@totalUsedTokens WHERE UserId = @userId";

            using (var command = new SqlCommand(query, context.Database.GetDbConnection() as SqlConnection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@totalUsedTokens", totalUsedTokens);

                context.Database.OpenConnection();
                command.ExecuteNonQuery();
            }
        }


    }
}
