using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoodleJump.Classes
{ /*Реализация физики игры: прыжок и контакт с платформой*/
    public class Physics
    {
        public Transform transform; /*хранит позицию и размер */
        float gravity; /*гравитация, при помощи которой будем реализовывать прыжок*/
        float a;

        public float dx; /*двжение персонажем влево, вправо*/
        bool usedBonus = false;

        public Physics(PointF position, Size size) /*конструктор, который хранит в себе позицию и размер*/
        {
            transform = new Transform(position, size);
            gravity = 0;
            a = 0.4f; /* ускорение */
            dx = 0; /*движение влево и вправо*/
        }

        public void ApplyPhysics() /* функция вызова расчета физики*/
        {
            CalculatePhysics(); 
        }

        public void CalculatePhysics() /*передвижение: влево, вправо, прыжок*/
        {
            if (dx != 0)
            {
                transform.position.X += dx; /*двигаем персонажа*/
            }
            if(transform.position.Y < 700)
            {
                transform.position.Y += gravity; /*увеличивыем позицию по y*/
                gravity += a;

                if (gravity > -25 && usedBonus)
                {
                    PlatformController.GenerateRandomPlatform();
                    PlatformController.startPlatformPosY = -200;
                    PlatformController.GenerateStartSequence();
                    PlatformController.startPlatformPosY = 0;
                    usedBonus = false;
                }

                Collide();
            }
        }

        public bool StandartCollidePlayerWithObjects(bool forMonsters,bool forBonuses)
        {
            if (forMonsters)
            {
                for (int i = 0; i < PlatformController.enemies.Count; i++)
                {
                    var enemy = PlatformController.enemies[i];
                    PointF delta = new PointF();
                    delta.X = (transform.position.X + transform.size.Width / 2) - (enemy.physics.transform.position.X + enemy.physics.transform.size.Width / 2);
                    delta.Y = (transform.position.Y + transform.size.Height / 2) - (enemy.physics.transform.position.Y + enemy.physics.transform.size.Height / 2);
                    if (Math.Abs(delta.X) <= transform.size.Width / 2 + enemy.physics.transform.size.Width / 2)
                    {
                        if (Math.Abs(delta.Y) <= transform.size.Height / 2 + enemy.physics.transform.size.Height / 2)
                        {
                            if (!usedBonus)
                                return true;
                        }
                    }
                }
            }
            if (forBonuses)
            {
                for (int i = 0; i < PlatformController.bonuses.Count; i++)
                {
                    var bonus = PlatformController.bonuses[i];
                    PointF delta = new PointF();
                    delta.X = (transform.position.X + transform.size.Width / 2) - (bonus.physics.transform.position.X + bonus.physics.transform.size.Width / 2);
                    delta.Y = (transform.position.Y + transform.size.Height / 2) - (bonus.physics.transform.position.Y + bonus.physics.transform.size.Height / 2);
                    if (Math.Abs(delta.X) <= transform.size.Width / 2 + bonus.physics.transform.size.Width / 2)
                    {
                        if (Math.Abs(delta.Y) <= transform.size.Height / 2 + bonus.physics.transform.size.Height / 2)
                        {
                            if (bonus.type == 1 && !usedBonus)
                            {
                                usedBonus = true;
                                AddForce(-30);
                            }
                            if (bonus.type == 2 && !usedBonus)
                            {
                                usedBonus = true;
                                AddForce(-60);
                            }

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool StandartCollide()
        {
            for(int i = 0; i < PlatformController.bullets.Count; i++)
            {
                var bullet = PlatformController.bullets[i];
                PointF delta = new PointF();
                delta.X = (transform.position.X + transform.size.Width / 2) - (bullet.physics.transform.position.X + bullet.physics.transform.size.Width / 2);
                delta.Y = (transform.position.Y + transform.size.Height / 2) - (bullet.physics.transform.position.Y + bullet.physics.transform.size.Height / 2);
                if (Math.Abs(delta.X) <= transform.size.Width / 2 + bullet.physics.transform.size.Width / 2)
                {
                    if (Math.Abs(delta.Y) <= transform.size.Height / 2 + bullet.physics.transform.size.Height / 2)
                    {
                        PlatformController.RemoveBullet(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Collide() /*пробегаемся по всем платформам, которые есть на карте*/
        {
            for(int i = 0; i < PlatformController.platforms.Count; i++)
            {
                var platform = PlatformController.platforms[i];
                if(transform.position.X+transform.size.Width/2 >= platform.transform.position.X && transform.position.X + transform.size.Width/2 <= platform.transform.position.X + platform.transform.size.Width) /*если середина игрока по X находится в пределах платформы от X до X + size*/
                {
                    if(transform.position.Y+transform.size.Height >= platform.transform.position.Y && transform.position.Y + transform.size.Height <= platform.transform.position.Y + platform.transform.size.Height) /*если середина игрока по Y находится в пределах платформы от Y до Y + size*/
                    {
                        if (gravity > 0) /**/
                        {
                            AddForce();
                            if (!platform.isTouchedByPlayer)
                            {
                                PlatformController.score += 20; /*прибавление очков игроку за прикосновение до платформы*/
                                PlatformController.GenerateRandomPlatform();
                                platform.isTouchedByPlayer = true;
                            }
                        }
                    }
                }
            }
        }

        public void AddForce(int force = -10) /*прыжок персонажа */
        {
            gravity = force;
        }
    }
}
