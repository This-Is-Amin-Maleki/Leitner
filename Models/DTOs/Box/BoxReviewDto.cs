using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using System.Reflection;

namespace ModelsLeit.DTOs.Box
{
    public record BoxReviewDto
    {
        public long BoxId { get; set; }
        public string CollectionName { get; set; }
        public ICollection<CardDto> Cards { get; set; }
    }
}