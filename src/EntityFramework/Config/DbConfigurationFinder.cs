namespace System.Data.Entity.Config
{
    using System.Collections.Generic;
    using System.Linq;

    internal class DbConfigurationFinder
    {
        private readonly IEnumerable<Type> _typesToSearch;

        public DbConfigurationFinder(IEnumerable<Type> typesToSearch)
        {
            _typesToSearch = typesToSearch;
        }

        public virtual DbConfiguration FindConfiguration()
        {
            var configurations = _typesToSearch
                .Where(t => typeof(DbConfiguration).IsAssignableFrom(t))
                .ToList();

            if (configurations.Count == 0)
            {
                return new DbConfiguration();
            }

            if (configurations.Count > 1)
            {
                throw new Exception("Multiple configuration classes defined.");
            }

            return CreateConfiguration(configurations.Single());
        }

        private static DbConfiguration CreateConfiguration(Type configurationType)
        {
            if (!typeof(DbConfiguration).IsAssignableFrom(configurationType))
            {
                throw new InvalidOperationException("Bad type.");
            }

            if (configurationType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException("No constructor.");
            }

            if (configurationType.IsAbstract)
            {
                throw new InvalidOperationException("Is abstract.");
            }

            if (configurationType.IsGenericType)
            {
                throw new InvalidOperationException("Is generic.");
            }

            return (DbConfiguration)Activator.CreateInstance(configurationType);
        }

    }
}