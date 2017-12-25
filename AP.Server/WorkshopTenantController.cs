namespace AP.Server
{
    using AP.Business.Registration.ReadModel;
    using AP.EntityModel.ReadModel;
    using Microsoft.AspNetCore.Mvc;

    public abstract class WorkshopTenantController : Controller
    {
        private WorkshopAlias workshopAlias;
        private string workshopCode;

        protected WorkshopTenantController(IWorkshopDao workshopDao)
        {
            WorkshopDao = workshopDao;
        }

        public IWorkshopDao WorkshopDao { get; private set; }

        public string WorkshopCode
        {
            get
            {
                return workshopCode ??
                    (workshopCode = (string)ControllerContext.RouteData.Values["workshopCode"]);
            }
            internal set { workshopCode = value; }
        }

        public WorkshopAlias WorkshopAlias
        {
            get
            {
                return workshopAlias ??
                    (workshopAlias = WorkshopDao.GetWorkshopAlias(WorkshopCode));
            }
            internal set { this.workshopAlias = value; }
        }

        //TODO: Move ActionFilters into separate class

        /*
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!string.IsNullOrEmpty(this.ConferenceCode) &&
                this.ConferenceAlias == null)
            {
                filterContext.Result = new HttpNotFoundResult("Invalid conference code.");
            }
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            if (filterContext.Result is ViewResultBase)
            {
                this.ViewBag.Conference = this.ConferenceAlias;
            }
        }
        */
    }
}