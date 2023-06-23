using System.Collections.ObjectModel;
using TodoSQLite.Data;
using TodoSQLite.Models;

namespace TodoSQLite.Views;

public partial class TodoListPage : ContentPage
{
    TodoItemDatabase database;
    public ObservableCollection<TodoItem> Items { get; set; } = new();
    public TodoListPage(TodoItemDatabase todoItemDatabase)
	{
		InitializeComponent();
        database = todoItemDatabase;
        BindingContext = this;

    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var items = await database.GetItemsAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Items.Clear();

            foreach (var item in items)

                if (item.Afbeelding == null)
                {
                    item.Afbeelding = "person.png";
                    Items.Add(item);
                }
                else
                {
                    Items.Add(item);
                }     

        });
    }
    async void OnItemAdded(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TodoItemPage), true, new Dictionary<string, object>
        {
            ["Item"] = new TodoItem()

        });

    }

    private async void  CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TodoItem item)
            return;

        await Shell.Current.GoToAsync(nameof(TodoItemPage), true, new Dictionary<string, object>
        {
            ["Item"] = item
        });
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            // Geen zoekterm, toon alle items
            var items = await database.GetItemsAsync();
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
        else
        {
            // Zoekterm aanwezig, filter de items op basis van de zoekterm
            var filteredItems = await database.GetItemsBySearchAsync(searchText);
            Items.Clear();
            foreach (var item in filteredItems)
            {
                Items.Add(item);
            }
        }
    }
}

