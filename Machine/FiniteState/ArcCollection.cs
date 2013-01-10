﻿using System;
using System.Collections;
using System.Collections.Generic;
using SIL.Collections;
using SIL.Machine.FeatureModel;

namespace SIL.Machine.FiniteState
{
	public class ArcCollection<TData, TOffset> : ICollection<Arc<TData, TOffset>> where TData : IData<TOffset>
	{
		private readonly State<TData, TOffset> _state;
		private readonly List<Arc<TData, TOffset>> _arcs;
		private readonly IComparer<Arc<TData, TOffset>> _arcComparer;
		private readonly IFstOperations<TData, TOffset> _operations; 

		public ArcCollection(IFstOperations<TData, TOffset> operations, State<TData, TOffset> state)
		{
			_state = state;
			_operations = operations;
			_arcs = new List<Arc<TData, TOffset>>();
			_arcComparer = ProjectionComparer<Arc<TData, TOffset>>.Create(arc => arc.PriorityType).Reverse();
		}

		IEnumerator<Arc<TData, TOffset>> IEnumerable<Arc<TData, TOffset>>.GetEnumerator()
		{
			return _arcs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _arcs.GetEnumerator();
		}

		public State<TData, TOffset> Add(State<TData, TOffset> target)
		{
			return Add(target, ArcPriorityType.Medium);
		}

		public State<TData, TOffset> Add(State<TData, TOffset> target, ArcPriorityType priorityType)
		{
			return AddInternal(new Arc<TData, TOffset>(_state, target, priorityType));
		}

		public State<TData, TOffset> Add(FeatureStruct input, State<TData, TOffset> target)
		{
			if (!input.IsFrozen)
				throw new ArgumentException("The input must be immutable.", "input");
			return AddInternal(new Arc<TData, TOffset>(_state, new Input(input, 1), new PriorityUnionOutput<TData, TOffset>(FeatureStruct.New().Value).ToEnumerable(), target));
		}

		public State<TData, TOffset> Add(FeatureStruct input, FeatureStruct output, State<TData, TOffset> target)
		{
			return Add(input, output, false, target);
		}

		public State<TData, TOffset> Add(FeatureStruct input, FeatureStruct output, bool replace, State<TData, TOffset> target)
		{
			if (_operations == null)
				throw new InvalidOperationException("Outputs are not valid on acceptors.");

			if (input != null && !input.IsFrozen)
				throw new ArgumentException("The input must be immutable.", "input");
			if (output != null && !output.IsFrozen)
				throw new ArgumentException("The output must be immutable.", "output");

			Output<TData, TOffset> outputAction;
			if (input == null)
				outputAction = new InsertOutput<TData, TOffset>(output, _operations.Insert);
			else if (output == null)
				outputAction = new RemoveOutput<TData, TOffset>(_operations.Remove);
			else if (replace)
				outputAction = new ReplaceOutput<TData, TOffset>(output, _operations.Replace);
			else
				outputAction = new PriorityUnionOutput<TData, TOffset>(output, _operations.Replace);

			return AddInternal(new Arc<TData, TOffset>(_state, new Input(input, 1), outputAction.ToEnumerable(), target));
		}

		internal State<TData, TOffset> Add(Input input, IEnumerable<Output<TData, TOffset>> output, State<TData, TOffset> target)
		{
			return AddInternal(new Arc<TData, TOffset>(_state, input, output, target));
		}

		internal State<TData, TOffset> Add(State<TData, TOffset> target, int tag)
		{
			return AddInternal(new Arc<TData, TOffset>(_state, target, tag));
		}

		internal State<TData, TOffset> Add(Input input, IEnumerable<Output<TData, TOffset>> outputs, State<TData, TOffset> target, IEnumerable<TagMapCommand> cmds)
		{
			return AddInternal(new Arc<TData, TOffset>(_state, input, outputs, target, cmds));
		}

		void ICollection<Arc<TData, TOffset>>.Add(Arc<TData, TOffset> arc)
		{
			AddInternal(arc);
		}

		private State<TData, TOffset> AddInternal(Arc<TData, TOffset> arc)
		{
			int index = _arcs.BinarySearch(arc, _arcComparer);
			if (index < 0)
				index = ~index;
			_arcs.Insert(index, arc);
			return arc.Target;
		}

		public void Clear()
		{
			_arcs.Clear();
		}

		public bool Contains(Arc<TData, TOffset> item)
		{
			return _arcs.Contains(item);
		}

		public void CopyTo(Arc<TData, TOffset>[] array, int arrayIndex)
		{
			_arcs.CopyTo(array, arrayIndex);
		}

		public bool Remove(Arc<TData, TOffset> item)
		{
			return _arcs.Remove(item);
		}

		public int Count
		{
			get { return _arcs.Count; }
		}

		bool ICollection<Arc<TData, TOffset>>.IsReadOnly
		{
			get { return false; }
		}

		public int IndexOf(Arc<TData, TOffset> item)
		{
			return _arcs.IndexOf(item);
		}

		public Arc<TData, TOffset> this[int index]
		{
			get { return _arcs[index]; }
		}
	}
}