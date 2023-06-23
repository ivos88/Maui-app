using Microsoft.Maui.Graphics;
using TodoSQLite.Data;
using TodoSQLite.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.ComponentModel;

namespace TodoSQLite.Views;

[QueryProperty("Item", "Item")]
public partial class TodoItemPage : ContentPage
{
    private readonly TodoItemDatabase database;

    public TodoItem Item
    {
        get => BindingContext as TodoItem;
        set => BindingContext = value;
    }

    public TodoItemPage(TodoItemDatabase todoItemDatabase)
    {
        InitializeComponent();
        database = todoItemDatabase;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CheckIfPageContainsObject();
    }

    private void CheckIfPageContainsObject()
    {
        if (string.IsNullOrEmpty(Item.Afbeelding))
        {
            LblAfbeelding.Source = "person.png";
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Item.VoorNaam))
        {
            await DisplayAlert("Naam Benodigd", "Vul een voornaam in alsjeblieft.", "OK");
            return;
        }

        await database.SaveItemAsync(Item);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool shouldDelete = await DisplayAlert("Verwijderen", "Weet u het zeker?", "Ja", "Nee");
        if (shouldDelete)
        {
            try
            {
                if (Item.ID == 0)
                    return;

                await database.DeleteItemAsync(Item);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception)
            {
                await DisplayAlert("Foutmelding", "Er is iets misgegaan, probeer opnieuw.", "OK");
            }
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void CallButton_Clicked(object sender, EventArgs e)
    {
        bool shouldCall = await DisplayAlert("Telefoonnummer Bellen", $"Wilt u {Item.VoorNaam} {Item.AchterNaam} bellen?", "Ja", "Nee");
        if (shouldCall)
        {
            try
            {
                Uri phoneNumber = new Uri($"tel:{Item.TelefoonNummer}");
                await Launcher.OpenAsync(phoneNumber);
            }
            catch (Exception)
            {
                await DisplayAlert("Foutmelding Telefoonnummer", "Het gekozen telefoonnummer is niet geldig.", "OK");
            }
        }
    }

    private async void PhotoButton_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.IsCaptureSupported)
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using var sourceStream = await photo.OpenReadAsync();
                using var localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);

                var fileInfo = new FileInfo(photo?.FullPath);
                Item.Afbeelding = fileInfo.ToString();

                LblAfbeelding.Source = fileInfo.ToString();
            }
        }
    }

    private async void GallaryPhotoButton_Clicked(object sender, EventArgs e)
    {
        var selectphoto = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images
        });

        if (selectphoto == null)
            return;

        var stream = await selectphoto.OpenReadAsync();
        ImageSource image = ImageSource.FromStream(() => stream);

        var fileInfo = new FileInfo(selectphoto?.FullPath);
        Item.Afbeelding = fileInfo.ToString();

        LblAfbeelding.Source = fileInfo.ToString();
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!(sender is RadioButton button))
            return;

        Item.Geslacht = button.Content.ToString();

        if (Item.IsMan)
            radiobuttonman.IsChecked = true;
        else if (Item.IsVrouw)
            radiobuttonvrouw.IsChecked = true;
        else if (Item.IsOnbekend)
            radiobuttononbekend.IsChecked = true;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FullImagePage(Item.Afbeelding));
    }
}