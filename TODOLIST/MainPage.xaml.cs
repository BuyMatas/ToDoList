using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;

namespace TODOLIST
{
    // klass för varje att-göra-post
    public class TodoItem : INotifyPropertyChanged
    {
        public string Text { get; set; }
        public bool IsDone { get; set; }

        public string DeadlineText { get; set; } = "";

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

    // huvudsidan
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<TodoItem> TodoList { get; } = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            InitDatumPickers();
            LoadList();
        }

        // ladda listan
        void LoadList()
        {
            if (Preferences.ContainsKey("SavedTodoList"))
            {
                var json = Preferences.Get("SavedTodoList", "");
                var list = JsonSerializer.Deserialize<ObservableCollection<TodoItem>>(json);
                if (list != null)
                {
                    TodoList.Clear();
                    foreach (var item in list)
                        TodoList.Add(item);
                }
            }
        }

        // spara listan
        void SaveList()
        {
            var json = JsonSerializer.Serialize(TodoList);
            Preferences.Set("SavedTodoList", json);
        }

        // Lägg till knappen
        void OnButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AddListItem.Text))
            {
                DateTime deadline = GetValtDatumEllerIdag();
                string deadlineText = FormatDeadline(deadline);

                TodoList.Add(new TodoItem
                {
                    Text = AddListItem.Text,
                    DeadlineText = deadlineText
                });

                AddListItem.Text = string.Empty;
                SaveList();
            }
        }

        // beskrivnings knappen
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

        // ta bort swipe
        void OnDeleteSwipe(object sender, EventArgs e)
        {
            if (sender is SwipeItem { BindingContext: TodoItem item })
            {
                TodoList.Remove(item);
                SaveList();
            }
        }

        // datumväljare
        private void InitDatumPickers()
        {
            int år = DateTime.Now.Year;
            for (int i = 0; i < 10; i++)
                VäljÅr.Items.Add((år + i).ToString());

            for (int m = 1; m <= 12; m++)
                VäljMånad.Items.Add(m.ToString("D2"));

            for (int d = 1; d <= 31; d++)
                VäljDag.Items.Add(d.ToString("D2"));

            // fyll i med dagens datum
            VäljÅr.SelectedItem = DateTime.Now.Year.ToString();
            VäljMånad.SelectedItem = DateTime.Now.Month.ToString("D2");
            VäljDag.SelectedItem = DateTime.Now.Day.ToString("D2");
        }

        // returnera valt datum eller dagens datum
        private DateTime GetValtDatumEllerIdag()
        {
            try
            {
                if (VäljÅr.SelectedIndex >= 0 && VäljMånad.SelectedIndex >= 0 && VäljDag.SelectedIndex >= 0)
                {
                    int år = int.Parse(VäljÅr.SelectedItem.ToString());
                    int månad = int.Parse(VäljMånad.SelectedItem.ToString());
                    int dag = int.Parse(VäljDag.SelectedItem.ToString());

                    return new DateTime(år, månad, dag);
                }
            }
            catch { }

            return DateTime.Today;
        }

        // formatera deadlinen
        private string FormatDeadline(DateTime deadline)
        {
            var idag = DateTime.Today;
            var diff = (deadline - idag).Days;
            var absDiff = Math.Abs(diff);

            if (diff == 0)
                return "Idag";
            if (diff == 1)
                return "Imorgon";
            if (diff == -1)
                return "Igår";

            if (diff > 1 && diff <= 7)
                return $"Om {diff} dagar";
            if (diff > 7 && diff < 30)
                return $"Om {diff / 7} veckor";
            if (diff >= 30 && diff < 365)
                return $"Om {diff / 30} månader";
            if (diff >= 365)
                return $"Om {diff / 365} år";

            if (diff < -1 && diff >= -3)
                return "För några dagar sedan";
            if (diff < -3 && diff >= -7)
                return $"{absDiff} dagar sedan";
            if (diff < -7 && diff > -30)
                return $"{absDiff / 7} veckor sedan";
            if (diff <= -30 && diff > -365)
                return $"{absDiff / 30} månader sedan";
            if (diff <= -365)
                return $"{absDiff / 365} år sedan";

            return deadline.ToString("d MMM yyyy", new System.Globalization.CultureInfo("sv-SE"));
        }
    }
}
