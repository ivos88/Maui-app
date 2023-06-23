using SQLite;
using TodoSQLite.Models;

namespace TodoSQLite.Data;

public class TodoItemDatabase
{
    SQLiteAsyncConnection Database;
    public TodoItemDatabase()
    {
    }
    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        var result = await Database.CreateTableAsync<TodoItem>();
    }

    public async Task<List<TodoItem>> GetItemsAsync()
    {
        await Init();
        return await Database.Table<TodoItem>().ToListAsync();
    }

    public async Task<TodoItem> GetItemAsync(int id)
    {
        await Init();
        return await Database.Table<TodoItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveItemAsync(TodoItem item)
    {
        await Init();
        if (item.ID != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    public async Task<int> DeleteItemAsync(TodoItem item)
    {
        await Init();
        return await Database.DeleteAsync(item);
    }

    public async Task<List<TodoItem>> GetItemsBySearchAsync(string search)
    {
        return await Database.Table<TodoItem>().Where(p =>
            p.VoorNaam.ToLower().Contains(search.ToLower()) ||
            p.AchterNaam.ToLower().Contains(search.ToLower()))
            .ToListAsync();
    }

}