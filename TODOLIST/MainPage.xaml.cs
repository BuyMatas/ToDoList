using System.Text.Json;
using Microsoft.Maui.Storage;


using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TODOLIST


{
    public class TodoItem : INotifyPropertyChanged
    {
        public string Text { get; set; }
        public bool IsDone { get; set; }

        private string description;
        public string Description
        {
            get => description;
            set
            {
                if (description == value) return;
                description = value;
                OnPropertyChanged(nameof(DescriptionButtonText));
            }
        }

        public string DescriptionButtonText =>
            string.IsNullOrWhiteSpace(Description) ? "Lägg till beskrivning" : Description;

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public partial class MainPage : ContentPage
    {
        public ObservableCollection<TodoItem> TodoList { get; } = new();

        public MainPage()
        {
            InitializeComponent();

            ÅrPicker();
            Månadpicker();
            DagPicker();

            BindingContext = this;

            LoadList();
        }

        void SaveList()
        {
            var json = JsonSerializer.Serialize(TodoList);
            Preferences.Set("SavedTodoList", json);
        }

        void LoadList()
        {
            if (Preferences.ContainsKey("SavedTodoList"))
            {
                string json = Preferences.Get("SavedTodoList", "");
                var list = JsonSerializer.Deserialize<ObservableCollection<TodoItem>>(json);

                if (list != null)
                {
                    TodoList.Clear();
                    foreach (var item in list)
                        TodoList.Add(item);
                }
            }
        }
     
        void OnButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AddListItem.Text))
            {
                TodoList.Add(new TodoItem { Text = AddListItem.Text });
                AddListItem.Text = string.Empty;
                SaveList();
            }
        }

        async void OnDescriptionClicked(object sender, EventArgs e)
        {
            if (sender is Button { BindingContext: TodoItem item })
            {
                string result = await DisplayPromptAsync("Beskrivning", "Skriv en beskrivning:", initialValue: item.Description);
                if (result != null)
                {
                    item.Description = result;

                    SaveList();
                }
            }
        }

        void OnDeleteSwipe(object sender, EventArgs e)
        {
            if (sender is SwipeItem { BindingContext: TodoItem item })
            {
                TodoList.Remove(item);
                SaveList();

            }
                
        }

        private void ÅrPicker()
        {
            int År = DateTime.Now.Year;
            for (int i = 0; i < 10; i++)
            {
                VäljÅr.Items.Add((År + i).ToString());
            }

        }
        private void Månadpicker()
        {

            for (int månad = 1; månad <= 12; månad++)
            {

                VäljMånad.Items.Add(månad.ToString("D2"));

            }
        }
        private void DagPicker()
        {
                for (int Dag = 1; Dag <= 31; Dag++)
                {

                    VäljDag.Items.Add(Dag.ToString("D2"));

                }
        }

    }
}


