using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Application.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Query.Mappings
{
    public class CustomerMap : IReadDbMapping
    {
        public void Configure()
        {
            BsonClassMap.TryRegisterClassMap<CustomerQueryModel>(classMap =>
            {

                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);

                classMap.MapMember(customer => customer.Id)
                    .SetIsRequired(true);

                classMap.MapMember(customer => customer.FirstName)
                    .SetIsRequired(true);

                classMap.MapMember(customer => customer.LastName)
                   .SetIsRequired(true);

                classMap.MapMember(customer => customer.DateOfBirth)
                   .SetIsRequired(true)
                   .SetSerializer(new DateTimeSerializer(true));

                classMap.MapMember(customer => customer.PhoneNumber)
                   .SetIsRequired(true);

                classMap.MapMember(customer => customer.Email)
                   .SetIsRequired(true);

                classMap.MapMember(customer => customer.BankAccountNumber)
                   .SetIsRequired(true);


                // Ignore
                classMap.UnmapMember(customer => customer.FullName);

            });
        }
    }
}
