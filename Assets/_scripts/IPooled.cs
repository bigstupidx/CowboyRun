using System;

public interface IPooled
{
	// when the item get form the pool, call this...
	void OnGetOut();

	// when the item pull into the pool, call this...
	void OnPullInto();
}

