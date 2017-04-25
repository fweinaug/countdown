using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace CountdownApp
{
  public class NavigationService
  {
    private Frame Frame
    {
      get
      {
        var frame = AppShell.Current?.AppFrame;
        return frame;
      }
    }

    public void EditCountdown(EditCountdownViewModel viewModel)
    {
      Frame.Navigate(typeof(EditCountdownPage), viewModel);
    }

    public async void SelectImage(EditCountdownViewModel viewModel)
    {
      var picker = new FileOpenPicker
      {
        ViewMode = PickerViewMode.Thumbnail,
        SuggestedStartLocation = PickerLocationId.PicturesLibrary
      };

      picker.FileTypeFilter.Add(".jpg");
      picker.FileTypeFilter.Add(".jpeg");
      picker.FileTypeFilter.Add(".png");
      picker.FileTypeFilter.Add(".bmp");

      var file = await picker.PickSingleFileAsync();
      if (file == null)
        return;

      var image = await SelectedImage.FromFile(file);

      if (!image.IsValid)
        return;

      if (image.IsSquare)
      {
        var imageData = await image.AsBytes();

        viewModel.ChangeImage(imageData);
        return;
      }

      var cropViewModel = new CropImageViewModel(viewModel)
      {
        SelectedImage = image
      };

      Frame.Navigate(typeof(CropImagePage), cropViewModel);
    }

    public void ShowSettings()
    {
      Frame.Navigate(typeof(SettingsPage), null);
    }

    public void GoBack()
    {
      Frame.GoBack();
    }
  }
}