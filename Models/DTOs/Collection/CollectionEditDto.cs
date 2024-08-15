﻿using SharedLeit;

namespace ModelsLeit.DTOs.Collection
{
    public record CollectionEditDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? Name { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public CollectionStatus Status { get; set; }
        public long CardsQ { get; set; }
        public long BoxCount { get; set; }
    }
}