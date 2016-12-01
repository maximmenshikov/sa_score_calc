using System;
using System.Collections.Generic;
using System.Linq;

namespace sa_score_calc
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine ("No file specified");
				return;
			}

			if (System.IO.File.Exists (args [0]) == false)
			{
				Console.WriteLine("File not found");
				return;
			}

			Dictionary<string, double> weights = new Dictionary<string, double> () {
				{"What", 0.2},
				{"When", 0.15},
				{"Where", 0.1},
				{"Who", 0.05},
				{"Why", 0.2},
				{"How to fix", 0.3}
			};

			var testedUtilities = new List<string> ();

			Report report = null;
			UtilityAnswer utilAnswer = null;
			Answer answer = null;
			var reports = new List<Report> ();

			// Read full report line by line and construct list of reports.
			var sr = System.IO.File.OpenText (args [0]);
			string s;
			while ((s = sr.ReadLine()) != null) {
				var st = s.Trim ();
				if (String.IsNullOrEmpty (st) == true)
					continue;
				if (st.StartsWith ("-----")) {
					report = null;
					continue;
				}

				if (report == null) {
					report = new Report ();
					report.ParseInnerName (st);
					reports.Add (report);
					continue;
				}

				if (st.Contains ("(correct") || st.Contains ("(incorrect)") || st.EndsWith(":")) {	
					utilAnswer = new UtilityAnswer ();
					utilAnswer.ParseUtilityName (st);
					if (!testedUtilities.Contains (utilAnswer.UtilityName))
						testedUtilities.Add (utilAnswer.UtilityName);
					report.UtilityAnswers.Add (utilAnswer);
					continue;
				}

				if (st.Contains ("?")) {
					answer = new Answer ();
					answer.Parse (st);
					utilAnswer.Answers.Add (answer.Name, answer);
				}
			}
			sr.Close();

			// Collect scores for each error class.
			var errorClassUtilityScores = new Dictionary<string, Dictionary<string, List<double>>> ();
			foreach (var _report in reports)
			{
				if (!_report.Supported)
					continue;
				var result = _report.Calculate (ref weights);
				string ec = _report.InnerName;
				if (String.IsNullOrEmpty (_report.ErrorClass) == false)
					ec = _report.ErrorClass;

				if (!errorClassUtilityScores.ContainsKey (ec)) {
					errorClassUtilityScores.Add (ec, new Dictionary<string, List<Double>>());
					foreach (var util in testedUtilities) {
						errorClassUtilityScores [ec].Add (util, new List<double> ());
					}
				}

				foreach (var util in result) {
					errorClassUtilityScores [ec][util.Key].Add(util.Value);
				}

			}

			// Collect scores for each utility.
			var utilScores = new Dictionary<string, List<double>>();
			foreach (var _ecUtil in errorClassUtilityScores) {
				Console.WriteLine (_ecUtil.Key + ":");
				foreach (var util in _ecUtil.Value) {
					Console.Write (util.Key);
					if (!utilScores.ContainsKey (util.Key))
						utilScores.Add (util.Key, new List<double> ());

					if (util.Value.Count > 0) {
						Console.WriteLine (": " + util.Value.Average ());
						utilScores[util.Key].AddRange(util.Value);
					} else {
						utilScores [util.Key].Add(0);
						Console.WriteLine (": 0");
					}
				}
				Console.WriteLine ();
			}

			Console.WriteLine("----------");

			// Print out scores.
			foreach (var _util in utilScores) {
				Console.Write (_util.Key + ": ");

				if (_util.Value.Count > 0) {
					Console.Write (_util.Value.Average () / utilScores["In fact"].Average() + " ");
					Console.WriteLine (_util.Value.Where((a) => a > 0).Count() +  " / " + utilScores["In fact"].Count() + " ");
				} else {
					Console.WriteLine ("0 0");
				}
			}
		}
	}
}
