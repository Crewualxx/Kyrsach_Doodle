using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Classes
{
     public static class PlatformController /*создание новых платформ и очистка пройденных платформ*/
    {
        public static List<Platform> platforms; /*хранение платформ, которые находятся на карте*/
        public static List<Bullet> bullets = new List<Bullet>(); 
        public static List<Enemy> enemies = new List<Enemy>();
        public static List<Bonus> bonuses = new List<Bonus>();
        public static int startPlatformPosY = 400; /*спавн платформ*/
        public static int score = 0;

        public static void AddPlatform(PointF position) /*при помощи нее будет добавляться платформа*/
        {
            Platform platform = new Platform(position);
            platforms.Add(platform);
        }

        public static void CreateBullet(PointF pos) /*функция, которая генерирует пулю*/
        {
            var bullet = new Bullet(pos);
            bullets.Add(bullet); /*добовляется в лист*/
        }

        public static void GenerateStartSequence() /*при старте игры создается выделенное количество платформ*/
        {
            Random r = new Random();
            for(int i = 0; i < 10; i++)
            {
                int x = r.Next(0, 270);
                int y = r.Next(50, 60);
                startPlatformPosY -= y;
                PointF position = new PointF(x, startPlatformPosY); /*смещение платформы на уровень выше*/
                Platform platform = new Platform(position); /*создание платформы с позицией*/
                platforms.Add(platform); /*добовляем платформу в лист*/
            }
        }

        public static void GenerateRandomPlatform() /*добавление платформы в любое место*/
        {
            ClearPlatforms(); 
            Random r = new Random(); 
            int x = r.Next(0, 270);
            PointF position = new PointF(x, startPlatformPosY);
            Platform platform = new Platform(position);
            platforms.Add(platform);

            var c = r.Next(1, 3);

            switch (c)
            {
                case 1:
                    c = r.Next(1, 10);
                    if (c == 1)
                    {
                        CreateEnemy(platform);
                    }
                    break;
                case 2:
                    c = r.Next(1, 10);
                    if (c == 1)
                    {
                        CreateBonus(platform);

                    }
                    break;
            }

           

            
        }

        public static void CreateBonus(Platform platform)
        {
            Random r = new Random();
            var bonusType = r.Next(1,3);

            switch (bonusType)
            {
                case 1:
                    var bonus = new Bonus(new PointF(platform.transform.position.X + (platform.sizeX / 2) - 7, platform.transform.position.Y - 15), bonusType);
                    bonuses.Add(bonus);
                    break;
                case 2:
                    bonus = new Bonus(new PointF(platform.transform.position.X + (platform.sizeX / 2) - 15, platform.transform.position.Y - 30), bonusType);
                    bonuses.Add(bonus);
                    break;
            }
        }

        public static void CreateEnemy(Platform platform) /*функция проирсовки в определенных координатах монтров*/
        {
            Random r = new Random();
            var enemyType = r.Next(1, 4);

            switch (enemyType)
            {
                case 1:
                    var enemy = new Enemy(new PointF(platform.transform.position.X + (platform.sizeX / 2) - 20, platform.transform.position.Y - 40),enemyType);
                    enemies.Add(enemy);
                    break;
                case 2:
                    enemy = new Enemy(new PointF(platform.transform.position.X + (platform.sizeX / 2) - 35, platform.transform.position.Y - 50), enemyType);
                    enemies.Add(enemy);
                    break;
                case 3:
                    enemy = new Enemy(new PointF(platform.transform.position.X + (platform.sizeX / 2) -35, platform.transform.position.Y - 60), enemyType);
                    enemies.Add(enemy);
                    break;

            }

            
            
        }

        public static void RemoveEnemy(int i)/*функция для удаления монстров*/
        {
            enemies.RemoveAt(i);
        }

        public static void RemoveBullet(int i) /*функция для удаления пуль*/
        {
            bullets.RemoveAt(i);
        }

        public static void ClearPlatforms() /*очистка пройденных платформ, которые находятся вдали от нашего персонажа*/
        {
            for(int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i].transform.position.Y >= 700) /*если позиция по Y больше или ровно семисот, то убираем ее*/
                    platforms.RemoveAt(i);
            }
            for (int i = 0; i < bonuses.Count; i++)
            {
                if (bonuses[i].physics.transform.position.Y >= 700)
                    bonuses.RemoveAt(i);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].physics.transform.position.Y >= 700)
                    enemies.RemoveAt(i);
            }
        }
    }
}
