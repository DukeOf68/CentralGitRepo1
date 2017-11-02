using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;
using TinyIoC;

namespace DaisyMedia68.Infrastructure
{
    public class TinyIocDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            try
            {
                //return TinyIoCContainer.Current.Resolve<serviceType>();


                return TinyIoCContainer.Current.Resolve(serviceType);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}