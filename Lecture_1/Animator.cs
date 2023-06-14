using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture_1
{
    internal class Animator
    {
        // Измненеие объектов шариков (будем двигать)
        // Отвечает за 1 шарик, который двигается в отдельном потоке

        // Нужно иметь в классе Аниматор сам шарик, для его сдвига
        private Circle c;
        private Thread? t = null;

        // Если t запущен и закончил работу => IsAlive = false
        // Если t не запустили поток / работает => IsAlive = true
        public bool IsAlive => t == null || t.IsAlive;

        public Size ContainerSize { get; set; }

        public Animator(Size containerSize)
        {
            c = new Circle(50, 0, 0); // начинают свой ход с 0,0
            ContainerSize = containerSize;
        }

        // Запускает наш аниматор
        public void Start ()
        {

            // Поток задется Лямба функцией с описанием метода, что будет выполняться
            t = new Thread( () =>
            {
               // Выполняем до края
               while (c.X + c.Diam < ContainerSize.Width)
               {
                   Thread.Sleep(30);
                   c.Move(1, 0);
               }
            });

            t.IsBackground = true;
            t.Start();
        }

        // Отрисовка шара
        // По факту дает связь с Paint из Circle для Painter
        // "мост"
        public void PaintCircle(Graphics g)
        {
            c.Paint(g);
        }


    }
}
