
/*
  błędy:
     - sprajty dziwnie się ucinają, kiedy są częściowo za ścianami
     - sprajty się przesuwają (?!!)

  do poprawienia:
    - rysowanie sprajtów jest bardzo obciążające dla programu, trzeba zmienić metodę
      chyba nie trzeba nawet próbować rysowania sprajta, jeżeli pomiędzy nim a graczem jest ściana
      ścianę trzeba wykrywać promieniem z dwóch punktów skrajnych sprajta, żeby uniknąć gliczy
    - animacje powinno wołać się po nazwie, a nie po indeksie

  ewentualne ulepszenia:
    - znaleźć bardziej optymalny sposób rysowania podług
    - może to samo dla sprajtów (?)
    - sprawdzić, czy można usprawnić rysowanie dekali
    - nie trzeba rysować ścian, których no i tak nie widać (boki klocków przy łączeniach)
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace DDA
{
    public partial class GameWindow : Form
    {
        Timer mainTimer;

        public GameWindow()
        {
            InitializeComponent();
        }

        void OnLoad(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            this.BackColor = Color.White;

            mainTimer = new Timer
            {
                Interval = 1000 / Main.targetFPS
            };
            mainTimer.Tick += new EventHandler(Tick);
            mainTimer.Start();

            Main.Init(this);
        }
        void OnKeyDown(object sender, KeyEventArgs e)
        {
            Input.KeyDown(e.KeyCode);
        }
        void OnKeyUp(object sender, KeyEventArgs e)
        {
            Input.KeyUp(e.KeyCode);
        }
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            Input.MouseDown(e.Button);
        }
        void Tick(object sender, EventArgs e)
        {
            Main.Tick();
        }

    }
}
