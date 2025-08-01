using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PararealImageViewer
{
    public partial class Form1 : Form
    {
        private readonly CommonOpenFileDialog folderDialog;
        private string currentOpenDir;
        private int currentFileIndex;
        private List<string> currentImageList;

        public Form1()
        {
            InitializeComponent();

            this.folderDialog = new CommonOpenFileDialog
            {
                Title = "フォルダを選択してください",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false,
                IsFolderPicker = true
            };
            this.currentOpenDir = "";
            this.currentFileIndex = 0;
            this.currentImageList = new List<string>();
        }

        /// <summary>
        /// 表示時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // アプリ名、バージョン表示
            var assembly = Assembly.GetExecutingAssembly();
            var asmName = assembly.GetName();
            this.WriteStatus($"{asmName.Name} ver.{asmName.Version}");

            // 初期画像
            this.pictureBox1.Image = Properties.Resources.NoImage;
        }

        /// <summary>
        /// 終了時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.folderDialog.Dispose();
        }

        /// <summary>
        /// 画像クリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // 操作部表示反転
            this.panel1.Visible = !this.panel1.Visible;
        }

        /// <summary>
        /// [開く]ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // 開く
            CommonFileDialogResult result = folderDialog.ShowDialog();

            if (result != CommonFileDialogResult.Ok) return;

            this.currentOpenDir = this.folderDialog.FileName;

            // ファイル検索
            this.currentImageList = Directory.EnumerateFiles(this.currentOpenDir)
                .Where(filename => filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    filename.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 表示
            this.currentFileIndex = (this.currentImageList.Count == 0) ? -1 : 0;
            this.ShowImage((this.currentFileIndex < 0) ? "" : this.currentImageList[this.currentFileIndex]);
        }

        /// <summary>
        /// [左]ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.currentFileIndex = (this.currentImageList.Count == 0)
                ? -1
                : (this.currentFileIndex == 0) ? this.currentImageList.Count - 1 : this.currentFileIndex - 1;
            this.ShowImage((this.currentFileIndex < 0) ? "" : this.currentImageList[this.currentFileIndex]);
        }

        /// <summary>
        /// [右]ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.currentFileIndex = (this.currentImageList.Count == 0)
                ? -1
                : (this.currentFileIndex == this.currentImageList.Count - 1) ? 0 : this.currentFileIndex + 1;
            this.ShowImage((this.currentFileIndex < 0) ? "" : this.currentImageList[this.currentFileIndex]);
        }

        /// <summary>
        /// [終了]ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 内部

        /// <summary>
        /// 画像を表示
        /// </summary>
        /// <param name="path">画像のパス</param>
        private void ShowImage(string path)
        {
            // メモリ解放
            this.pictureBox1.Image?.Dispose();
            this.pictureBox1.Image = null;

            // 画像設定
            Image img = null;
            if (File.Exists(path))
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    img = Image.FromStream(fs);
                }
                this.WriteStatus(path);
            }
            else
            {
                // 内蔵リソース画像
                img = Properties.Resources.NoImage;
                this.WriteStatus(path + "(ファイルなし)");
            }
            this.pictureBox1.Image = img;
        }

        /// <summary>
        /// テキスト書き込み
        /// </summary>
        /// <param name="message">メッセージ</param>
        private void WriteStatus(string message)
        {
            this.label1.Text = message;
        }
    }
}
