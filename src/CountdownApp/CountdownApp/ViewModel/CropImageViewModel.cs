using System.Windows.Input;

namespace CountdownApp
{
  public class CropImageViewModel : ViewModelBase
  {
    private readonly EditCountdownViewModel parent;
    private readonly NavigationService navigationService = new NavigationService();

    private SelectedImage selectedImage;

    private Command applyCommand = null;
    private Command cancelCommand = null;

    public SelectedImage SelectedImage
    {
      get { return selectedImage; }
      set { SetProperty(ref selectedImage, value); }
    }

    public ICommand ApplyCommand
    {
      get { return applyCommand ?? (applyCommand = new Command(Apply)); }
    }

    public ICommand CancelCommand
    {
      get { return cancelCommand ?? (cancelCommand = new Command(Cancel)); }
    }

    public CropImageViewModel(EditCountdownViewModel parent)
    {
      this.parent = parent;
    }

    public async void Apply()
    {
      var bytes = await SelectedImage.AsBytes();

      parent.ChangeImage(bytes);
      navigationService.GoBack();
    }

    public void Cancel()
    {
      navigationService.GoBack();
    }
  }
}