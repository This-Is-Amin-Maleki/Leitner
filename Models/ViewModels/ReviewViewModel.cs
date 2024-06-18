using ModelsLeit.Entities;
using System.Reflection;

namespace ModelsLeit.ViewModels
{
    public record ReviewViewModel
    {
        public long BoxId { get; set; }
        public string CollectionName { get; set; }
        public ICollection<CardViewModel> Cards { get; set; }
    }
}