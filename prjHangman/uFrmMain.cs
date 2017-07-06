using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prjHangman
{
    public partial class frmMain : Form
    {
        //all frmMain class level variables.
        string[] wordsArray;
        //diffrent frm ""...prefer way is string.Empty
        string chosenWord = string.Empty;
        string wordToGuess = string.Empty;
        int error = 0;
        int turnsAllowed;
        bool gameInProgress;
        string alphabetsForBtn = "abcdefghijklmnopqrstuvwxyz";

        //constructor frmMain.
        public frmMain()
        {
            InitializeComponent();
            wordsArray = readWord();//returning a string array.
            StartGame();
        }

        /****************************METHOD SECTION*****************************/
        //game starts from here.
        private void StartGame()
        {
            wordToGuess = string.Empty;//to collect user inputs.
            pbHead.Visible = false;
            pbBody.Visible = false;
            pbLeftArm.Visible = false;
            pbRightArm.Visible = false;
            pbLeftLeg.Visible = false;
            pbRightLeg.Visible = false;

            NewGame.Visible = true;
            NewGame.Enabled = true;

            disableAllLabels();//method call to disble all button;

            gameInProgress = true;
            error = 0;

            //error section elements.
            txtForError.Visible = true;
            errorCount.Visible = true;

            chosenWord = SelectWord().ToUpper();//method call to select a word.
            turnsAllowed = allowTurns(chosenWord);//method call toget total turns.
            showTurnsLeft(error);//Method call to show turns left.

            //loop for showing labels equal to length of chosenWord.
            for (int i = 0; i < chosenWord.Length; i++)
            {
                Label labelLetter = Controls["lblLetter" + i.ToString()] as Label;
                labelLetter.Visible = true;
                labelLetter.Text = "____";
                wordToGuess = wordToGuess + i.ToString();//seting indices in wortToGuess.
            }
            EnableDisableBtn(true);//method call to enable all butns.
        }

        //Add new Word to file.
        private void AddWord()
        {
            string word = newWordTxt.Text.ToUpper() + " ";//grabing txt from element newWrodTxt.
            if (word.Length >= 4 && word.Length <= 9)
            {
                File.AppendAllText(@"D:\words.txt", word);
                newWordTxt.Text = string.Empty;
                MessageBox.Show("Word Added", "Word File Status", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Word Length should not be greater than 9", "Word File Status", MessageBoxButtons.OK);
                newWordTxt.Text = string.Empty;
            }
        }

        //read a word from file.
        private string[] readWord()
        {
            string wordFileContent = File.ReadAllText(@"D:\words.txt");//getting all txt of file.
            string[] words = wordFileContent.Split(' ');//spliting in string array based on ' '.
            return words;
        }
        
        //method for enabling all buttons.
        private void EnableDisableBtn(bool flag)
        {
            foreach (var letter in alphabetsForBtn)
            {
                Button btn = Controls["btn" + letter.ToString().ToUpper()] as Button;
                btn.Enabled = flag;
            }
            btnGuess.Enabled = flag;
            txtGuess.Enabled = flag;
            txtGuess.Text = string.Empty;
        }

        //selectWord from words array RANDOMLY.
        private string SelectWord()
        {
            Random randomIndex = new Random();
            return wordsArray[randomIndex.Next(wordsArray.Length - 1)];
        }

        //method to check whether the letter is in ChosenWord or not.
        private void CheckLetter(char letter)
        {
            bool letterFound;

            if (chosenWord.Contains(letter))
            {
                for (int i = 0; i < chosenWord.Length; i++)
                {
                    if (letter.Equals(chosenWord[i]))
                    {
                        Label lableLetter = Controls["lblLetter" + i.ToString()] as Label;
                        lableLetter.Text = letter.ToString();
                        lableLetter.TextAlign = ContentAlignment.TopCenter;

                        wordToGuess = wordToGuess.Replace(i.ToString(), letter.ToString());
                        letterFound = true;

                        checkerWin(letterFound);//Method call to check 'WIN'.
                    }
                }
            }
            else
            {
                error++;
                showTurnsLeft(error);
                switch (error)
                {
                    case 1:
                        pbHead.Visible = true;
                        break;
                    case 2:
                        pbBody.Visible = true;
                        break;
                    case 3:
                        pbLeftArm.Visible = true;
                        pbLeftArm.BringToFront();
                        break;
                    case 4:
                        pbRightArm.Visible = true;
                        pbRightArm.BringToFront();
                        break;
                    case 5:
                        pbLeftLeg.Visible = true;
                        break;
                    case 6:
                        pbRightLeg.Visible = true;
                        gameInProgress = false;
                        loseImage.Visible = true;
                        MessageBox.Show("You were hanged!!!", "Project Hangman",        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        EnableDisableBtn(false);
                        RevealWords();
                        break;
                }
            }
        }

        //method to check player wins or not.
        private void checkerWin(bool letterFound)
        {
            if (wordToGuess.Equals(chosenWord))
            {
                gameInProgress = false;
                winImage.Visible = true;
                MessageBox.Show("Congo!!! you won the game.", "Project Hangman", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                EnableDisableBtn(false);
            }
        }

        //method to Reveal the word to player.
        private void RevealWords()
        {
            for (int i = 0; i < chosenWord.Length; i++)
            {
                Controls["lblLetter" + i.ToString()].Text = chosenWord[i].ToString().ToUpper();
            }
        }

        //method to return allowedTurns.
        private int allowTurns(string chosenWord)
        {
            return chosenWord.Length;
        }

        //to show turns left.
        private void showTurnsLeft(int err)
        {
            int turnsLeft = turnsAllowed - err;
            Controls["errorCount"].Text = turnsLeft.ToString();
        }

        //method to start a new game from menu.
        private void startNewGame()
        {
            winImage.Visible = false;
            loseImage.Visible = false;
            if (gameInProgress == true)
            {
                if (MessageBox.Show("Do you want to start a new game?", "Hangman", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    StartGame();
                }
                else
                {
                    return;
                }
            }
            StartGame();
        }

        //method for selecting a game level.
        private void selectGameLevel(string level)
        {
            if (level.Equals(easyRadio.Name.ToString()))
            {
                turnsAllowed = allowTurns(chosenWord);//method call toget total turns.
                showTurnsLeft(error);//Method call to show turns left.
            }
            else if (level.Equals(mediumRadio.Name.ToString()))
            {
                int totalTurns = allowTurns(chosenWord);//method call toget total turns.
                turnsAllowed = totalTurns - 2;
                showTurnsLeft(error);//Method call to show turns left.

            }
            else if (level.Equals(hardRadio.Name.ToString()))
            {
                int totalTurns = allowTurns(chosenWord);//method call toget total turns.
                turnsAllowed = (totalTurns / 2);
                showTurnsLeft(error);//Method call to show turns left.
            }
        }

        //method call to disable all labels.
        private void disableAllLabels()
        {
            for (int i = 0; i < 9; i++)
            {
                Controls["lblLetter" + i.ToString()].Visible = false;
            }
        }

        //method call to tryToGuess.
        void tryToGuess(string guessedWord)
        {
            if (MessageBox.Show("Do you really want to guess", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (guessedWord.Equals(chosenWord))
                {
                    gameInProgress = false;
                    winImage.Visible = true;
                    MessageBox.Show("Congo!!! you won the game.", "Project Hangman", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    EnableDisableBtn(false);
                }

                else
                {

                    pbHead.Visible = true;
                    pbBody.Visible = true;
                    pbLeftArm.Visible = true;
                    pbRightArm.Visible = true;
                    pbLeftLeg.Visible = true;
                    pbRightLeg.Visible = true;
                    gameInProgress = true;
                    loseImage.Visible = true;
                    MessageBox.Show("You were hanged!!!", "Project Hangman", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    EnableDisableBtn(false);
                    RevealWords();
                }
            }
        }



        /********************EVENT SECTION*************************/

        //event capturing for all btns.
        private void CaptureLetter(object sender, EventArgs e)
        {
            char letter;//define variable to store btn letter.
            Button button = (Button)sender;//casting object to button.
            letter = Convert.ToChar(button.Text);
            button.Enabled = false;

            CheckLetter(letter);//calling 'checkLetter' method.
        }

        //Event to create a new file for words.
        private void newFilebtn_Click(object sender, EventArgs e)
        {
            AddWord();
        }

        //event handler for start a new game....clicked from menu.
        private void NewGame_Click(object sender, EventArgs e)
        {
            startNewGame();
        }

        //event handler for easy radio btn ...level easy.
        private void easyRadio_CheckedChanged(object sender, EventArgs e)
        {
            selectGameLevel(easyRadio.Name.ToString());
        }

        //event handler for meduim radio btn....level medium
        private void mediumRadio_CheckedChanged(object sender, EventArgs e)
        {
            selectGameLevel(mediumRadio.Name.ToString());
        }

        //event handler for hard radio btn ..... level hard.
        private void hardRadio_CheckedChanged(object sender, EventArgs e)
        {
            selectGameLevel(hardRadio.Name.ToString());
        }

        private void btnGuess_Click(object sender, EventArgs e)
        {
            //trying to guess number..
            tryToGuess(txtGuess.Text.ToUpper());
        }
    }
}
