using Imaginosia.Gameplay;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imaginosia
{
	public static class FindNearest
	{
		public delegate bool TargetingRule(Entity entity);

		public static Entity GetClosestEntity(List<Entity> entities, Vector2 center, float range, TargetingRule rule = null)
		{
			Entity targetEntity = null;
			float distToTarget = range;
			//IEnumerable<Enemy> nearbyEntities =
			//	from entity in entities
			//	where Vector2.Distance(entity.GetFoot(), center) <= range && (rule == null || rule(entity))
			//	select entity;
			//List<NPC> 
			foreach (Entity entity in entities)
			{
				if ((Vector2.Distance(entity.GetFoot(), center) <= range || range < 0) && (rule == null || rule(entity)))
				{
					float testDist = Vector2.Distance(entity.GetFoot(), center);
					if (testDist <= distToTarget)
					{
						targetEntity = entity;
						distToTarget = testDist;
					}
				}
			}
			return targetEntity;
		}
	}
}
