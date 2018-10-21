using System;
using System.Collections.Generic;
using PirateJusticeStatus.Model;
using System.Linq;

namespace PirateJusticeStatus.ViewModel
{
	public class PartyModel<T>
	{
		public List<T> Courts { get; set; }

		public PartyModel()
        {
			Courts = new List<T>();
        }

		public PartyModel(IEnumerable<T> courts)
        {
			Courts = new List<T>(courts);
        }
    }
}
