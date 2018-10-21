using Nancy;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using Nancy.Security;
using Nancy.Authentication.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using PirateJusticeStatus.Model;
using PirateJusticeStatus.ViewModel;
using PirateJusticeStatus.Infrastructure;

namespace PirateJusticeStatus.Module
{
	public class LoginModel
	{
		public string Password { get; set; }
	}

	public class LoginModule : NancyModule
    {
		private IDatabase _database;

		public LoginModule()
        {
            _database = Global.Database;

			Get["/login"] = parameters =>
            {
				return View["View/login.sshtml", new LoginModel()];
            };

			Get["/login/{id}/{key}"] = parameters =>
            {
				string idString = parameters.id;
				string keyString = parameters.key;
				Guid id;

				if (Guid.TryParse(idString, out id))
				{
					var court = _database.Query<Court>(id);

					if (court != null)
					{
						if (!court.CourtKey.IsNullOrEmpty() &&
							court.CourtKey == keyString)
						{
							var user = new User(court.Name, court.Id.ToString());
							Global.Users.Add(user);
							Global.Log.Notice("Court key login for " + court.Name);
							return this.LoginAndRedirect(user.Id, DateTime.Now.AddDays(14), "/court/" + court.Id.ToString());
						}
						else if (!court.BoardKey.IsNullOrEmpty() &&
								 court.BoardKey == keyString)
						{
							var user = new User(court.Name + " (Vorstand)", court.Id.ToString());
							Global.Users.Add(user);
							Global.Log.Notice("Board key login for " + court.Name);
							return this.LoginAndRedirect(user.Id, DateTime.Now.AddDays(14), "/court/" + court.Id.ToString());
						}
						else
						{
							Global.Log.Info("Key mismatch at login");
							return Response.AsRedirect("/error/badkey");
						}
					}
					else
                    {
						Global.Log.Info("Unkonwn court at login");
                        return Response.AsRedirect("/error/unknownobject");
                    }
				}
				else
                {
					Global.Log.Info("Bad uid at login");
                    return Response.AsRedirect("/error/unknownobject");
                }
            };

			Get["/logout"] = parameters =>
            {
				var user = this.Context.CurrentUser as User;

                if (user != null)
				{
					Global.Users.Remove(user);
				}

				return this.LogoutAndRedirect("/");
            };

			Post["/login/"] = parameters =>
            {
				var login = this.Bind<LoginModel>();

				if (login.Password == Global.Config.AdminPassword)
				{
					var user = new User("Admin", "admin");
					Global.Users.Add(user);
					Global.Log.Notice("Admin logged in");
					return this.LoginAndRedirect(user.Id, DateTime.Now.AddDays(14), "/courts");
				}
				else
				{
					Global.Log.Info("Admin login unsuccessful");
					return Response.AsRedirect("/login");
				}
            };
        }
    }
}
