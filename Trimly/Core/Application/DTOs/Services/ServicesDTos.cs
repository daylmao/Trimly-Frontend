using Trimly.Core.Application.Utils;

namespace Trimly.Core.Application.DTOs.Services;

public class ServicesDTos
{
        public Guid? ServiceId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public string? ImageUrl { get; set; }
        public Status? Status { get; set; }
        public Guid? RegisteredCompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
}