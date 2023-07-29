﻿using Azure;

namespace MuseumAPI.Models
{
    public class Museum
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public DateTime? FoundationDate { get; set; }
        public string? Architect { get; set; }
        public string? Website { get; set; }

        // Hidden from the API because it's not in the DTO
        public virtual ICollection<Artist> Artists { get; set; } = null!;
        public virtual ICollection<Exhibition> Exhibitions { get; set; } = null!;

        public virtual long? UserId { get; set; }
        public virtual User? User { get; set; } = null!;
    }
}
