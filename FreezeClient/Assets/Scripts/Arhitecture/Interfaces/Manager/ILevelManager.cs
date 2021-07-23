using System;

namespace Inconspicuous.Framework {
	public interface ILevelManager {
		IObservable<IContextView> Load(string name);
	}
}
