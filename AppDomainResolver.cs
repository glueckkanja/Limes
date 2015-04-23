using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Limes
{
    internal class AppDomainResolver : MarshalByRefObject
    {
        private readonly AppDomain _appDomain;

        public AppDomainResolver(AppDomain appDomain)
        {
            _appDomain = appDomain;
        }

        public string Resolve(string name)
        {
            // check if the assembly has already been loaded in the parent domain
            // this is important for when an assembly is loaded directly into the parent domain, e.g. in linqpad
            Assembly assembly = _appDomain.GetAssemblies().FirstOrDefault(x => x.FullName == name);

            if (assembly != null)
            {
                return assembly.Location;
            }

            // try to resolve using reflection only
            assembly = Assembly.ReflectionOnlyLoad(name);

            if (assembly != null)
            {
                return assembly.Location;
            }

            return null;
        }
    }
}