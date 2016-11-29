using System;
using System.Collections.Generic;

namespace sa_score_calc
{
	public class UtilityAnswer
	{
		private Dictionary<string, Answer> _answers = new Dictionary<string, Answer>();

		/// <summary>
		/// Answers to 5W1H questions.
		/// </summary>
		/// <value>The answers.</value>
		public Dictionary<string, Answer> Answers
		{
			get {
				return _answers;
			}
		}

		/// <summary>
		/// Is report correct?
		/// </summary>
		/// <value><c>true</c> if this report is correct; otherwise, <c>false</c>.</value>
		public bool IsCorrect { get; set; } = true;

		/// <summary>
		/// Utility's name
		/// </summary>
		/// <value>The name of the utility.</value>
		public string UtilityName { get; set; }

		/// <summary>
		/// Parses the name of the utility and its correctness from "App (correct):" format.
		/// </summary>
		/// <param name="input">Input string</param>
		public void ParseUtilityName(string input)
		{
			UtilityName = input;
			if (UtilityName.Contains ("(")) {
				UtilityName = UtilityName.Substring (0, UtilityName.LastIndexOf ("(")).Trim ();
				IsCorrect = !input.Contains ("(incorrect");
			}
			UtilityName = UtilityName.Replace (":", "");
		}

		/// <summary>
		/// Calculates weighted final score for this utility.
		/// </summary>
		/// <param name="scores">Scores.</param>
		public double Calculate(ref Dictionary<string, double> weights)
		{
			double score = 0.0;
			if (IsCorrect == false)
				return 0.0;
			foreach (var answer in _answers) {
				score += answer.Value.Calculate (ref weights);
			}
			return score;
		}
	}

}

