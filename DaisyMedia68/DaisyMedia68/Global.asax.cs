using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DaisyMedia68.Infrastructure;
using TinyIoC;

namespace DaisyMedia68
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //DependencyResolver.SetResolver(new TinyIocDependencyResolver());

            IocRegisterations();
        }

        private void IocRegisterations()
        {
            TinyIoCContainer.Current.Register<IMyInterface>(new MyConcreteImplementationOfMyInterface());
            TinyIoCContainer.Current.Register<MyConcreteClassWithDependency>();

        }
    }

    internal class MyConcreteClassWithDependency
    {
        private MyConcreteImplementationOfMyInterface _dep1;


        internal MyConcreteClassWithDependency(MyConcreteImplementationOfMyInterface dep1)
        {
            _dep1 = dep1;

        }

        internal void TryThis()
        {
            _dep1.SayHello();
        }

    }

    internal class MyConcreteImplementationOfMyInterface : IMyInterface
    {

        internal string ClassVariable1 { get; set; }

        public void SayHello()
        {
            throw new NotImplementedException();
        }
    }

    internal interface IMyInterface
    {
        void SayHello();
    }

}
