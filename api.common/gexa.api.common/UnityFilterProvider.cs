using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.Practices.Unity;

namespace api.common
{
    /// <summary>
    /// Filter provider to inject dependencies into atrributes.
    /// 
    /// </summary>
    public class UnityFilterProvider : ActionDescriptorFilterProvider, IFilterProvider
    {
        private IUnityContainer _container;
        private readonly ActionDescriptorFilterProvider _defaultProvider = new ActionDescriptorFilterProvider();

        public UnityFilterProvider(IUnityContainer container)
        {
            _container = container;
        }

        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            // filter out just the authorize filter attributes.
            var attributes = base.GetFilters(configuration, actionDescriptor).Where(x => x.Instance is AuthorizationFilterAttribute).ToList();
            foreach (var attr in attributes)
            {
                _container.BuildUp(attr.Instance.GetType(), attr.Instance);
            }

            return attributes;
        }
    }
}