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
		public string Status { get; set; }
		public string LoadStatus { get; set; }

		public PublicCourtModel()
        {
			Id = Guid.NewGuid();
			Name = string.Empty;
			Status = string.Empty;
			LoadStatus = string.Empty;
        }

        private string ComputeJudgeStatus(Court court)
        {
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

            if (court.Name.StartsWith("Bundes", StringComparison.InvariantCulture))
            {
                switch (count)
                {
                    case 0:
                        return "Unbesetzt";
                    case 1:
                    case 2:
                        return "Handlungsunfähig";
                    case 3:
                    case 4:
                        return "Minimale Besetzung";
                    case 5:
                    case 6:
                        return "Ausreichende Besetzung";
                    case 7:
                    case 8:
                        return "Gute Besetzung";
                    case 9:
                    case 10:
                        return "Sehr gute Besetzung";
                    default:
                        return "Hervorragende Besetzung";
                }
            }
            else
            {
                switch (count)
                {
                    case 0:
                        return "Unbesetzt";
                    case 1:
                    case 2:
                        return "Handlungsunfähig";
                    case 3:
                        return "Minimale Besetzung";
                    case 4:
                        return "Ausreichende Besetzung";
                    case 5:
                        return "Gute Besetzung";
                    case 6:
                        return "Sehr gute Besetzung";
                    default:
                        return "Hervorragende Besetzung";
                }
            }
        }

        public PublicCourtModel(Court court)
        {
			Id = court.Id;
			Name = court.Name.Sanatize();

            if (court.Substitute != null)
            {
                Status = "Vertreten durch " + court.Substitute.Name;
            }
            else if (court.LastUpdate.AddDays(55) < DateTime.Now)
            {
                Status = "Unbekannt (Keine Rückmeldung)";
            }
            else
            {
                Status = ComputeJudgeStatus(court);

                if (court.LastUpdate.AddDays(45) < DateTime.Now)
                {
                    Status += "(Rückmeldung überfällig)";
                }
                else if (court.LastUpdate.AddDays(35) < DateTime.Now)
                {
                    Status += "(Rückmeldung erwartet)";
                }
            }

			LoadStatus = court.CaseLoad.Translate();
        }
    }
}
