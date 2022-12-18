using DoodleJump.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace DoodleJump
{
    public partial class Form1 : Form
    {
        public Label labelscore;
        public int score = 0;
        public Stopwatch tim = new Stopwatch();
        public bool GameOver = false;
        public PictureBox Pause;
        public string PlayerName;
        public Save save = new Save();
        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Save));
        private int timelf1 = 0;

        Player player; /*переменная игрок и таймер*/
        Timer timer1;
        public Form1()
        {
            InitializeComponent(); 
            Init();
            timer1 = new Timer(); /*создание таймера*/
            timer1.Interval = 15 /*интервал*/
            timer1.Tick += new EventHandler(Update); 
            timer1.Start();
            this.KeyDown += new KeyEventHandler(OnKeyboardPressed); /*обработчик коннекта*/
            this.KeyUp += new KeyEventHandler(OnKeyboardUp);
            this.BackgroundImage = Properties.Resources.back;
            this.Height = 600;
            this.Width = 600;
            this.Paint += new PaintEventHandler(OnRepaint);
            labelscore = new Label();
            Pause = new PictureBox();
            Pause.Size = new Size(80, 80);
            Pause.SizeMode = PictureBoxSizeMode.StretchImage;
            Pause.Image = Properties.Resources.Pause;
            Pause.Location = new Point(800 + 5, 20);
            Pause.Click += Pause_Click;
            Pause.MouseEnter += Button_MouseEnter;
            Pause.MouseLeave += Button_MouseLeave;
            this.Controls.Add(Pause);
        }

        public void Init() /* функция, где будет происходить иницализация всех классов*/
        {
            PlatformController.platforms = new System.Collections.Generic.List<Platform>(); /*лист, который содержит стартовую платформу под персонажем*/
            PlatformController.AddPlatform(new System.Drawing.PointF(100, 400));
            PlatformController.startPlatformPosY = 400;
            PlatformController.score = 0;
            PlatformController.GenerateStartSequence();
            PlatformController.bullets.Clear();
            PlatformController.bonuses.Clear();
            PlatformController.enemies.Clear();
            player = new Player();
        }

        private void OnKeyboardUp(object sender,KeyEventArgs e) /*новый экземпляр класса Player*/
        {
            player.physics.dx = 0; /*обработчик для остановки движения по d и x*/
            player.sprite = Properties.Resources.man2;
            switch (e.KeyCode.ToString())
            {
                case "Space":
                    PlatformController.CreateBullet(new PointF(player.physics.transform.position.X + player.physics.transform.size.Width / 2, player.physics.transform.position.Y));
                    break;
            }
        }

        private void OnKeyboardPressed(object sender,KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Right":
                    player.physics.dx = 6;
                    break;
                case "Left":
                    player.physics.dx = -6;
                    break;
                case "Space":
                    player.sprite = Properties.Resources.man_shooting;
                    //PlatformController.CreateBullet(new PointF(player.physics.transform.position.X + player.physics.transform.size.Width/2,player.physics.transform.position.Y));
                    break;
            }
        }


        private void Update(object sender,EventArgs e) /*расчет физики и функций*/
        {
            if (!GameOver)
            {
                TimeSpan timeSpan = tim.Elapsed;
                timelf1++;
                labelscore.Text = "Время: " + (timeSpan.Seconds + timeSpan.Minutes * 60) + "\n" + "Баллы: " + score + "\n" + "Максимальная длина: " + (maxlenght + 1);
                this.Text = "Doodle Jump: Score - " + PlatformController.score;

            if ( (player.physics.transform.position.Y >= PlatformController.platforms[0].transform.position.Y + 200) || player.physics.StandartCollidePlayerWithObjects(true,false))
                Init();

            player.physics.StandartCollidePlayerWithObjects(false, true);

            if (PlatformController.bullets.Count > 0)
            {
                for (int i = 0; i < PlatformController.bullets.Count; i++)
                {
                    if (Math.Abs(PlatformController.bullets[i].physics.transform.position.Y - player.physics.transform.position.Y) > 500)
                    {
                        PlatformController.RemoveBullet(i);
                        continue;
                    }
                    PlatformController.bullets[i].MoveUp();
                }
            }
            if (PlatformController.enemies.Count > 0)
            {
                for (int i = 0; i < PlatformController.enemies.Count; i++)
                {
                    if (PlatformController.enemies[i].physics.StandartCollide())
                    {
                        PlatformController.RemoveEnemy(i);
                        break;
                    }
                }
            }

            player.physics.ApplyPhysics();
            FollowPlayer();

            Invalidate();
        }

        public void FollowPlayer() /*функция, которая смещает камеру игрока и платформы после прохождения некоторого пути*/
        {
            int offset = 400 - (int)player.physics.transform.position.Y;
            player.physics.transform.position.Y += offset;
            for(int i = 0; i < PlatformController.platforms.Count; i++)
            {
                var platform = PlatformController.platforms[i];
                platform.transform.position.Y += offset;
            }
            for (int i = 0; i < PlatformController.bullets.Count; i++)
            {
                var bullet = PlatformController.bullets[i];
                bullet.physics.transform.position.Y += offset;
            }
            for (int i = 0; i < PlatformController.enemies.Count; i++)
            {
                var enemy = PlatformController.enemies[i];
                enemy.physics.transform.position.Y += offset;
            }
            for (int i = 0; i < PlatformController.bonuses.Count; i++)
            {
                var bonus = PlatformController.bonuses[i];
                bonus.physics.transform.position.Y += offset;
            }
        }

        private void OnRepaint(object sender, PaintEventArgs e) /*обработчик событий перерисовки*/
        {
            Graphics g = e.Graphics; /*получаем элемент графики*/
            if (PlatformController.platforms.Count > 0) /*есть ли в списке платформа. Если да, то начинается отрисовка*/
            {
                for (int i = 0; i < PlatformController.platforms.Count; i++)
                    PlatformController.platforms[i].DrawSprite(g); /*передача графики к Drawsprite*/
            }
            if (PlatformController.bullets.Count > 0)
            {
                for (int i = 0; i < PlatformController.bullets.Count; i++)
                    PlatformController.bullets[i].DrawSprite(g);
            }
            if (PlatformController.enemies.Count > 0)
            {
                for (int i = 0; i < PlatformController.enemies.Count; i++)
                    PlatformController.enemies[i].DrawSprite(g);
            }
            if (PlatformController.bonuses.Count > 0)
            {
                for (int i = 0; i < PlatformController.bonuses.Count; i++)
                    PlatformController.bonuses[i].DrawSprite(g);
            }
            player.DrawSprite(g);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
