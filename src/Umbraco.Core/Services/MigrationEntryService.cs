using System;
using System.Collections.Generic;
using System.Linq;
using Semver;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Umbraco.Core.Services
{
    /// <summary>
    /// Manages migration entries in the database
    /// </summary>
    public sealed class MigrationEntryService : RepositoryService, IMigrationEntryService
    {
        public MigrationEntryService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        /// <summary>
        /// Creates a migration entry, will throw an exception if it already exists
        /// </summary>
        /// <param name="migrationName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IMigrationEntry CreateEntry(string migrationName, SemVersion version)
        {
            var entry = new MigrationEntry
            {
                MigrationName = migrationName,
                Version = version
            };

            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repo = uow.CreateRepository<IMigrationEntryRepository>();
                repo.AddOrUpdate(entry);
                uow.Complete();
            }

            return entry;
        }

        /// <summary>
        /// Finds a migration by name and version, returns null if not found
        /// </summary>
        /// <param name="migrationName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IMigrationEntry FindEntry(string migrationName, SemVersion version)
        {
            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repo = uow.CreateRepository<IMigrationEntryRepository>();
                return repo.FindEntry(migrationName, version);
            }
        }

        /// <summary>
        /// Gets all entries for a given migration name
        /// </summary>
        /// <param name="migrationName"></param>
        /// <returns></returns>
        public IEnumerable<IMigrationEntry> GetAll(string migrationName)
        {
            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repo = uow.CreateRepository<IMigrationEntryRepository>();
                var query = repo.Query
                    .Where(x => x.MigrationName.ToUpper() == migrationName.ToUpper());
                return repo.GetByQuery(query);
            }
        }

    }
}