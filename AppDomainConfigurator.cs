using System;
using System.IO;
using System.Reflection;

namespace Limes
{
    public class AppDomainConfigurator : MarshalByRefObject, IDisposable
    {
        private readonly AppDomain _appDomain;

        public AppDomainConfigurator()
        {
            _appDomain = AppDomain.CurrentDomain;
            _appDomain.AssemblyResolve += OnAssemblyResolve;
        }

        internal AppDomainResolver SourceResolver { get; set; }

        public void Dispose()
        {
            _appDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        public void SetConsoleOut(TextWriter writer)
        {
            Console.SetOut(writer);
        }

        public void SetConsoleError(TextWriter writer)
        {
            Console.SetError(writer);
        }

        public void SetConsoleIn(TextReader reader)
        {
            Console.SetIn(reader);
        }

        public void LoadAssembly(string path)
        {
            Assembly.LoadFrom(path);
        }

        public void LoadAssembly(byte[] assembly)
        {
            Assembly.Load(assembly);
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string path = null;
            AppDomainResolver resolver = SourceResolver;

            if (resolver != null)
            {
                path = resolver.Resolve(args.Name);
            }

            if (path != null)
            {
                return Assembly.LoadFrom(path);
            }

            return null;
        }
    }
}