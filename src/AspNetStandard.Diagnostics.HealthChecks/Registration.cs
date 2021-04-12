using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public class Registration // ToDo: É legal pensar em alguma forma de executar o dispose dos checks, principalmente quando não for singleton, pois as conexões precisam ser liberadas.
    {
        public Registration(IHealthCheck instance)
        {
            Instance = instance;
            IsSingleton = true;
        }

        public Registration(Type type)
        {
            Type = type;
            IsSingleton = false;
        }

        public IHealthCheck Instance { get; }

        public Type Type { get; set; }

        public bool IsSingleton { get; }
    }
}