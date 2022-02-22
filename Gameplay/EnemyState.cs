using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public enum EnemyState
	{
		Sleep, // The enemy will not move
		Wander, // The enemy will wander and search for an interest
		Sneak, // The enemy has found an interest and is lurking about it
		Attack, // The enemy has found an interest and is pursuing it
		Flee // The enemy is afraid and running away
	}
}
