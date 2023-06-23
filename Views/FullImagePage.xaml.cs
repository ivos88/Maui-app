namespace TodoSQLite.Views;

public partial class FullImagePage : ContentPage
{
    public static readonly BindableProperty FullImageSourceProperty =
    BindableProperty.Create(nameof(FullImageSource), typeof(string), typeof(FullImagePage));

    public string FullImageSource
    {
        get => (string)GetValue(FullImageSourceProperty);
        set => SetValue(FullImageSourceProperty, value);
    }

    public FullImagePage(string imageSource)
    {
        InitializeComponent();
        BindingContext = this;

        FullImageSource = imageSource;
        if (string.IsNullOrEmpty(FullImageSource))
        {
            Picture.Source = "person.png";
        }
    }
}