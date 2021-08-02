using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public class RegistrationWcf
    {
        internal RegistrationWcf(IHealthCheck instance)
        {
            Instance = instance;
            IsSingleton = true;
        }

        internal RegistrationWcf(Type type)
        {
            Type = type;
            IsSingleton = false;
        }

        internal IHealthCheck Instance { get; }

        internal Type Type { get; set; }

        internal bool IsSingleton { get; }
    }
}
