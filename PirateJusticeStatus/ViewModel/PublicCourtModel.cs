using System;
using System.Collections.Generic;
using System.Linq;
using PirateJusticeStatus.Model;
using PirateJusticeStatus.Util;

namespace PirateJusticeStatus.ViewModel
{
	public class PublicCourtModel
    {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string UpdateStatus { get; set; }
		public string JudgeStatus { get; set; }
		public string LoadStatus { get; set; }

		public PublicCourtModel()
        {
			Id = Guid.NewGuid();
			Name = string.Empty;
			UpdateStatus = string.Empty;
			JudgeStatus = string.Empty;
			LoadStatus = string.Empty;
        }

		public PublicCourtModel(Court court)
        {
			Id = court.Id;
			Name = court.Name.Sanatize();
            
			if (court.LastUpdate.AddDays(55) < DateTime.Now)
            {
				UpdateStatus = "Nachrichtenlos";
            }
			else if (court.LastUpdate.AddDays(45) < DateTime.Now)
            {
				UpdateStatus = "Überfällig";
            }
			else if (court.LastUpdate.AddDays(35) < DateTime.Now)
			{
				UpdateStatus = "Warten";
			}
			else
			{
				UpdateStatus = "OK";
			}

			LoadStatus = court.CaseLoad.Translate();

			int count = 0;
            foreach (var judge in court.Judges)
			{
                switch (judge.Status)
				{
					case Model.JudgeStatus.Available:
						count++;
						break;
				}
			}

            switch (count)
			{
				case 0:
					JudgeStatus = "Unbesetzt";
					break;
				case 1:
				case 2:
					JudgeStatus = "Handlungsunfähig";
					break;
				case 3:
					JudgeStatus = "Minimal";
					break;
				case 4:
					JudgeStatus = "OK";
                    break;
				case 5:
                    JudgeStatus = "Gut";
                    break;
				case 6:
                    JudgeStatus = "Sehr gut";
                    break;
				default:
                    JudgeStatus = "Hervorragend";
                    break;
			}
        }
    }
}
