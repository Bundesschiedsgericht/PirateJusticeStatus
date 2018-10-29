using System;
using System.Collections.Generic;
using PirateJusticeStatus.Model;
using System.Linq;
using PirateJusticeStatus.Util;

namespace PirateJusticeStatus.ViewModel
{
	public class UpdateCourtModel : PublicCourtModel
    {
		public string PendingCases { get; set; }
		public List<SelectOption> PendingCasesOptions { get; set; }
		public JudgeModel Judge1 { get; set; }
		public JudgeModel Judge2 { get; set; }
		public JudgeModel Judge3 { get; set; }
		public JudgeModel Judge4 { get; set; }
		public JudgeModel Judge5 { get; set; }
		public JudgeModel Judge6 { get; set; }
		public JudgeModel Judge7 { get; set; }
		public JudgeModel Judge8 { get; set; }
		public JudgeModel Judge9 { get; set; }

        public UpdateCourtModel()
			: base()
        {
			PendingCases = string.Empty;
            PendingCasesOptions = new List<SelectOption>();
            foreach (CaseLoad pending in Enum.GetValues(typeof(CaseLoad)))
            {
                PendingCasesOptions.Add(new SelectOption(pending.ToString(), pending.Translate().ConvertToHtml(), false));
            }

			Judge1 = new JudgeModel();
			Judge2 = new JudgeModel();
			Judge3 = new JudgeModel();
			Judge4 = new JudgeModel();
			Judge5 = new JudgeModel();
			Judge6 = new JudgeModel();
			Judge7 = new JudgeModel();
			Judge8 = new JudgeModel();
			Judge9 = new JudgeModel();
        }

		public UpdateCourtModel(Court court)
			: base(court)
        {
            PendingCases = court.CaseLoad.ToString();
			PendingCasesOptions = new List<SelectOption>();
			foreach (CaseLoad pending in Enum.GetValues(typeof(CaseLoad)))
			{
				PendingCasesOptions.Add(new SelectOption(pending.ToString(), pending.Translate().ConvertToHtml(), pending == court.CaseLoad));
			}

			Judge1 = court.Judges.Count >= 1 ? new JudgeModel(court.Judges.Skip(0).First()) : new JudgeModel();
			Judge2 = court.Judges.Count >= 2 ? new JudgeModel(court.Judges.Skip(1).First()) : new JudgeModel();
			Judge3 = court.Judges.Count >= 3 ? new JudgeModel(court.Judges.Skip(2).First()) : new JudgeModel();
			Judge4 = court.Judges.Count >= 4 ? new JudgeModel(court.Judges.Skip(3).First()) : new JudgeModel();
			Judge5 = court.Judges.Count >= 5 ? new JudgeModel(court.Judges.Skip(4).First()) : new JudgeModel();
			Judge6 = court.Judges.Count >= 6 ? new JudgeModel(court.Judges.Skip(5).First()) : new JudgeModel();
			Judge7 = court.Judges.Count >= 7 ? new JudgeModel(court.Judges.Skip(6).First()) : new JudgeModel();
			Judge8 = court.Judges.Count >= 8 ? new JudgeModel(court.Judges.Skip(7).First()) : new JudgeModel();
			Judge9 = court.Judges.Count >= 9 ? new JudgeModel(court.Judges.Skip(8).First()) : new JudgeModel();
        }

		public virtual void Update(Court court, IEnumerable<JudgeModel> judges)
		{
			court.CaseLoad = PendingCases.TryParseEnum(CaseLoad.None);

			var realJudges = new List<JudgeModel>(judges.Where(j => j.Name.HasContent()));

			court.Judges.RemoveAll(c => !realJudges.Any(n => n.Id.Equals(c.Id)));
            
            foreach (var n in realJudges)
			{
				var c = court.Judges.Where(x => x.Id.Equals(n.Id)).SingleOrDefault();

                if (c != null)
				{
					n.Update(c);
				}
				else
				{
					var cn = new Judge(n.Id, court);
					n.Update(cn);
					court.Judges.Add(cn);
				}
			}
		}
    }
}
