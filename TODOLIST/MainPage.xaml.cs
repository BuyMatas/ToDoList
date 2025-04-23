using System.Collections.ObjectModel;

namespace TODOLIST
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<string> TodoList { get; set; }
        

        public MainPage()
        {
            InitializeComponent();

            TodoList = new ObservableCollection<string>();

            BindingContext = this;

        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(AddListItem.Text))
            {
                TodoList.Add(AddListItem.Text); 
            }
        }
    }

}
