using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class EnemyManager
    {
        List<Enemy> Enemies;

        public void Add(Enemy NewEnemy)
        {
            Enemies.Add(NewEnemy);
        }

        public void Update()
        {
            foreach(Enemy E in Enemies)
                E.Update();
        }

        public void Remove(int EnemyId)
        {
            Enemies.RemoveAt(EnemyId);
        }
    }
}
