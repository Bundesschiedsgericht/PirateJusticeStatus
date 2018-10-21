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
	public class PublicModule : NancyModule
    {
		private IDatabase _database;

		public PublicModule()
        {
            _database = Global.Database;

			Get["/"] = parameters =>
			{
				return View["View/public_courts.sshtml", new PartyModel<PublicCourtModel>(_database.Query<Court>().Select(c => new PublicCourtModel(c)).OrderBy(c => c.Name))];
			};
        }
    }
}
