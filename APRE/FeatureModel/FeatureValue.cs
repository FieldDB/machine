using System;
using System.Collections.Generic;

namespace SIL.APRE.FeatureModel
{
	public abstract class FeatureValue : ICloneable
	{
		internal FeatureValue Forward { get; set; }

		public abstract FeatureValue Clone();

		object ICloneable.Clone()
		{
			return Clone();
		}

		public abstract bool Negation(out FeatureValue output);

		internal abstract void Merge(FeatureValue other, VariableBindings varBindings);
		internal abstract bool Subtract(FeatureValue other, VariableBindings varBindings);
		internal abstract bool IsDefiniteUnifiable(FeatureValue other, bool useDefaults, VariableBindings varBindings);
		internal abstract bool DestructiveUnify(FeatureValue other, bool useDefaults, bool preserveInput,
			IDictionary<FeatureValue, FeatureValue> copies, VariableBindings varBindings);
		protected abstract bool NondestructiveUnify(FeatureValue other, bool useDefaults, IDictionary<FeatureValue, FeatureValue> copies,
			VariableBindings varBindings, out FeatureValue output);
		internal abstract FeatureValue Clone(IDictionary<FeatureValue, FeatureValue> copies);

		internal bool UnifyDefinite(FeatureValue other, bool useDefaults, VariableBindings varBindings, out FeatureValue output)
		{
			var copies = new Dictionary<FeatureValue, FeatureValue>(new IdentityEqualityComparer<FeatureValue>());
			return UnifyDefinite(other, useDefaults, copies, varBindings, out output);
		}

		internal bool UnifyDefinite(FeatureValue other, bool useDefaults, IDictionary<FeatureValue, FeatureValue> copies,
			VariableBindings varBindings, out FeatureValue output)
		{
			other = Dereference(other);

			FeatureValue fv1;
			if (!copies.TryGetValue(this, out fv1))
				fv1 = null;
			FeatureValue fv2;
			if (!copies.TryGetValue(other, out fv2))
				fv2 = null;

			if (fv1 == null && fv2 == null)
			{
				if (!NondestructiveUnify(other, useDefaults, copies, varBindings, out output))
				{
					output = null;
					return false;
				}
			}
			else if (fv1 != null && fv2 != null)
			{
				fv1.DestructiveUnify(fv2, useDefaults, false, copies, varBindings);
				output = fv1;
			}
			else if (fv1 != null)
			{
				fv1.DestructiveUnify(other, useDefaults, true, copies, varBindings);
				output = fv1;
			}
			else
			{
				fv2.DestructiveUnify(this, useDefaults, true, copies, varBindings);
				output = fv2;
			}

			return true;
		}

		protected static bool Dereference<T>(FeatureValue value, out T actualValue) where T : FeatureValue
		{
			value = Dereference(value);

			if (!(value is T))
			{
				actualValue = null;
				return false;
			}

			actualValue = (T) value;
			return true;
		}

		protected static T Dereference<T>(T value) where T : FeatureValue
		{
			while (value.Forward != null)
				value = (T) value.Forward;
			return value;
		}
	}
}