﻿using System.Collections.Generic;
using SIL.Collections;
using SIL.Machine.Matching;

namespace SIL.Machine.Rules
{
	public class MultiplePatternRule<TData, TOffset> : PatternRule<TData, TOffset> where TData : IData<TOffset>, IDeepCloneable<TData>
	{
		public MultiplePatternRule(SpanFactory<TOffset> spanFactory, IPatternRuleSpec<TData, TOffset> ruleSpec)
			: base(spanFactory, ruleSpec)
		{
		}

		public MultiplePatternRule(SpanFactory<TOffset> spanFactory, IPatternRuleSpec<TData, TOffset> ruleSpec, MatcherSettings<TOffset> matcherSettings)
			: base(spanFactory, ruleSpec, matcherSettings)
		{
		}

		protected override IEnumerable<TData> ApplyImpl(TData input, TOffset start)
		{
			var results = new List<TData>();
			foreach (Match<TData, TOffset> match in Matcher.AllMatches(input, start))
			{
				TData outputData;
				RuleSpec.ApplyRhs(this, match, out outputData);
				results.Add(outputData);
			}
			return results;
		}
	}
}
