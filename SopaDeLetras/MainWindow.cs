using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SopaDeLetras.Properties;

// ReSharper disable InvertIf

namespace SopaDeLetras
{
    public partial class MainWindow : Form
    {
        private const string Filename = @"sopa.csv";
        private const string AnagFile = @"anagramas.txt";
        private const string Path = @"C:\Users\andriusic\Documents\Visual Studio 2015\Projects\SopaDeLetras\SopaDeLetras\";
        private readonly string[] _words = File.ReadAllLines(Path + "palabras.txt");
        private readonly Random _rnd = new Random();
        private int _firstVal;
        private string[] _bothLists;
        public MainWindow()
        {
            InitializeComponent();
            fileTxt.Lines = _words;
            dificultyCmb.SelectedIndex = 0;
            LoadAnagrams();
        }

        public void ConcatenateFiles()
        {
            _bothLists = new string[fileTxt.Lines.Length + anagramTxt.Lines.Length];
            fileTxt.Lines.CopyTo(_bothLists, 0);
            anagramTxt.Lines.CopyTo(_bothLists, fileTxt.Lines.Length);
        }

        public void LoadAnagrams()
        {
            var anagramList = File.ReadAllLines(Path + AnagFile);
            var foundAnagrams = WordFingerprint(fileTxt.Lines, anagramList);
            anagramTxt.Lines = foundAnagrams;
            ConcatenateFiles();
        }

        public string[] WordFingerprint(string[] wordsList, string[] anagramsList)
        {
            List<string> result = new List<string>();
            

            for (var a = 0; a < anagramsList.Length; a++)
            {
                for (var i = 0; i < wordsList.Length; ++i)
                {
                    if (anagramsList.ElementAt(a).Length == wordsList.ElementAt(i).Length)
                    {
                        int anagramsFingerprint = 0;
                        int wordListFingerprint = 0;

                        for (int j = 0; j < anagramsList.ElementAt(a).Length; j++)
                        {
                            anagramsFingerprint = anagramsFingerprint + anagramsList.ElementAt(a).ElementAt(j);
                            wordListFingerprint = wordListFingerprint + wordsList.ElementAt(i).ElementAt(j);
                            if (anagramsFingerprint == wordListFingerprint && j == anagramsList.ElementAt(a).Length-1)
                            {
                                if (!result.Contains(anagramsList.ElementAt(a)))
                                {
                                    result.Add(anagramsList.ElementAt(a));
                                }
                            }
                        }

                    }

                }
                }
            return result.ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConcatenateFiles();

            switch (dificultyCmb.SelectedIndex)
                {
                    case 0:
                        CreatePuzzle(_bothLists, 4);
                        break;
                    case 1:
                        CreatePuzzle(_bothLists, 3);
                        break;
                    case 2:
                        CreatePuzzle(_bothLists, 2);
                        break;
                    case 3:
                        CreatePuzzle(_bothLists, 1);
                        break;
                    default:
                        MessageBox.Show(Resources.MainWindow_button1_Click_Favor_seleccionar_dificultad);
                        break; 
            }
        }

        public bool RowValidator(int posX ,string word, int dgvSize)
        {
            return posX + word.Length < dgvSize;
        }

        public bool ColumnValidator(int posY, string word, int dgvSize)
        {
            return word.Length + posY < dgvSize;
        }

        //Verify that the assigned spaces are null for the horizontal word
        public bool RowContentCheck(int posX, int posY, string word)
        {
            var aux = 0;
            for (var o = 0; o <= word.Length; o++)
            {
                if (dgv[posX, posY].Value != null)
                {
                    aux++;
                }
                posX++;
            }
            return aux <= 0;
        }
        
        //Verify that the assigned spaces are null for the vertical word
        public bool ColumnContentCheck(int posX, int posY, string word)
        {
            var aux = 0;
            for (var o = 0; o <= word.Length; o++)
            {
                if (dgv[posX, posY].Value != null)
                {
                    aux++;
                }
                posY++;
            }
            return aux <= 0;
        }

