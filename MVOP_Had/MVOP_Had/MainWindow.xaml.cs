using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MVOP_Had
{
    public partial class MainWindow : Window
    {
        DispatcherTimer time;
        List<Prop> Snake;
        List<Prop> Apple;
        Random rd = new Random();
        int x = 20;
        int y = 20;
        int direction = 0;
        int up = 1;
        int down = 2;
        int left = 3;
        int right = 4;
        int score = 0;
        int count = 0;
        bool active = false;

        public MainWindow()
        {

        }
        private void NewGame()
        {
            if (!active)
            {
                active = true;
                string file = "text.txt";
                string[] s = File.ReadAllLines(file);
                string[] sh = s[0].Split(' ');
                tbHs.Text = sh[0] + " - Highscore";
            }
            else
            {
                time.Stop();
                score = 0;
                direction = 0;
                x = 0; y = 20;
            }
            label1.Visibility = Visibility.Hidden;
            canvas1.Children.Clear();
            tbS.Text = "Score: " + score;
            time = new DispatcherTimer();
            Snake = new List<Prop>();
            Apple = new List<Prop>();
            Snake.Add(new Prop(x, y));
            Apple.Add(new Prop(rd.Next(0, 19) * 20, rd.Next(0, 19) * 20));
            time.Interval = TimeSpan.FromMilliseconds(100);   /*you can change speed of the snake here */
            time.Tick += time_Tick;
            active = true;
            AddSnake();
            AddApple();
            time.Start();
        }//nová hra

        void AddApple()
        {
            Apple[0].PlaceApple();
            canvas1.Children.Insert(0, Apple[0].ell);
        }//přidá jídlo na canvas


        void AddSnake()
        {
            foreach (Prop snake in Snake)
            {
                snake.PlaceSnake();
                canvas1.Children.Add(snake.rec);
            }
        }// vloží všechny články hada na canvas

        void time_Tick(object sender, EventArgs e)
        {
            if (direction != 0)
            {
                for (int i = Snake.Count - 1; i > 0; i--)
                {
                    Snake[i] = Snake[i - 1];
                }
            }


            if (direction == up)
                y -= 20;
            if (direction == down)
                y += 20;
            if (direction == left)
                x -= 20;
            if (direction == right)
                x += 20;
        
            if (Snake[0].x == Apple[0].x && Snake[0].y == Apple[0].y)
            {
                Snake.Add(new Prop(Apple[0].x, Apple[0].y));
                Apple[0] = new Prop(rd.Next(0, 19) * 20, rd.Next(0, 19) * 20);
                canvas1.Children.RemoveAt(0);
                AddApple();
                score++;
                tbS.Text = "Score: "+score.ToString();
            }


            Snake[0] = new Prop(x, y);

            if (Snake[0].x > 380 || Snake[0].y > 380 || Snake[0].x < 0 || Snake[0].y < 0)
            {
                Death();
            }


            for (int i = 1; i < Snake.Count; i++)
            {
                if (Snake[0].x == Snake[i].x && Snake[0].y == Snake[i].y)
                    Death();
            }


            for (int i = 0; i < canvas1.Children.Count; i++)
            {
                if (canvas1.Children[i] is Rectangle)
                    count++;
            }
            canvas1.Children.RemoveRange(1, count);
            count = 0;
            AddSnake();
        }//pohyb

        private void Death()
        {
            time.Stop();
            SecondWindow sec = new SecondWindow(score, this);
            MessageBoxResult result = MessageBox.Show("Konec hry, vaše score je " + score + ", přejete si score uložit?",
                "GAME OVER",
                MessageBoxButton.YesNo);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    this.Visibility = Visibility.Hidden;
                    sec.Show();
                    break;
                case MessageBoxResult.No:
                    label1.Visibility = Visibility.Visible;
                    break;
            }
        }//prohra

        private void Window_KeyDown(object sender, KeyEventArgs e) 
        {
            if (e.Key == Key.Up && direction != down)
                direction = up;
            if (e.Key == Key.Down && direction != up)
                direction = down;
            if (e.Key == Key.Left && direction != right)
                direction = left;
            if (e.Key == Key.Right && direction != left)
                direction = right;
            if (e.Key == Key.Enter)
            {
                    NewGame();
            }
        }//input

    }
}
