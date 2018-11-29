
namespace api.common.Controllers
{
    public class ExternalBaseController : BaseController
    {
        public ExternalBaseController(External_Api_Settings settings)
            : base(settings)
        {
            this.External_Api_Settings = settings;
        }

        protected External_Api_Settings External_Api_Settings
        {
            get;
            set;
        }
    }
}