using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SerializeTime {
	public float hours;
	public float minutes;
	public float seconds;

	public SerializeTime(float seconds) : this(0, seconds) { }
	public SerializeTime(float minutes, float seconds) : this(0, minutes, seconds) { }
	public SerializeTime(float hours, float minutes, float seconds) {
		this.hours = hours;
		this.minutes = minutes;
		this.seconds = seconds;
	}

	public float InSeconds() {
		return ((hours * 60 + minutes) * 60) + seconds;
	}

	public SerializeTime FromSeconds(float sec) {
		seconds = sec;
		minutes = (int)seconds / 60;
		hours = (int)minutes / 60;
		minutes %= 60;
		seconds %= 60;
		return this;
	}

	public void Normalize() {
		FromSeconds(InSeconds());
	}

	public static implicit operator float(SerializeTime st) {
		return st.InSeconds();
	}
	public static implicit operator SerializeTime(float f) {
		return new SerializeTime().FromSeconds(f);
	}
}

public interface IWeighted {
	int Weight { get; }
}

public static class Utilities {

	public static V GetOrInsertDefault<K,V>(this Dictionary<K,V> dict, K key, Func<V> def) {
		V value;
		if (!dict.TryGetValue(key, out value)) {
			value = def();
			dict[key] = value;
		}
		return value;
	}

	public static V WeightedChoice<V>(this ICollection<V> list, int totalWeight) where V : IWeighted {
		int randomNumber = UnityEngine.Random.Range(0, totalWeight);
		
		foreach (V inst in list) {
			if (randomNumber < inst.Weight) {
				return inst;
			}
			randomNumber -= inst.Weight;
		}
		return default(V);
	}

	public static bool Contains(this LayerMask mask, int layer) {
		return mask == (mask | (1 << layer));
	}

	public static List<T>[] ArrayOfLists<T>(int length) {
		List<T>[] aol = new List<T>[length];
		for (int i = 0; i < length; i++) {
			aol[i] = new List<T>();
		}
		return aol;
	}
}
