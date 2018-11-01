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
	public class CourtModule : NancyModule
    {
		private IDatabase _database;

        public CourtModule()
        {
			this.RequiresAuthentication();

            _database = Global.Database;

			Get["/courts"] = parameters =>
            {
				return View["View/update_courts.sshtml", new PartyModel<UpdateCourtModel>(_database.Query<Court>().Select(c => new UpdateCourtModel(c)).OrderBy(c => c.Name))];
            };

            Get["/court/{id}"] = parameters =>
            {
				string idString = parameters.id;
                Guid id;

				if (Guid.TryParse(idString, out id))
				{
					this.RequiresAnyClaim("admin", idString);

					var court = _database.Query<Court>(id);

					if (court == null)
					{
						Global.Log.Warning("Unknown court at update");
						return Response.AsRedirect("/error/unknownobject");
					}
					else
					{
						return View["View/update_court.sshtml", new UpdateCourtModel(court)];
					}
				}
				else
				{
					Global.Log.Warning("Bad uid at update");
					return Response.AsRedirect("/error/unknownobject");
				}
            };

			Post["/court/{id}"] = parameters =>
            {
				var updateCourt = this.Bind<UpdateCourtModel>();
                var updateJudges = this.Bind<List<JudgeModel>>();
				string idString = parameters.id;
				Guid id;

				if (Guid.TryParse(idString, out id))
				{
					this.RequiresAnyClaim("admin", idString);

					_database.BeginTransaction();
					try
					{
                        var currentCourt = _database.Query<Court>(id);

						if (currentCourt == null)
						{
							Global.Log.Warning("Unknown court at update");
							return Response.AsRedirect("/error/unknownobject");
						}
						else
						{
							updateCourt.Update(currentCourt, updateJudges);
							_database.Update(currentCourt);
						}

                        _database.CommitTransaction();

						Global.Log.Notice("Update of " + currentCourt.Name + " by " + this.Context.CurrentUser.UserName);
						Global.Mail.SendAdmin("[PJS] Update", "Update of " + currentCourt.Name + " by " + this.Context.CurrentUser.UserName);

						return Response.AsRedirect("/court/updated");
					}
					catch (Exception exception)
					{
                        Global.Log.Error(exception.ToString());
						_database.AbortTransaction();
						throw;
					}
				}
				else
				{
					Global.Log.Warning("Bad uid at update");
					return Response.AsRedirect("/error/unknownobject");
				}
            };

			Get["/court/updated"] = parameters =>
            {
                return View["View/updated.sshtml"];
            };
        }
    }
}
