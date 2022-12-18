using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Classes
{
    public class Platform
    {
        /* класс, содержащий платформы, по которым будем передвигаться игрок */



        Image sprite; /*отрисовка картинки*/
        public Transform transform; /*хранение размера и позиции*/
        public int sizeX; /*две переменные для того, чтобы задать размер платформам*/
        public int sizeY;
        public bool isTouchedByPlayer; /*проверка на касание платформы пользователем*/

        public Platform(PointF pos)
        {
            sprite = Properties.Resources.platform; /*подгрузка ресурсов*/
            sizeX = 80; /*размер платформы по x*/
            sizeY = 10; /*размер по y*/
            transform = new Transform(pos, new Size(sizeX, sizeY)); /*создание нового экземпляра, которому передаем параметры размеров*/
            isTouchedByPlayer = false;
        }

        public void DrawSprite(Graphics g) /*отрисовка платформы */
        {
            /* метод, куда передаем картинку, позицию и ее размеры */
            g.DrawImage(sprite, transform.position.X, transform.position.Y, transform.size.Width, transform.size.Height);
        }

    }
}
