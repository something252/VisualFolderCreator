<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NewScreen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.DebugTestToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BREAKToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.ContextMenuStripListBox = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.RightClickMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenContainingFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsMenuToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveScreenAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.ListBox1Names = New System.Windows.Forms.ListBox()
        Me.OpenContainingFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStripListBox.SuspendLayout()
        Me.RightClickMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Enabled = False
        Me.MenuStrip1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DebugTestToolStripMenuItem, Me.BREAKToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1289, 29)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        Me.MenuStrip1.Visible = False
        '
        'DebugTestToolStripMenuItem
        '
        Me.DebugTestToolStripMenuItem.Name = "DebugTestToolStripMenuItem"
        Me.DebugTestToolStripMenuItem.Size = New System.Drawing.Size(109, 25)
        Me.DebugTestToolStripMenuItem.Text = "DEBUG TEST"
        '
        'BREAKToolStripMenuItem
        '
        Me.BREAKToolStripMenuItem.Name = "BREAKToolStripMenuItem"
        Me.BREAKToolStripMenuItem.Size = New System.Drawing.Size(68, 25)
        Me.BREAKToolStripMenuItem.Text = "BREAK"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.PictureBox1.Enabled = False
        Me.PictureBox1.Location = New System.Drawing.Point(21, 56)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(99, 100)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        Me.PictureBox1.Visible = False
        '
        'ListBox1
        '
        Me.ListBox1.AllowDrop = True
        Me.ListBox1.ContextMenuStrip = Me.ContextMenuStripListBox
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.HorizontalScrollbar = True
        Me.ListBox1.ItemHeight = 18
        Me.ListBox1.Location = New System.Drawing.Point(641, 652)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(561, 76)
        Me.ListBox1.TabIndex = 2
        Me.ListBox1.Visible = False
        '
        'ContextMenuStripListBox
        '
        Me.ContextMenuStripListBox.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteToolStripMenuItem1, Me.OpenToolStripMenuItem, Me.OpenContainingFolderToolStripMenuItem, Me.ViewScreenToolStripMenuItem})
        Me.ContextMenuStripListBox.Name = "ContextMenuStripListBox"
        Me.ContextMenuStripListBox.Size = New System.Drawing.Size(202, 92)
        '
        'DeleteToolStripMenuItem1
        '
        Me.DeleteToolStripMenuItem1.Name = "DeleteToolStripMenuItem1"
        Me.DeleteToolStripMenuItem1.Size = New System.Drawing.Size(137, 22)
        Me.DeleteToolStripMenuItem1.Text = "Delete"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.OpenToolStripMenuItem.Text = "Open"
        '
        'ViewScreenToolStripMenuItem
        '
        Me.ViewScreenToolStripMenuItem.Name = "ViewScreenToolStripMenuItem"
        Me.ViewScreenToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.ViewScreenToolStripMenuItem.Text = "View Screen"
        '
        'ListView1
        '
        Me.ListView1.Alignment = System.Windows.Forms.ListViewAlignment.Left
        Me.ListView1.AllowDrop = True
        Me.ListView1.AutoArrange = False
        Me.ListView1.BackColor = System.Drawing.Color.Black
        Me.ListView1.BackgroundImageTiled = True
        Me.ListView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ListView1.ContextMenuStrip = Me.RightClickMenu
        Me.ListView1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView1.ForeColor = System.Drawing.Color.White
        Me.ListView1.LargeImageList = Me.ImageList1
        Me.ListView1.Location = New System.Drawing.Point(149, 56)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Scrollable = False
        Me.ListView1.Size = New System.Drawing.Size(1084, 575)
        Me.ListView1.TabIndex = 3
        Me.ListView1.TileSize = New System.Drawing.Size(77, 105)
        Me.ListView1.UseCompatibleStateImageBehavior = False
        '
        'RightClickMenu
        '
        Me.RightClickMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem1, Me.OpenContainingFileToolStripMenuItem, Me.DeleteToolStripMenuItem, Me.ViewListToolStripMenuItem, Me.OptionsMenuToolStripMenuItem, Me.SaveScreenAsToolStripMenuItem})
        Me.RightClickMenu.Name = "RightClickMenu"
        Me.RightClickMenu.Size = New System.Drawing.Size(215, 136)
        '
        'OpenToolStripMenuItem1
        '
        Me.OpenToolStripMenuItem1.Name = "OpenToolStripMenuItem1"
        Me.OpenToolStripMenuItem1.Size = New System.Drawing.Size(214, 22)
        Me.OpenToolStripMenuItem1.Text = "Open"
        '
        'OpenContainingFileToolStripMenuItem
        '
        Me.OpenContainingFileToolStripMenuItem.Name = "OpenContainingFileToolStripMenuItem"
        Me.OpenContainingFileToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.OpenContainingFileToolStripMenuItem.Text = "Open Containing Folder(s)"
        '
        'DeleteToolStripMenuItem
        '
        Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        Me.DeleteToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.DeleteToolStripMenuItem.Text = "Delete"
        '
        'ViewListToolStripMenuItem
        '
        Me.ViewListToolStripMenuItem.Name = "ViewListToolStripMenuItem"
        Me.ViewListToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.ViewListToolStripMenuItem.Text = "View List"
        '
        'OptionsMenuToolStripMenuItem
        '
        Me.OptionsMenuToolStripMenuItem.Name = "OptionsMenuToolStripMenuItem"
        Me.OptionsMenuToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.OptionsMenuToolStripMenuItem.Text = "Options Menu"
        '
        'SaveScreenAsToolStripMenuItem
        '
        Me.SaveScreenAsToolStripMenuItem.Name = "SaveScreenAsToolStripMenuItem"
        Me.SaveScreenAsToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.SaveScreenAsToolStripMenuItem.Text = "Save Screen As..."
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.ImageList1.ImageSize = New System.Drawing.Size(48, 48)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'ListBox1Names
        '
        Me.ListBox1Names.FormattingEnabled = True
        Me.ListBox1Names.ItemHeight = 18
        Me.ListBox1Names.Location = New System.Drawing.Point(491, 658)
        Me.ListBox1Names.Name = "ListBox1Names"
        Me.ListBox1Names.Size = New System.Drawing.Size(119, 58)
        Me.ListBox1Names.TabIndex = 4
        Me.ListBox1Names.Visible = False
        '
        'OpenContainingFolderToolStripMenuItem
        '
        Me.OpenContainingFolderToolStripMenuItem.Name = "OpenContainingFolderToolStripMenuItem"
        Me.OpenContainingFolderToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.OpenContainingFolderToolStripMenuItem.Text = "Open Containing Folder"
        '
        'NewScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1289, 740)
        Me.Controls.Add(Me.ListBox1Names)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.ListBox1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "NewScreen"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "New Screen"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStripListBox.ResumeLayout(False)
        Me.RightClickMenu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents RightClickMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenContainingFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DebugTestToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BREAKToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsMenuToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveScreenAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents DeleteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStripListBox As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewScreenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ListBox1Names As System.Windows.Forms.ListBox
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenContainingFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
