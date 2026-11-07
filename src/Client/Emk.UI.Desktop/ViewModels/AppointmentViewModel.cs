using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Emk.Domain.DTOs;
using Emk.Domain.Interfaces;

namespace Emk.UI.Desktop.ViewModels
{
    /// <summary>Логика отображения окна записи. Работает только через IAppointmentService.</summary>
    public class AppointmentViewModel : INotifyPropertyChanged
    {
        private readonly IAppointmentService _service;
        private string _status = "";

        public AppointmentViewModel(IAppointmentService service) => _service = service;

        public DateTime Date { get; set; } = DateTime.Today;
        public string PatientId { get; set; } = "";
        public ObservableCollection<Slot> Slots { get; } = new();
        public Slot? SelectedSlot { get; set; }

        public string Status
        {
            get => _status;
            private set { _status = value; OnPropertyChanged(); }
        }

        public async Task LoadAsync()
        {
            try
            {
                var (response, fromCache) = await _service.GetSlotsAsync(DateOnly.FromDateTime(Date));
                Slots.Clear();
                foreach (var slot in response.Slots) Slots.Add(slot);
                Status = fromCache ? "Нет связи со шлюзом, показано сохраненное расписание" : $"Свободно слотов: {Slots.Count}";
            }
            catch (Exception ex)
            {
                Status = "Ошибка: " + ex.Message;
            }
        }

        public async Task BookAsync()
        {
            if (SelectedSlot is null) { Status = "Выберите время приема"; return; }
            try
            {
                var appointment = await _service.BookAsync(SelectedSlot, PatientId, null);
                Status = $"Вы записаны на {appointment.Slot.StartTime:dd.MM HH:mm}";
            }
            catch (Exception ex)
            {
                Status = "Ошибка: " + ex.Message;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
    }
}