        public void CreatePuzzle(string[] wordList, int dificulty)
        {
            //Load file
            button1.Enabled = false;
            //Clear DatagridView 
            ClearDgv(dgv);
            var wordListSize = 0;
            //Define DatagridView Dimentions
            for (int i = 0; i < wordList.Length; i++)
            {
                for (int a = 0; a < wordList.ElementAt(i).Length; a++)
                {
                    wordListSize++;
                }
                
            }
            if (wordListSize < 50)
            {
                dgv.RowCount = wordListSize / 2;
                dgv.ColumnCount = wordListSize / 2;

            }
            else
            {
                dgv.RowCount = wordListSize / dificulty;
                dgv.ColumnCount = wordListSize / dificulty;
            }

            //Creates rows and colums
            for (var i = 0; i < wordList.Length; i++)
            {
                    var word = wordList.ElementAt(i);
                    var posX = _rnd.Next(1,dgv.RowCount);
                    var posY = _rnd.Next(1, dgv.ColumnCount);
                    
                if (i % 2 == 0)
                {
                    //Creates horizontal
                        if (dgv[posX, posY].Value == null  && RowValidator(posX, word,dgv.RowCount) && RowContentCheck(posX,posY,word))
                        {
                            for (var a = 0; a < word.Length; a++)
			                {
                            if(dgv[posX, posY].Value == null)
                            {
                                dgv[posX, posY].Value = word.ElementAt(a);
                                posX++;
                            }
			                
                            }
                        }else{
                            i--;
                        }
                 }
                else
                {
                    //Creates vertical
                    if (dgv[posX, posY].Value == null && ColumnValidator(posY, word, dgv.ColumnCount) && ColumnContentCheck(posX, posY, word))
                    {
                        for (var a = 0; a < word.Length; a++)
                        {

                            if (dgv[posX, posY].Value == null)
                            {
                                dgv[posX, posY].Value = word.ElementAt(a);
                                posY++;
                            }
                            else
                            {
                                for (var e = 0; e < a; e++)
                                {
                                    posX = _rnd.Next(1, dgv.RowCount);
                                    posY = _rnd.Next(1, dgv.ColumnCount);
                                    dgv[posX, posY].Value = null;
                                    i--;
                                }
                            }
                        }
                    }
                    else
                    {
                        i--;
                    }
                }
            }
            FillDgv(dgv);
            button1.Enabled = true;
        }

        //Fill blank spaces with random letters
        public void FillDgv(DataGridView dataGrid)
        {
            var randomChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (var i = 0; i < dataGrid.ColumnCount; i++)
            {
                for (var u = 0; u < dataGrid.RowCount; u++)
                {
                    if (dataGrid[u, i].Value == null)
                    {
                        //Picks a random char from the string above
                        dataGrid[u, i].Value = randomChar.ElementAt(_rnd.Next(1, randomChar.Length));
                    }
                }
            }
        }

