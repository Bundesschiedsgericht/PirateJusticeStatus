using System;
using PirateJusticeStatus.Model;

namespace PirateJusticeStatus.ViewModel
{
    public static class Translation
    {
		public static string Translate(this JudgeStatus value)
		{
			switch (value)
			{
				case JudgeStatus.Available:
					return "Aktiv";
				case JudgeStatus.OnLeave:
					return "Beurlaubt";
				case JudgeStatus.Resigned:
					return "Zurück- oder Ausgetreten";
				case JudgeStatus.Unavailable:
					return "Inaktiv oder unerreichbar";
				default:
					throw new InvalidOperationException();
			}
		}

		public static string Translate(this JudgeType value)
        {
            switch (value)
            {
				case JudgeType.Full:
                    return "Ordentlicher Richter";
				case JudgeType.Reserve:
                    return "Ersatzrichter";
				case JudgeType.Chair:
                    return "Vorsitzender Richter";
                default:
                    throw new InvalidOperationException();
            }
        }

		public static string Translate(this CaseLoad value)
        {
            switch (value)
            {
				case CaseLoad.None:
                    return "Keine hängigen Verfahren";
				case CaseLoad.One:
					return "Ein hängiges Verfahren";
				case CaseLoad.TwoPlus:
					return "2-4 hängige Verfahren";
				case CaseLoad.FivePlus:
					return "5-9 hängige Verfahren";
				case CaseLoad.TenPlus:
					return "10 oder mehr hängige Verfahren";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
