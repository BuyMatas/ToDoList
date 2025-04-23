using System.Collections.ObjectModel;

namespace TODOLIST
{
    public class TodoItem
    {
        public string Text { get; set; } 
        public bool IsDone { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        public ObservableCollection<TodoItem> TodoList { get; set; }

        public MainPage()
        {
            InitializeComponent(); 

            TodoList = new ObservableCollection<TodoItem>();

            BindingContext = this;
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AddListItem.Text))
            {
                TodoList.Add(new TodoItem { Text = AddListItem.Text, IsDone = false });
                AddListItem.Text = string.Empty;
            }
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            var itemToRemove = button?.BindingContext as TodoItem;

            if (itemToRemove != null && TodoList.Contains(itemToRemove))
            {
                TodoList.Remove(itemToRemove);
            }
        }
    }
}
