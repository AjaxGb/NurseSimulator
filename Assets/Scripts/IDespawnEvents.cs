using System;

public interface IDespawnEvents {
	void AddDespawnAction(Action a);
	bool RemoveDespawnAction(Action a);
}
