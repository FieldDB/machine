﻿using SIL.Collections;

namespace SIL.Machine.Matching.Fluent
{
	public interface IQuantifierGroupSyntax<TData, TOffset> : IAlternationGroupSyntax<TData, TOffset> where TData : IData<TOffset>, IDeepCloneable<TData>
	{
		IAlternationGroupSyntax<TData, TOffset> ZeroOrMore { get; }
		IAlternationGroupSyntax<TData, TOffset> LazyZeroOrMore { get; }

		IAlternationGroupSyntax<TData, TOffset> OneOrMore { get; }
		IAlternationGroupSyntax<TData, TOffset> LazyOneOrMore { get; }

		IAlternationGroupSyntax<TData, TOffset> Optional { get; }
		IAlternationGroupSyntax<TData, TOffset> LazyOptional { get; }

		IAlternationGroupSyntax<TData, TOffset> Range(int min, int max);
		IAlternationGroupSyntax<TData, TOffset> LazyRange(int min, int max);
	}
}
