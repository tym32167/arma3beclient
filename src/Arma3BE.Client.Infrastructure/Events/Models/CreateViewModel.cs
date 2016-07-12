using System.Windows.Controls;

namespace Arma3BE.Client.Infrastructure.Events.Models
{
    public class CreateViewModel<T>
    {
        public ContentControl Parent { get; }
        public T ViewModel { get; }

        public CreateViewModel(ContentControl parent, T viewModel)
        {
            Parent = parent;
            ViewModel = viewModel;
        }
    }
}