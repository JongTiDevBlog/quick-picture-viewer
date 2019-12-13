﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace quick_picture_viewer
{
	public partial class MainForm : Form
	{
		private string openPath;
		private int zoomFactor = 100;
		private int width = 0;
		private int height = 0;
		private Bitmap originalImage;
		private bool autoZoom = true;
		private Point panelMouseDownLocation;
		private bool fullscreen = false;

		public MainForm(string openPath)
		{
			this.openPath = openPath;

			InitializeComponent();
		}

		private void openButton_Click_1(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				openFile(openFileDialog1.FileName);
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			if(!string.IsNullOrEmpty(openPath))
			{
				openFile(openPath);
			}

			picturePanel.MouseWheel += new MouseEventHandler(picturePanel_MouseWheel);
		}

		private void openFile(string path)
		{
			try
			{
				openImage(new Bitmap(path), path, Path.GetFileName(path));
			}
			catch
			{
				MessageBox.Show("Unable to open this file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void openImage(Bitmap bitmap, string filePath, string fileName)
		{
			originalImage = bitmap;
			pictureBox.Image = originalImage;

			width = pictureBox.Image.Size.Width;
			height = pictureBox.Image.Size.Height;
			sizeLabel.Text = "Size: " + width.ToString() + " x " + height.ToString() + " px";
			pathLabel.Text = "File: " + filePath;

			this.Text = fileName + " - Quick Picture Viewer";

			zoomInButton.Enabled = true;
			zoomOutButton.Enabled = true;
			flipVerticalButton.Enabled = true;
			flipHorizontalButton.Enabled = true;
			rotateLeftButton.Enabled = true;
			rotateRightButton.Enabled = true;
			saveAsButton.Enabled = true;
			copyButton.Enabled = true;

			zoomComboBox.Enabled = true;

			setZoomText("Auto");
		}

		private void zoomInButton_Click(object sender, EventArgs e)
		{
			zoomIn();
		}

		private void zoomOutButton_Click(object sender, EventArgs e)
		{
			zoomOut();
		}

		private void zoomIn()
		{
			setZoomText((zoomFactor + 5).ToString() + "%");
		}

		private void zoomOut()
		{
			setZoomText((zoomFactor - 5).ToString() + "%");
		}

		private void setZoomFactor(int newZoomFactor)
		{
			zoomFactor = newZoomFactor;

			setAutoZoom(false);

			int newWidth = Convert.ToInt32(width * zoomFactor / 100);
			int newHeight = Convert.ToInt32(height * zoomFactor / 100);

			Size newSize = new Size(newWidth, newHeight);

			Bitmap bmp = new Bitmap(originalImage, newSize);
			pictureBox.Image = bmp;

			//updatePictureBoxLocation();
		}

		private void updatePictureBoxLocation()
		{
			if (pictureBox.Image.Width < picturePanel.Width)
			{
				pictureBox.Left = (picturePanel.Width - pictureBox.Image.Width) / 2 - picturePanel.HorizontalScroll.Value;
			}
			else
			{
				pictureBox.Left = 0;
			}

			if (pictureBox.Image.Height < picturePanel.Height)
			{
				pictureBox.Top = (picturePanel.Height - pictureBox.Image.Height) / 2 - picturePanel.VerticalScroll.Value;
			}
			else
			{
				pictureBox.Top = 0;
			}
		}

		private void setZoomText(string text)
		{
			zoomComboBox.Text = text;
			zoomLabel.Text = "Zoom: " + text;
		}

		private void setAutoZoom(bool b)
		{
			autoZoom = b;
			if (b)
			{
				pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox.Dock = DockStyle.Fill;
			}
			else
			{
				pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
				pictureBox.Dock = DockStyle.None;
			}
		}

		private void autoZoomButton_Click(object sender, EventArgs e)
		{
			if (zoomComboBox.Text == "Auto")
			{
				setZoomText("100%");
			}
			else
			{
				setZoomText("Auto");
			}
		}

		private void aboutButton_Click(object sender, EventArgs e)
		{
			//if(MessageBox.Show(
			//	"Quick Picture Viewer\nv" + ProductVersion + "\n\nby Module Art\nAuthor: Jake Volynko\n\nVisit a project web page?\nhttps://github.com/ModuleArt/quick-picture-viewer/", 
			//	"About", 
			//	MessageBoxButtons.YesNo, 
			//	MessageBoxIcon.Asterisk
			//) == DialogResult.Yes)
			//{
			//	System.Diagnostics.Process.Start("https://github.com/ModuleArt/quick-picture-viewer/");
			//} 
			AboutBox1 aboutBox = new AboutBox1();
			aboutBox.ShowDialog();
		}

		private void flipVerticalButton_Click(object sender, EventArgs e)
		{
			originalImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
			pictureBox.Image = originalImage;
			setZoomText("Auto");
		}

		private void flipHorizontalButton_Click(object sender, EventArgs e)
		{
			originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
			pictureBox.Image = originalImage;
			setZoomText("Auto");
		}

		private void rotateLeftButton_Click(object sender, EventArgs e)
		{
			originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
			pictureBox.Image = originalImage;
			setZoomText("Auto");
		}

		private void rotateRightButton_Click(object sender, EventArgs e)
		{
			originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
			pictureBox.Image = originalImage;
			setZoomText("Auto");
		}

		private void saveAsButton_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
				switch (saveFileDialog1.FilterIndex)
				{
					case 1:
						originalImage.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
						break;
					case 2:
						originalImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
					case 3:
						originalImage.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
						break;
					case 4:
						originalImage.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
						break;
				}
				fs.Close();
			}
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetImage(originalImage);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsImage())
			{
				openImage(new Bitmap(Clipboard.GetImage()), "From clipboard", "From clipboard");
			}
			else if (Clipboard.ContainsData(DataFormats.FileDrop))
			{
				string path = ((string[]) Clipboard.GetData(DataFormats.FileDrop))[0];
				openFile(path);
			}
		}

		private void picturePanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && !autoZoom) {
				panelMouseDownLocation = new Point(
					e.Location.X + picturePanel.HorizontalScroll.Value, 
					e.Location.Y + picturePanel.VerticalScroll.Value
				);
			}
		}

		private void picturePanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && !autoZoom)
			{
				int newX = panelMouseDownLocation.X - e.X;
				int newY = panelMouseDownLocation.Y - e.Y;

				if(newX > picturePanel.HorizontalScroll.Minimum)
				{
					if(newX < picturePanel.HorizontalScroll.Maximum)
					{
						picturePanel.HorizontalScroll.Value = newX;
					}
					else
					{
						picturePanel.HorizontalScroll.Value = picturePanel.HorizontalScroll.Maximum;
					}
				}
				else
				{
					picturePanel.HorizontalScroll.Value = picturePanel.HorizontalScroll.Minimum;
				}

				if (newY > picturePanel.VerticalScroll.Minimum)
				{
					if (newY < picturePanel.VerticalScroll.Maximum)
					{
						picturePanel.VerticalScroll.Value = newY;
					}
					else
					{
						picturePanel.VerticalScroll.Value = picturePanel.VerticalScroll.Maximum;
					}
				}
				else
				{
					picturePanel.VerticalScroll.Value = picturePanel.VerticalScroll.Minimum;
				}
			}
		}

		private void picturePanel_MouseWheel(object sender, MouseEventArgs e)
		{
			if(Control.ModifierKeys == Keys.Control)
			{
				int newVertical = picturePanel.VerticalScroll.Value + e.Delta;

				if (newVertical > picturePanel.VerticalScroll.Minimum)
				{
					if (newVertical < picturePanel.VerticalScroll.Maximum)
					{
						picturePanel.VerticalScroll.Value = newVertical;
					}
					else
					{
						picturePanel.VerticalScroll.Value = picturePanel.VerticalScroll.Maximum;
					}
				}
				else 
				{
					picturePanel.VerticalScroll.Value = picturePanel.VerticalScroll.Minimum;
				}

				if (e.Delta > 0)
				{
					zoomIn();
				}
				else if(e.Delta < 0)
				{
					zoomOut();
				}
			}
			else if (Control.ModifierKeys == Keys.Shift && !autoZoom)
			{
				int newVertical = picturePanel.VerticalScroll.Value + e.Delta;

				if (newVertical > picturePanel.VerticalScroll.Minimum)
				{
					if (newVertical < picturePanel.VerticalScroll.Maximum)
					{
						picturePanel.VerticalScroll.Value = newVertical;
					}
					else
					{
						picturePanel.VerticalScroll.Value = picturePanel.VerticalScroll.Maximum;
					}
				}
				else
				{
					picturePanel.VerticalScroll.Value = picturePanel.VerticalScroll.Minimum;
				}

				int newHorizontal = picturePanel.HorizontalScroll.Value + e.Delta;

				if (newHorizontal > picturePanel.HorizontalScroll.Minimum)
				{
					if(newHorizontal < picturePanel.HorizontalScroll.Maximum)
					{
						picturePanel.HorizontalScroll.Value = newHorizontal;
					}
					else
					{
						picturePanel.HorizontalScroll.Value = picturePanel.HorizontalScroll.Maximum;
					}
				}
				else
				{
					picturePanel.HorizontalScroll.Value = picturePanel.HorizontalScroll.Minimum;
				}
			}
		}

		private void fullscreenButton_Click(object sender, EventArgs e)
		{
			setFullscreen(!fullscreen);
		}

		private void setFullscreen(bool b)
		{
			fullscreen = b;

			this.WindowState = FormWindowState.Normal;

			if (b)
			{
				this.FormBorderStyle = FormBorderStyle.None;
				this.WindowState = FormWindowState.Maximized;
			}
			else
			{
				this.FormBorderStyle = FormBorderStyle.Sizable;
			}
		}

		private void zoomComboBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (zoomComboBox.Text == "Auto")
				{
					setZoomFactor(100);
					setAutoZoom(true);
				}
				else
				{
					string substr = zoomComboBox.Text.Replace("%", "");
					int zoom = int.Parse(substr);

					if (zoom < 5)
					{
						zoom = 5;
						setZoomText(zoom.ToString() + "%");
					}
					else
					{
						if (zoom > 500)
						{
							zoom = 500;
							setZoomText(zoom.ToString() + "%");
						}
						else
						{
							setZoomFactor(zoom);
						}
					}
				}				
			}
			catch
			{

			}
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				if (e.Shift)
				{
					if (e.KeyCode == Keys.R)
					{
						rotateLeftButton.PerformClick();
					}
					else if (e.KeyCode == Keys.H)
					{
						flipVerticalButton.PerformClick();
					}
				}
				else
				{
					if (e.KeyCode == Keys.O)
					{
						openButton.PerformClick();
					}
					else if (e.KeyCode == Keys.S)
					{
						saveAsButton.PerformClick();
					}
					else if (e.KeyCode == Keys.A)
					{
						autoZoomButton.PerformClick();
					}
					else if (e.KeyCode == Keys.R)
					{
						rotateRightButton.PerformClick();
					}
					else if (e.KeyCode == Keys.H)
					{
						flipHorizontalButton.PerformClick();
					}
					else if (e.KeyCode == Keys.C)
					{
						copyButton.PerformClick();
					}
					else if (e.KeyCode == Keys.V)
					{
						pasteButton.PerformClick();
					}
					else if (e.KeyCode == Keys.Oemplus)
					{
						zoomInButton.PerformClick();
					}
					else if (e.KeyCode == Keys.OemMinus)
					{
						zoomOutButton.PerformClick();
					}
				}
			}
			else
			{
				if (e.KeyCode == Keys.F11)
				{
					fullscreenButton.PerformClick();
				}
			}
		}

		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			Bitmap bitmap = e.Data.GetData(DataFormats.Bitmap) as Bitmap;
			string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

			if (bitmap != null)
			{
				openImage(bitmap, "Dragged image", "Dragged image");
			}
			else if (files.Length > 0)
			{
				string path = files[0];
				openFile(path);
			}
		}

		private void MainForm_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Bitmap))
			{
				e.Effect = DragDropEffects.All;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}
	}
}