using System.ComponentModel;

namespace OrdersScreen.ViewModels
{
    public class OrderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Update()
        {
            // Update all properties in a  single event
            OnPropertyChanged("");
        }

        public string Id { get; set; } = "-";
        public string CreationDate { get; set; } = "-";
        public string Advisor { get; set; } = "-";
        public string Account { get; set; } = "-";
        public string Symbol { get; set; } = "-";
        public string OrdType { get; set; } = "-";
        public string OrdQty { get; set; } = "-";
        public string OrdQtyApparent { get; set; } = "-";
        public string LeavesQty { get; set; } = "-";
        public string CancelledQty { get; set; } = "-";
        public string CumQty { get; set; } = "-";
        public string TotalValue { get; set; } = "-";
        public string AvailableValue { get; set; } = "-";
        public string GoalValue { get; set; } = "-";
    }
}
