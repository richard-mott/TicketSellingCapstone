using C868.Capstone.Core.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.ViewModels.Data
{
    public class AuditoriumViewModel : ObservableObject
    {
        private readonly Auditorium auditorium;
        public Auditorium Auditorium => auditorium ?? new Auditorium();

        public int Id => Auditorium.AuditoriumId;

        public string Name
        {
            get => Auditorium.Name;
            set => SetProperty(Auditorium.Name, value, Auditorium,
                (aud, name) => aud.Name = name);
        }

        public int Capacity
        {
            get => Auditorium.Capacity;
            set => SetProperty(Auditorium.Capacity, value, Auditorium,
                (aud, capacity) => aud.Capacity = capacity);
        }

        public AuditoriumViewModel(Auditorium auditorium)
        {
            this.auditorium = auditorium;
        }

        public AuditoriumViewModel Clone()
        {
            return new AuditoriumViewModel(Auditorium.Clone());
        }
    }
}