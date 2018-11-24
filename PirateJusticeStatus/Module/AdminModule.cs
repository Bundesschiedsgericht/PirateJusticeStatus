using Nancy;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using PirateJusticeStatus.Model;
using PirateJusticeStatus.ViewModel;
using PirateJusticeStatus.Infrastructure;

namespace PirateJusticeStatus.Module
{
	public class AdminModule : NancyModule
    {
		private IDatabase _database;

		public AdminModule()
        {
			this.RequiresAuthentication();
			this.RequiresClaims("admin");

            _database = Global.Database;

			Get["/admin"] = parameters =>
            {
				return Response.AsRedirect("/admin/courts");
            };

			Get["/admin/courts"] = parameters =>
            {
                return View["View/admin_courts.sshtml", new PartyModel<AdminCourtModel>(_database.Query<Court>().Select(c => new AdminCourtModel(_database, c)).OrderBy(c => c.Name))];
            };

            Get["/admin/court/{id}"] = parameters =>
            {
				string idString = parameters.id;
                Guid id;

				if (Guid.TryParse(idString, out id))
				{
					var court = _database.Query<Court>(id);

					if (court == null)
					{
						Global.Log.Warning("Unknown court at admin view");
						return Response.AsRedirect("/error/unknownobject");
					}
					else
					{
                        return View["View/admin_court.sshtml", new AdminCourtModel(_database, court)];
					}
				}
				else if (idString == "new")
				{
                    return View["View/admin_court.sshtml", new AdminCourtModel()];
				}
				else
				{
					Global.Log.Warning("Bad uid at admin view");
					return Response.AsRedirect("/error/unknownobject");
				}
            };

			Post["/admin/court/{id}"] = parameters =>
            {
				var updateCourt = this.Bind<AdminCourtModel>();
                var updateJudges = this.Bind<List<JudgeModel>>();
				string idString = parameters.id;
				Guid id;

                if (Guid.TryParse(idString, out id))
                {
                    _database.BeginTransaction();

                    try
                    {
                        var currentCourt = _database.Query<Court>(id);

                        if (currentCourt == null)
                        {
                            currentCourt = new Court(id);
                            updateCourt.Update(_database, currentCourt, updateJudges);
                            _database.Insert(currentCourt);
                        }
                        else
                        {
                            updateCourt.Update(_database, currentCourt, updateJudges);
                            _database.Update(currentCourt);
                        }

                        _database.CommitTransaction();
                    }
                    catch
                    {
                        _database.AbortTransaction();
                        throw;
                    }

                    Global.Log.Notice("Update of " + updateCourt.Name + " by admin");
                    Global.Mail.SendAdmin("[PJS] Admin Update", "Update of " + updateCourt.Name + " by admin");

                    return Response.AsRedirect("/admin/courts");
                }
				else
				{
					Global.Log.Warning("Bad uid at admin update");
					return Response.AsRedirect("/error/unknownobject");
				}
            };
        }
    }
}
