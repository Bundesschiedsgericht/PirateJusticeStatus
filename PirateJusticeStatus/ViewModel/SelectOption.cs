using System;
namespace PirateJusticeStatus.ViewModel
{
    public class SelectOption
    {
		public string Value { get; set; }
		public string Text { get; set; }
		public bool Selected { get; set; }

		public SelectOption()
        {
        }

        public SelectOption(string value, string text, bool selected)
        {
			Value = value;
			Text = text;
			Selected = selected;
        }
    }
}
