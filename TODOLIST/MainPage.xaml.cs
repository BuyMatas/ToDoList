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
        public ObservableCollection<TodoItem> TodoList { get; set; } // uppdaterar

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
                AddListItem.Text = "";
            }
        }

        private void OnDeleteSwipe(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;
            var item = swipeItem?.BindingContext as TodoItem;

            if (item != null && TodoList.Contains(item))
            {
                TodoList.Remove(item);
            }
        }

    }
}
