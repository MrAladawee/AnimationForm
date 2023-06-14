using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture_1
{
    internal class Painter
    {
        // Отвечает за отрисовку в своем потоке

        private object locker = new object(); // объект локер

        private List<Animator> animators = new(); // Список аниматоров
                                          // Связь с шариками, которые двигаются
                                          // Которые будем отрисовывать
        
        private Size containerSize;
        private Thread t;

        private Graphics mainGraphics; // основной графикс, где рисуем
        private BufferedGraphics bg;
        private bool isAlive;
        private int objectsPainted = 0;


        public Graphics MainGraphics
        {
            get => mainGraphics;
            set
            {
                // Исходя из метода Start.
                // Не только в месте употребления, но и к общим данным в месте употребления
                lock (locker)
                {
                    // В случае изменения мэйнПанель поменяются: контейнер сайз и буфферизованный график относительно нового мэйнПанель (то есть выполняется не 1 раз в конструкторе)
                    // Изменение панели влечет изменение тут (менняться будет сразу все нужное)
                    mainGraphics = value;

                    ContainerSize = mainGraphics.VisibleClipBounds.Size.ToSize(); // узнаем размер контейнера
                                                                                  // VisibleClipBounds - свойство дает возможность видеть размеры
                                                                                  // Получаем Size и преобразуем в целочисленный размер (могут быть float, его тип SizeF)

                    bg = BufferedGraphicsManager.Current.Allocate(mainGraphics, new Rectangle(new Point(0, 0), ContainerSize)); // (куда выводим из буффера изображение,
                                                                                                                                // прямоугольная область, определяющий размеры и позицию
                                                                                                                                // буфферизованного изображения)
                    objectsPainted = 0;
                }
            }
        }


        public Size ContainerSize {
            
            get => containerSize;
            
            set
            {
                containerSize = value;
                // Во время изменения ContainerSize мы меняем и у всех аниматоров
                foreach (Animator animator in animators)
                {
                    animator.ContainerSize = ContainerSize;
                }
            }
        }
        
        public Painter(Graphics mainGraphics)
        {
            MainGraphics = mainGraphics;
        }

        // Добавляет нового аниматора
        // Вызывается при нажатии на "Старт"
        public void AddNew()
        {
            var a = new Animator(ContainerSize); // контейнер из свойства
            animators.Add(a);
            a.Start(); // Запуск аниматора на выполнение
        }

        // Отрисовка сцены
        // Поток будет не двигать шары, а отрисовывать
        public void Start()
        {
            isAlive = true;
            t = new Thread( () =>
            {
                try // для проблемы с закрытием программы
                    // а поток работает
                {
                    while (isAlive)
                    {
                        // match => Animator
                        // it - каждый очередной элемент из коллекции animators
                        // Мы смотрим, it IsAlive или нет
                        // То что вернет Лямба - надо ли удалять объект или 
                        animators.RemoveAll((it) => !it.IsAlive);

                        // Синхронизируем потоки 
                        // Чтобы два и более потока не обращались к общим данным

                        lock (locker)
                        {

                            // Необходимо для проверки на законченность отрисовки
                            // Если все аниматоры отрисованы - работаем дальше
                            // В противном случае ничего не делаем

                            // Однако появятся черный экран.
                            // на PaintOnBuffer отрисовка попадает на старый график
                            // Мы меняем MainGraphic, но на новом MainGraphic (в новом bg ничего не лежит)
                            // Поэтому вылезают черные кадры (не очищенные, не прорисованные и т.д) => ставим Локер + PaintOnBuffer

                            if (PaintOnBuffer())
                            {

                                bg.Render(MainGraphics); // выводим график непосредственно на панель
                                                         // когда все изображение готово
                                                         // на MainGraphics

                            }
                        }

                        if (isAlive) Thread.Sleep(30);
                    }
                }
                catch { }
            });
            t.IsBackground = true;
            t.Start();
        }

        public void Stop()
        {
            isAlive = false;
            t.Interrupt(); // прерывание потока (приводит к исключению во время Sleep / ожидание чего-то)
        }

        private bool PaintOnBuffer()
        {
            objectsPainted = 0;
            var objectsCount = animators.Count;
            bg.Graphics.Clear(Color.White);
            foreach (Animator animator in animators)
            {
                // Двойной буффер
                animator.PaintCircle(bg.Graphics); // чтобы что-то нарисовать исп. свойство Graphics у bg
                objectsPainted++;
            }
            return objectsPainted == objectsCount;
        }

    }
}
