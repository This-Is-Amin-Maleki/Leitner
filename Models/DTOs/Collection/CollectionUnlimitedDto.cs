﻿using ModelsLeit.DTOs.User;
using SharedLeit;

namespace ModelsLeit.DTOs.Collection
{
    public record CollectionUnlimitedDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public CollectionStatus Status { get; set; }
        public UserMiniDto User { get; set; }

        public long CardsQ { get; set; }
        public long BoxCount { get; set; }
    }
}
