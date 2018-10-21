using Nancy;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using PirateJusticeStatus.Model;
using PirateJusticeStatus.ViewModel;

namespace PirateJusticeStatus.Module
{
	public class ErrorModule : NancyModule
    {
		public ErrorModule()
        {
			Get["/error/{id}"] = parameters =>
            {
				string id = parameters.id;

				switch (id)
				{
					case "badkey":
						return "Key mismatch";
					case "unknownobject":
						return "Object not found";
					default:
						return "Unknown error";
				}
            };
        }
    }
}
