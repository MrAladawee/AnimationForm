using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture_1
{
    internal class Circle
    {

        private Random random = new Random();

        private int diam; // диаметр шара
        private int x, y; // лев.верх точка описанного прямоугольника
        public Color Color { get; set; } // цвет шарика (свойство)

        public int X { get => x; } // только чтение
        public int Y { get => y; } // только чтение
        public int Diam { get => diam; } // только чтение

        public Circle(int diam, int x, int y, Color color)
        {
            this.diam = diam;
            this.x = x;
            this.y = y;
            this.Color = color;
        }

        public Circle(int diam, int x, int y)
        {
            this.diam = diam;
            this.x = x;
            this.y = y;
            this.Color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
        }

        // Метод для сдвига шарика на величину dx,dy
        public void Move(int dx, int dy)
        {
            x += dx;
            y += dy;
        }

        // Отрисовка
        public void Paint(Graphics g)
        {
            Brush brush = new SolidBrush(Color);
            g.FillEllipse(brush, X, Y, Diam, Diam);
        }

    }
}
