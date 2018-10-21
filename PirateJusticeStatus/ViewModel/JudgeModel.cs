using System;
using System.Collections.Generic;
using PirateJusticeStatus.Model;

namespace PirateJusticeStatus.ViewModel
{
    public class JudgeModel
    {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public List<SelectOption> TypeOptions { get; set; }
		public string Status { get; set; }
		public List<SelectOption> StatusOptions { get; set; }

        public JudgeModel()
        {
			Id = Guid.NewGuid();
			Name = string.Empty;

			Type = string.Empty;
			TypeOptions = new List<SelectOption>();
			foreach (JudgeType type in Enum.GetValues(typeof(JudgeType)))
            {
				TypeOptions.Add(new SelectOption(type.ToString(), type.Translate().ConvertToHtml(), false));
            }
            
			Status = string.Empty;
			StatusOptions = new List<SelectOption>();
			foreach (JudgeStatus status in Enum.GetValues(typeof(JudgeStatus)))
            {
                StatusOptions.Add(new SelectOption(status.ToString(), status.Translate().ConvertToHtml(), false));
            }
        }

		public JudgeModel(Judge judge)
        {
			Id = judge.Id;
			Name = judge.Name;

			Type = judge.Type.ToString();
			TypeOptions = new List<SelectOption>();
			foreach (JudgeType type in Enum.GetValues(typeof(JudgeType)))
			{
				TypeOptions.Add(new SelectOption(type.ToString(), type.Translate().ConvertToHtml(), type == judge.Type));
			}

			Status = judge.Status.ToString();
			StatusOptions = new List<SelectOption>();
			foreach (JudgeStatus status in Enum.GetValues(typeof(JudgeStatus)))
            {
				StatusOptions.Add(new SelectOption(status.ToString(), status.Translate().ConvertToHtml(), status == judge.Status));
            }
        }

		public void Update(Judge judge)
		{
			judge.Name = Name;
			judge.Type = (JudgeType)Enum.Parse(typeof(JudgeType), Type);
			judge.Status = (JudgeStatus)Enum.Parse(typeof(JudgeStatus), Status);
		}
    }
}
