using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain.DTO
{
    public class ImportResultDto
    {
        public bool? Status { get; set; }
        public List<UserRegistrationDto>? InvalidUsers { get; set; }
    }
}
