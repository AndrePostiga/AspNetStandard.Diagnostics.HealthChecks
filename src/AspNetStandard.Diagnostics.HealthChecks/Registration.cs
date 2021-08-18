using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    internal class Registration
    {
        internal Registration(IHealthCheck instance)
        {
            Instance = instance;
            IsSingleton = true;
        }

        internal Registration(Type type)
        {
            Type = type;
            IsSingleton = false;
        }

        internal IHealthCheck Instance { get; }

        internal Type Type { get; set; }

        internal bool IsSingleton { get; }

        internal bool Teste = false;
    }
}