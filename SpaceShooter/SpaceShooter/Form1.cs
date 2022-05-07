using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace SpaceShooter
{
    public partial class Form1 : Form
    {
        // Bg medija se cuje konstantno
        WindowsMediaPlayer gameMedia;

        // Pucanje medija se cuje kada pucamo
        WindowsMediaPlayer shootgMedia;

        // Kada neprijatelj umre cuce se eksplozija
        WindowsMediaPlayer explosion;


        PictureBox[] enemiesMunition;
        int enemiesMunitionSpeed;

        // Dodajemo zvevzde i definisemo brzinu i poziciju
        PictureBox[] stars;
        int backgroundspeed;
        int playerSpeed;

        // Dodajemo municiju koja se pokrece i int za brzinu koji je ceo broj
        PictureBox[] munitions;
        int MunitionSpeed;

        // Dodajemo neprijatelje
        PictureBox[] enemies;
        int enemieSpeed;

        Random rnd;

        int score;
        int level;
        int dificulty;
        bool pause;
        bool gameIsOver;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // na pocetku pauza
            pause = false;
            // sve dok igramo nije gameOver
            gameIsOver = false;
            score = 0;
            level = 1;
            dificulty = 9;

            backgroundspeed = 25;
            playerSpeed = 7;
            enemieSpeed = 10;
            MunitionSpeed = 40;
            enemiesMunitionSpeed = 7;

            munitions = new PictureBox[3];

            // Ubacujemo sliku municije
            Image munition = Image.FromFile(@"asserts\munition.png");

            Image enemi1 = Image.FromFile("asserts\\E1.png");
            Image enemi2 = Image.FromFile("asserts\\E2.png");
            Image enemi3 = Image.FromFile("asserts\\E3.png");
            Image boss1 = Image.FromFile("asserts\\Boss1.png");
            Image boss2 = Image.FromFile("asserts\\Boss2.png");

            enemies = new PictureBox[10];

            // Inicijacija neprijatelja sa loopom
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new PictureBox();
                enemies[i].Size = new Size(40, 40);
                enemies[i].SizeMode = PictureBoxSizeMode.Zoom;
                enemies[i].BorderStyle = BorderStyle.None;
                /* Visible na pocetku je negativna, jer ne treba da ih vidimo
                 kada pocne igra odmah*/
                enemies[i].Visible = false;
                this.Controls.Add(enemies[i]);
                /* Na primer ako je i = 0 dodacemo mu 1 i pomnoziti sa 50,
                 i ostalih - 50 je da popuni celu vrstu sa neprijateljima*/
                enemies[i].Location = new Point((i + 1) * 50, -50);
            }

            enemies[0].Image = boss1;
            enemies[1].Image = enemi2;
            enemies[2].Image = enemi3;
            enemies[3].Image = enemi3;
            enemies[4].Image = enemi1;
            enemies[5].Image = enemi3;
            enemies[6].Image = enemi2;
            enemies[7].Image = enemi3;
            enemies[8].Image = enemi2;
            enemies[9].Image = boss2;


            for (int i = 0; i < munitions.Length; i++)
            {
                munitions[i] = new PictureBox();
                munitions[i].Size = new Size(8, 8);
                munitions[i].Image = munition;
                munitions[i].SizeMode = PictureBoxSizeMode.Zoom;
                munitions[i].BorderStyle = BorderStyle.None;
                this.Controls.Add(munitions[i]);
            }

            // Pravljenje WMP
            gameMedia = new WindowsMediaPlayer();
            shootgMedia = new WindowsMediaPlayer();
            explosion = new WindowsMediaPlayer();

            // Insertujemo sve melodije
            gameMedia.URL = "songs\\GameSong.mp3";
            shootgMedia.URL = "songs\\shoot.mp3";
            explosion.URL = "songs\\boom.mp3";

            // Podesavanje melodija
            gameMedia.settings.setMode("loop", true);
            gameMedia.settings.volume = 5;
            shootgMedia.settings.volume = 1;
            explosion.settings.volume = 5;


            stars = new PictureBox[15];
            rnd = new Random();

            for(int i = 0; i < stars.Length; i++)
            {
                // Zvezde su samo mali kvadratici
                stars[i] = new PictureBox();
                // Ne zelimo da imamo border oko njih
                stars[i].BorderStyle = BorderStyle.None;
                //Dinamicne pozicije za nase zvezde
                stars[i].Location = new Point(rnd.Next(20, 700), rnd.Next(-10, 400));
                if (i % 2 == 1)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[i].BackColor = Color.Wheat;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }
                this.Controls.Add(stars[i]);
            }
            enemiesMunition = new PictureBox[10];

            for(int i = 0; i < enemiesMunition.Length; i++)
            {
                enemiesMunition[i] = new PictureBox();
                enemiesMunition[i].Size = new Size(2, 25);
                enemiesMunition[i].Visible = false;
                enemiesMunition[i].BackColor = Color.Yellow;
                // Generisemo nasumicni broj iz array 
                int j = rnd.Next(0, 10);
                // Dajemo im poziciju za Municiju i bice locirana gde je ene.
                // - 20 je jer zelimo da municija bude u centru neprijatelja
                enemiesMunition[i].Location = new Point(enemies[j].Location.X, enemies[j].Location.Y - 20);
                this.Controls.Add(enemiesMunition[i]);
            }

            // Dopustamo da se pusta u pozadini
            gameMedia.controls.play();
        }

        private void MoveBgTimer_Tick(object sender, EventArgs e)
        {
            //Stavicemo da se neke zvezde pomeraju brze od drugih
            // Od nule pa do pola duzine array
            for (int i = 0; i < stars.Length/2; i++)
            {
                //Definisemo poziciju
                stars[i].Top += backgroundspeed;
                /* Onda cemo da prverimo da li su zvezde tu,
                 ako prodju granicu pordera bice vracene nazad */
                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }

            for (int i = stars.Length / 2; i < stars.Length; i++)
            {
                /* radicemo istu stvar kao gore samo sto ce se ove 
                 Pomerati malo sporije zbog Dinamike */
                stars[i].Top += backgroundspeed - 2;
                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }
        }

        private void LeftMoveTImer_Tick(object sender, EventArgs e)
        {
            if(Player.Left > 10)
            {
                Player.Left -= playerSpeed;
            }
        }

        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            // sa PictureBox ne moze da se koristi .Right
            if (Player.Right < 670)
            {
                Player.Left += playerSpeed;
            }
        }

        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top < 400)
            {
                Player.Top += playerSpeed;
            }
        }

        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 10)
            {
                Player.Top -= playerSpeed;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            /*  Proveravamo da li je igrac stisnu deno, levo, gore 
             ili dole i time smo aktivirali tajmer za dugme koje 
            je pretisnuto. Mi nismo zapravo rekli racunaru sta da radi
            on ce to izvrsiti kada pustimo dugme*/

            /* Najprostije receno, ako stisnes desdno dugme,
             desni tajmer krece i ides desno */

            if(!pause)
            {
                if (e.KeyCode == Keys.Right)
                {
                    RightMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Left)
                {
                    LeftMoveTImer.Start();
                }
                if (e.KeyCode == Keys.Down)
                {
                    DownMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Up)
                {
                    UpMoveTimer.Start();
                }
            }
           
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Kada igrac pusti dugme, ovaj deo se aktivira koji gasi tajmere
            RightMoveTimer.Stop();
            LeftMoveTImer.Stop();
            DownMoveTimer.Stop();
            UpMoveTimer.Stop();

            if (e.KeyCode == Keys.Space)
            {
                if(!gameIsOver)
                {
                    if (pause)
                    {
                        StartTimers();
                        label1.Visible = false;
                        gameMedia.controls.play(); ;
                        pause = false;
                    }
                    else
                    {
                        label1.Location = new Point(this.Width / 2 - 120, 150);
                        label1.Text = "PAUSED";
                        label1.Visible = true;
                        gameMedia.controls.pause();
                        StopTimers();
                        pause = true;
                    }
                }
            }
        }

        private void MoveMunitionTimer_Tick(object sender, EventArgs e)
        {
            // Shooting stavljamo ovde jer smo vec definisali da mozemo da pucamo
            // svakih 20ms, tako da kad pucamo cuce se i sound za to
            shootgMedia.controls.play();

            // Enable je true, jer je pucanje automatski
            /* Podesili smo ga na 20 mili sekundi,
             tako da ce se pokrenti ovde na svakih 20ms */

            for (int i = 0; i < munitions.Length; i++)
            {
                if (munitions[i].Top > 0)
                {
                    // Ako nije dodatako Top, videce se,
                    // i samo smanjimo poziciju da opet bude na 0
                    munitions[i].Visible = true;
                    munitions[i].Top -= MunitionSpeed;

                    // Proveravamo da li je ikakva kolizija izmedju ene i pl
                    Collision();
                    CollisionWithEnemiesMunition();
                }
                else
                {
                    // kada dodje do 0, stavljamo visibiliti na false i
                    // vraca se u poziciju gde je igrac
                    munitions[i].Visible = false;
                    munitions[i].Location = new Point(Player.Location.X + 20, Player.Location.Y - 1 * 30);
                }
            }

            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveEnemies(enemies, enemieSpeed);
        }

        private void MoveEnemies(PictureBox[] array, int speed)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Visible = true;
                array[i].Top += speed;

                if(array[i].Top > this.Height)
                {
                    array[i].Location = new Point((i + 1) * 50, -200);
                }
            }
        }
        private void Collision()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if(munitions[0].Bounds.IntersectsWith(enemies[i].Bounds)
                    || munitions[1].Bounds.IntersectsWith(enemies[i].Bounds) 
                    || munitions[2].Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    // Ako se desi bilo kakva Col. izmedju Pl i Ene,
                    // cuce se explosion i ene ce se vratiti na pocetak
                    explosion.controls.play();

                    score += 1;
                    scorelbl.Text = (score < 10) ? "0" + score.ToString() : score.ToString();
                    

                    if (score % 30 == 0)
                    {
                        level += 1;
                        levellbl.Text = (level < 10) ? "0" + level.ToString() : level.ToString();

                        if (enemieSpeed <= 10 && enemiesMunitionSpeed <= 10 && dificulty >= 0)
                        {
                            dificulty--;
                            enemieSpeed++;
                            enemiesMunitionSpeed++;
                        }
                        if(level == 10)
                        {
                            GameOver("NICE DONE");
                        }
                    }

                    enemies[i].Location = new Point((i + 1) * 50, -100);
                }
                if(Player.Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    // A ako ene. dotakne Pl, Pl nestaje
                    explosion.settings.volume = 30;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over");
                }
            }
        }
        // GameOver ima string kao parametar, tako da ce nam on reci da je gameover
        private void GameOver(string str)
        {
            label1.Text = str;
            label1.Location = new Point(150, 80);
            label1.Visible = true;
            ReplayBtn.Visible = true;
            ExitBtn.Visible = true;

            gameMedia.controls.stop();
            StopTimers();
        }

        // Stop tajmerre
        private void StopTimers()
        {
            MoveBgTimer.Stop();
            MoveEnemiesTimer.Stop();
            MoveMunitionTimer.Stop();
            EnemiesMunitionTimer.Stop();
        }

        // Start Tajmere
        private void StartTimers()
        {
            MoveBgTimer.Start();
            MoveEnemiesTimer.Start();
            MoveMunitionTimer.Start();
            EnemiesMunitionTimer.Start();
        }

        private void EnemiesMunitionTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < enemiesMunition.Length - dificulty; i++)
            {
                if(enemiesMunition[i].Top < this.Height)
                {
                    enemiesMunition[i].Visible = true;
                    enemiesMunition[i].Top += enemiesMunitionSpeed;
                }
                else
                {
                    // Ako izadje municija se ponovo pojavljuje rendom
                    enemiesMunition[i].Visible = false;
                    int j = rnd.Next(0, 10);
                    enemiesMunition[i].Location = new Point(enemies[j].Location.X + 20, enemies[j].Location.Y + 30);
                }
            }
        }
        private void CollisionWithEnemiesMunition()
        {
            for (int i = 0; i < enemiesMunition.Length; i++)
            {
                // Proveravamo da li je ene mnicija napravila kontakt sa Pl
                if (enemiesMunition[i].Bounds.IntersectsWith(Player.Bounds)) 
                {
                    // Ako jeste onda se peimenjuje sledeci statment
                    enemiesMunition[i].Visible = false;
                    explosion.settings.volume = 30;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over");
                }
                
            } 
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ReplayBtn_Click(object sender, EventArgs e)
        {
            // Obrisemo ekran
            this.Controls.Clear();
            InitializeComponent();
            Form1_Load(e, e);
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