        public void SolveHorizontalPuzzle(string[] dict)
        {
            //For runs through the dictionary    
            for (var i = 0; i < dict.Length; i++)
            {
                var currentWord = dict.ElementAt(i);
                //For runs through each row
                for (var row = 0; row < dgv.RowCount; row++)
                {
                    //For runs through each column
                    for (int column = 0; column < dgv.ColumnCount; column++)
                    {
                        string value = dgv.Rows[row].Cells[column].Value.ToString();
                        if (value.ElementAt(0) == currentWord.ElementAt(0) && currentWord.Length + column < dgv.ColumnCount)
                        {
                            var completeWord = "";
                            //Calculate fingerprint
                            //For runs through word to compare 
                            for (var index = 0; index < currentWord.Length; index++)
                            {
                                if (index == 0)
                                {
                                    _firstVal = index + column;
                                }
                                completeWord = completeWord + dgv.Rows[row].Cells[index + column].Value;
                                var fingerPrintCurrent = ValidateWord(currentWord, completeWord);
                                if (fingerPrintCurrent)
                                {
                                    for (int s = index + column; s >= _firstVal; s--)
                                    {
                                        dgv[s, row].Selected = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SolveVerticalPuzzle(string[] dict)
        {
            //For runs through the dictionary    
            for (int i = 0; i < dict.Length; i++)
            {
                var currentWord = dict.ElementAt(i);
                //For runs through each row
                for (int row = 0; row < dgv.ColumnCount; row++)
                {
                    //For runs through each column
                    for (int column = 0; column < dgv.RowCount; column++)
                    {
                        string value = dgv.Rows[row].Cells[column].Value.ToString();
                        if (value.ElementAt(0) == currentWord.ElementAt(0) && currentWord.Length + row < dgv.RowCount)
                        {
                            string completeWord = "";
                            _firstVal = 0;
                            //For runs through word to compare 
                            for (int index = 0; index < currentWord.Length+1; index++)
                            {
                                if (index == 0)
                                {
                                    _firstVal = row;
                                }
                                completeWord = completeWord + dgv.Rows[index+row].Cells[column].Value;
                                bool fingerPrintCurrent = ValidateWord(currentWord, completeWord);
                                if (fingerPrintCurrent)
                                {
                                    for (int s = index + row; s >= _firstVal; s--)
                                    {
                                        dgv[column, s].Selected = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public bool ValidateWord(string word, string wordCompare)
        {
            int aux = 0;
            for (int i = 0; i < wordCompare.Length; i++)
            {
                if (word.Length == wordCompare.Length)
                {
                    if (word.ElementAt(i) == wordCompare.ElementAt(i))
                    {
                        aux++;
                    }
                }
            }
            return aux == word.Length;
        }

        public void ClearDgv(DataGridView dataGrid)
        {
            if(dataGrid.DataSource != null)
            {
                dataGrid.DataSource = null;
            }
            dataGrid.Rows.Clear();
            dataGrid.Refresh();
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            //Saves the content in DataGridView to a .csv file
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
            dgv.SelectAll();
            //Copy selection to clipboard
            // ReSharper disable once AssignNullToNotNullAttribute
            Clipboard.SetDataObject(dgv.GetClipboardContent());
            //Get the clipboard and serialize it to the file
            File.WriteAllText(Path + Filename, Clipboard.GetText(TextDataFormat.CommaSeparatedValue));
            dgv.ClearSelection();
        }
        
        //Loads the file to the DataGridView
        private void button2_Click(object sender, EventArgs e)
        {
            ClearDgv(dgv);
            //Setup the connection to the path of the file
            var conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0; Data Source = " +
                Path + "; Extended Properties = \"Text;HDR=NO;FMT=Delimited\"");
            conn.Open();
            //Setup the adapter with the query
            var adapter = new OleDbDataAdapter
                   ("SELECT * FROM " + "sopa.csv", conn);
            var ds = new DataTable();
            //Fill the DataTable with the adapter
            adapter.Fill(ds);
            conn.Close();
            dgv.RowCount = 0;
            dgv.ColumnCount = 0;
            dgv.DataSource = ds;
            dgv.Refresh();
        }


        public void ShowResult(DataGridView dataGrid)
        {
            for (var i = 0; i < dataGrid.ColumnCount; i++)
            {
                for (var u = 0; u < dataGrid.RowCount; u++)
                {
                    if (!dataGrid[u, i].Selected)
                    {
                        dataGrid[u, i].Value = " ";
                    }
                }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            ConcatenateFiles();
            SolveHorizontalPuzzle(_bothLists);
            SolveVerticalPuzzle(_bothLists);
            dgv[0, 0].Selected = false;
            ShowResult(dgv);
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
            //Copy selection to clipboard
            dgv.SelectAll();
            // ReSharper disable once AssignNullToNotNullAttribute
            Clipboard.SetDataObject(dgv.GetClipboardContent());
            //Get the clipboard and serialize it to the file
            File.WriteAllText(Path + "solucion.txt", Clipboard.GetText(TextDataFormat.CommaSeparatedValue));
            dgv.ClearSelection();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
