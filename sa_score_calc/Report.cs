using System;
using System.Collections.Generic;
using System.Linq;

namespace sa_score_calc
{
	public class Report
	{
		private List<UtilityAnswer> _utilityAnswers = new List<UtilityAnswer>();

		/// <summary>
		/// Inner name of the report.
		/// </summary>
		/// <value>Inner name</value>
		public string InnerName { get; set; }

		/// <summary>
		/// Error class of the report (usually Inner Name of another report)
		/// </summary>
		/// <value>The error class.</value>
		public string ErrorClass { get; set; }

		/// <summary>
		/// Is report supported by all utilities?
		/// </summary>
		/// <value><c>true</c> if supported; otherwise, <c>false</c>.</value>
		public bool Supported { get; set; }


		/// <summary>
		/// Gets the answers of utilities
		/// </summary>
		/// <value>The utility answers.</value>
		public List<UtilityAnswer> UtilityAnswers 
		{
			get
            {
				return _utilityAnswers;
			}
		}
            
        /// <summary>
        /// Parses the inner name, error class and supported flat from "report (another report) {notfullsupport}" format.
        /// </summary>
        /// <param name="input">Input string</param>
		public void ParseInnerName(string input)
		{
			InnerName = input;
			Supported = true;
			if (InnerName.Contains ("{")) {
				InnerName = InnerName.Substring (0, InnerName.IndexOf ("}"));
				if (input.Substring (input.IndexOf ("{"), input.IndexOf ("}") - input.IndexOf ("{") + 1) == "{notfullsupport}") {
					Supported = false;
				}
			}

			if (InnerName.Contains ("(")) {
				InnerName = InnerName.Substring (0, InnerName.LastIndexOf ("(")).Trim ();
				ErrorClass = input.Substring (input.IndexOf ("(") + 1);
				if (ErrorClass.Contains (")")) {
					ErrorClass = ErrorClass.Substring (0, ErrorClass.LastIndexOf (")")).Trim ();
				}
			}
		}

        /// <summary>
        /// Calculate utility-sorted score list.
        /// </summary>
        /// <param name="weights">Weight dictionary</param>
		public Dictionary<string, double> Calculate(ref Dictionary<string, double> weights)
		{
			var dict = new Dictionary<string, List<double>> ();
			foreach (var utilAnswer in _utilityAnswers) {
				if (!dict.ContainsKey (utilAnswer.UtilityName)) {
					dict.Add (utilAnswer.UtilityName, new List<double>());
				}
				var utilScoreList = dict [utilAnswer.UtilityName];
				utilScoreList.Add(utilAnswer.Calculate(ref weights));
			}

            // Find average score per each utility
			var resultDict = new Dictionary<string, double> ();
			foreach (var d in dict) {
				resultDict.Add (d.Key, d.Value.ToList ().Average ());
			}
			return resultDict;
		}
	}
}

