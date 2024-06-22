using Mc2.CrudTest.Presentation.Shared.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.AppSettings
{
    public class ConnectionOptions : IAppOptions
    {
        public static string ConfigSectionPath => "ConnectionStrings";

        [Required]
        public string SqlConnection { get; set; }

        [Required]
        public string NoSqlConnection { get; set; }
    }
}
