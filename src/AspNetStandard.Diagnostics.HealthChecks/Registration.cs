using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    internal class Registration // ToDo: É legal pensar em alguma forma de executar o dispose dos checks, principalmente quando não for singleton, pois as conexões precisam ser liberadas.
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
    }
}