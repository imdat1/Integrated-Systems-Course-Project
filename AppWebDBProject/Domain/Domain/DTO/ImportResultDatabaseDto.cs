using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain.DTO
{
    public class ImportResultDatabaseDto
    {
        public bool? Status { get; set; }
        public List<Database>? InvalidDatabases { get; set; }
    }
}
