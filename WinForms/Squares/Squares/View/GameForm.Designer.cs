using Squares.View.CustomControls;

namespace Squares.View
{
    partial class GameForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this._menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFileNewGame = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this._menuFileLoadGame = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFileSaveGame = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this._menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this._menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this._menuGame3_3 = new System.Windows.Forms.ToolStripMenuItem();
            this._menuGame5_5 = new System.Windows.Forms.ToolStripMenuItem();
            this._menuGame9_9 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this._scoreLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this._scoreLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this._currentPlayerText = new System.Windows.Forms.ToolStripStatusLabel();
            this._currentPlayerLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _menuStrip
            // 
            this._menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuFile,
            this._menuSettings});
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Padding = new System.Windows.Forms.Padding(9, 4, 0, 4);
            this._menuStrip.Size = new System.Drawing.Size(622, 32);
            this._menuStrip.TabIndex = 1;
            this._menuStrip.Text = "menuStrip1";
            // 
            // _menuFile
            // 
            this._menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuFileNewGame,
            this.toolStripMenuItem1,
            this._menuFileLoadGame,
            this._menuFileSaveGame,
            this.toolStripMenuItem2,
            this._menuFileExit});
            this._menuFile.Name = "_menuFile";
            this._menuFile.Size = new System.Drawing.Size(46, 24);
            this._menuFile.Text = "File";
            // 
            // _menuFileNewGame
            // 
            this._menuFileNewGame.Name = "_menuFileNewGame";
            this._menuFileNewGame.Size = new System.Drawing.Size(168, 26);
            this._menuFileNewGame.Text = "New Game";
            this._menuFileNewGame.Click += new System.EventHandler(this.MenuFileNewGame_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 6);
            // 
            // _menuFileLoadGame
            // 
            this._menuFileLoadGame.Name = "_menuFileLoadGame";
            this._menuFileLoadGame.Size = new System.Drawing.Size(168, 26);
            this._menuFileLoadGame.Text = "Load Game";
            this._menuFileLoadGame.Click += new System.EventHandler(this.MenuFileLoadGame_Click);
            // 
            // _menuFileSaveGame
            // 
            this._menuFileSaveGame.Name = "_menuFileSaveGame";
            this._menuFileSaveGame.Size = new System.Drawing.Size(168, 26);
            this._menuFileSaveGame.Text = "Save Game";
            this._menuFileSaveGame.Click += new System.EventHandler(this.MenuFileSaveGame_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(165, 6);
            // 
            // _menuFileExit
            // 
            this._menuFileExit.Name = "_menuFileExit";
            this._menuFileExit.Size = new System.Drawing.Size(168, 26);
            this._menuFileExit.Text = "Exit";
            this._menuFileExit.Click += new System.EventHandler(this.MenuFileExit_Click);
            // 
            // _menuSettings
            // 
            this._menuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuGame3_3,
            this._menuGame5_5,
            this._menuGame9_9});
            this._menuSettings.Name = "_menuSettings";
            this._menuSettings.Size = new System.Drawing.Size(76, 24);
            this._menuSettings.Text = "Settings";
            // 
            // _menuGame3_3
            // 
            this._menuGame3_3.Name = "_menuGame3_3";
            this._menuGame3_3.Size = new System.Drawing.Size(115, 26);
            this._menuGame3_3.Text = "3x3";
            this._menuGame3_3.Click += new System.EventHandler(this.MenuGame3_3_Click);
            // 
            // _menuGame5_5
            // 
            this._menuGame5_5.Name = "_menuGame5_5";
            this._menuGame5_5.Size = new System.Drawing.Size(115, 26);
            this._menuGame5_5.Text = "5x5";
            this._menuGame5_5.Click += new System.EventHandler(this.MenuGame5_5_Click);
            // 
            // _menuGame9_9
            // 
            this._menuGame9_9.Name = "_menuGame9_9";
            this._menuGame9_9.Size = new System.Drawing.Size(115, 26);
            this._menuGame9_9.Text = "9x9";
            this._menuGame9_9.Click += new System.EventHandler(this.MenuGame9_9_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._scoreLabel1,
            this._scoreLabel2,
            this._currentPlayerText,
            this._currentPlayerLabel});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Location = new System.Drawing.Point(0, 637);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 11, 0);
            this.statusStrip.Size = new System.Drawing.Size(622, 38);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // _scoreLabel1
            // 
            this._scoreLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this._scoreLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this._scoreLabel1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._scoreLabel1.Name = "_scoreLabel1";
            this._scoreLabel1.Size = new System.Drawing.Size(101, 32);
            this._scoreLabel1.Text = "score1: 0";
            // 
            // _scoreLabel2
            // 
            this._scoreLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this._scoreLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this._scoreLabel2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._scoreLabel2.Name = "_scoreLabel2";
            this._scoreLabel2.Size = new System.Drawing.Size(101, 32);
            this._scoreLabel2.Text = "score2: 0";
            // 
            // _currentPlayerText
            // 
            this._currentPlayerText.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._currentPlayerText.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._currentPlayerText.Name = "_currentPlayerText";
            this._currentPlayerText.Size = new System.Drawing.Size(59, 32);
            this._currentPlayerText.Text = "Player";
            // 
            // _currentPlayerLabel
            // 
            this._currentPlayerLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._currentPlayerLabel.Name = "_currentPlayerLabel";
            this._currentPlayerLabel.Size = new System.Drawing.Size(105, 32);
            this._currentPlayerLabel.Text = "Current player:";
            // 
            // _openFileDialog
            // 
            this._openFileDialog.Filter = "Squares table (*.sqt)|*.sqt";
            this._openFileDialog.Title = "Load Squares table";
            // 
            // _saveFileDialog
            // 
            this._saveFileDialog.Filter = "Squares table (*.sqt)|*.sqt";
            this._saveFileDialog.Title = "Save Squares game";
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 600);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this._menuStrip);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(580, 600);
            this.Name = "GameForm";
            this.Text = "Squares";
            this.Load += new System.EventHandler(this.GameForm_Load);
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem _menuFile;
        private System.Windows.Forms.ToolStripMenuItem _menuFileNewGame;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem _menuFileLoadGame;
        private System.Windows.Forms.ToolStripMenuItem _menuFileSaveGame;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem _menuFileExit;
        private CustomControls.DrawablePanel<VertexView> _gamePanel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _scoreLabel1;
        private System.Windows.Forms.ToolStripStatusLabel _scoreLabel2;
        private System.Windows.Forms.ToolStripMenuItem _menuSettings;
        private System.Windows.Forms.ToolStripMenuItem _menuGame3_3;
        private System.Windows.Forms.ToolStripMenuItem _menuGame5_5;
        private System.Windows.Forms.ToolStripMenuItem _menuGame9_9;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.SaveFileDialog _saveFileDialog;
        private System.Windows.Forms.ToolStripStatusLabel _currentPlayerLabel;
        private System.Windows.Forms.ToolStripStatusLabel _currentPlayerText;
    }
}

