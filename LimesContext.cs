using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Limes
{
    public sealed class LimesContext : IDisposable
    {
        private readonly object _domainLock = new object();

        private AppDomain _domain;

        public LimesContext(CancellationToken ct = default(CancellationToken))
        {
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationBase = Path.GetDirectoryName(typeof (LimesContext).Assembly.Location);

            _domain = AppDomain.CreateDomain("Limes Isolated: " + Guid.NewGuid(), null, setup);

            Configurator = Create<AppDomainConfigurator>();
            Configurator.SourceResolver = new AppDomainResolver(AppDomain.CurrentDomain);

            ct.Register(Dispose);
        }

        public AppDomainConfigurator Configurator { get; private set; }

        public void Dispose()
        {
            lock (_domainLock)
            {
                if (_domain != null)
                {
                    Configurator.Dispose();

                    AppDomain.Unload(_domain);
                    _domain = null;
                }
            }
        }

        public T Create<T>()
        {
            try
            {
                return (T) _domain.CreateInstanceAndUnwrap(typeof (T).Assembly.FullName, typeof (T).FullName);
            }
            catch (AppDomainUnloadedException)
            {
                return default(T);
            }
        }
    }
}