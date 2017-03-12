<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class NewScreen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.ContextMenuStripListBox = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenContainingFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.RightClickMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenContainingFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewScreenListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewBackupListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsMenuToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveScreenAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ListBox1Names = New System.Windows.Forms.ListBox()
        Me.BackupLoadListView = New System.Windows.Forms.ListView()
        Me.NameColumnHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.DateColumnHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.BackupContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ScreenSelectListView = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ScreenSelectContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RenameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewScreenToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ScreenSelectImageList = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.ContextMenuStripListBox.SuspendLayout()
        Me.RightClickMenu.SuspendLayout()
        Me.BackupContextMenuStrip.SuspendLayout()
        Me.ScreenSelectContextMenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListBox1
        '
        Me.ListBox1.AllowDrop = True
        Me.ListBox1.ContextMenuStrip = Me.ContextMenuStripListBox
        Me.ListBox1.Enabled = False
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.HorizontalScrollbar = True
        Me.ListBox1.ItemHeight = 18
        Me.ListBox1.Location = New System.Drawing.Point(1015, 652)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(130, 76)
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
        Me.DeleteToolStripMenuItem1.Size = New System.Drawing.Size(201, 22)
        Me.DeleteToolStripMenuItem1.Text = "Delete"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.OpenToolStripMenuItem.Text = "Open"
        '
        'OpenContainingFolderToolStripMenuItem
        '
        Me.OpenContainingFolderToolStripMenuItem.Name = "OpenContainingFolderToolStripMenuItem"
        Me.OpenContainingFolderToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.OpenContainingFolderToolStripMenuItem.Text = "Open Containing Folder"
        '
        'ViewScreenToolStripMenuItem
        '
        Me.ViewScreenToolStripMenuItem.Name = "ViewScreenToolStripMenuItem"
        Me.ViewScreenToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
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
        Me.ListView1.Location = New System.Drawing.Point(12, 12)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Scrollable = False
        Me.ListView1.Size = New System.Drawing.Size(1265, 634)
        Me.ListView1.TabIndex = 3
        Me.ListView1.TileSize = New System.Drawing.Size(77, 105)
        Me.ListView1.UseCompatibleStateImageBehavior = False
        '
        'RightClickMenu
        '
        Me.RightClickMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem1, Me.OpenContainingFileToolStripMenuItem, Me.DeleteToolStripMenuItem, Me.ViewListToolStripMenuItem, Me.ViewScreenListToolStripMenuItem, Me.ViewBackupListToolStripMenuItem, Me.OptionsMenuToolStripMenuItem, Me.SaveScreenAsToolStripMenuItem})
        Me.RightClickMenu.Name = "RightClickMenu"
        Me.RightClickMenu.Size = New System.Drawing.Size(215, 180)
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
        Me.ViewListToolStripMenuItem.Text = "List View"
        '
        'ViewScreenListToolStripMenuItem
        '
        Me.ViewScreenListToolStripMenuItem.Name = "ViewScreenListToolStripMenuItem"
        Me.ViewScreenListToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.ViewScreenListToolStripMenuItem.Text = "Screen Select"
        '
        'ViewBackupListToolStripMenuItem
        '
        Me.ViewBackupListToolStripMenuItem.Name = "ViewBackupListToolStripMenuItem"
        Me.ViewBackupListToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.ViewBackupListToolStripMenuItem.Text = "Backup Select"
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
        'ListBox1Names
        '
        Me.ListBox1Names.Enabled = False
        Me.ListBox1Names.FormattingEnabled = True
        Me.ListBox1Names.ItemHeight = 18
        Me.ListBox1Names.Location = New System.Drawing.Point(890, 652)
        Me.ListBox1Names.Name = "ListBox1Names"
        Me.ListBox1Names.Size = New System.Drawing.Size(119, 76)
        Me.ListBox1Names.TabIndex = 4
        Me.ListBox1Names.Visible = False
        '
        'BackupLoadListView
        '
        Me.BackupLoadListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.NameColumnHeader, Me.DateColumnHeader})
        Me.BackupLoadListView.ContextMenuStrip = Me.BackupContextMenuStrip
        Me.BackupLoadListView.Enabled = False
        Me.BackupLoadListView.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BackupLoadListView.Location = New System.Drawing.Point(149, 652)
        Me.BackupLoadListView.MultiSelect = False
        Me.BackupLoadListView.Name = "BackupLoadListView"
        Me.BackupLoadListView.Size = New System.Drawing.Size(144, 76)
        Me.BackupLoadListView.TabIndex = 6
        Me.BackupLoadListView.UseCompatibleStateImageBehavior = False
        Me.BackupLoadListView.View = System.Windows.Forms.View.Details
        Me.BackupLoadListView.Visible = False
        '
        'NameColumnHeader
        '
        Me.NameColumnHeader.Text = "Backup Name"
        Me.NameColumnHeader.Width = 200
        '
        'DateColumnHeader
        '
        Me.DateColumnHeader.Text = "Date"
        Me.DateColumnHeader.Width = 200
        '
        'BackupContextMenuStrip
        '
        Me.BackupContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem2, Me.CloseToolStripMenuItem})
        Me.BackupContextMenuStrip.Name = "BackupContextMenuStrip"
        Me.BackupContextMenuStrip.Size = New System.Drawing.Size(138, 48)
        '
        'OpenToolStripMenuItem2
        '
        Me.OpenToolStripMenuItem2.Name = "OpenToolStripMenuItem2"
        Me.OpenToolStripMenuItem2.Size = New System.Drawing.Size(137, 22)
        Me.OpenToolStripMenuItem2.Text = "Load"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.CloseToolStripMenuItem.Text = "View Screen"
        '
        'ScreenSelectListView
        '
        Me.ScreenSelectListView.Alignment = System.Windows.Forms.ListViewAlignment.Left
        Me.ScreenSelectListView.BackColor = System.Drawing.Color.DimGray
        Me.ScreenSelectListView.BackgroundImageTiled = True
        Me.ScreenSelectListView.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ScreenSelectListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.ScreenSelectListView.ContextMenuStrip = Me.ScreenSelectContextMenuStrip
        Me.ScreenSelectListView.Enabled = False
        Me.ScreenSelectListView.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ScreenSelectListView.ForeColor = System.Drawing.Color.White
        Me.ScreenSelectListView.LargeImageList = Me.ScreenSelectImageList
        Me.ScreenSelectListView.Location = New System.Drawing.Point(299, 652)
        Me.ScreenSelectListView.MultiSelect = False
        Me.ScreenSelectListView.Name = "ScreenSelectListView"
        Me.ScreenSelectListView.Size = New System.Drawing.Size(144, 76)
        Me.ScreenSelectListView.TabIndex = 7
        Me.ScreenSelectListView.TileSize = New System.Drawing.Size(77, 105)
        Me.ScreenSelectListView.UseCompatibleStateImageBehavior = False
        Me.ScreenSelectListView.Visible = False
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Screen Name"
        Me.ColumnHeader1.Width = 550
        '
        'ScreenSelectContextMenuStrip
        '
        Me.ScreenSelectContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem3, Me.AddScreenToolStripMenuItem, Me.RemoveScreenToolStripMenuItem, Me.RenameToolStripMenuItem, Me.ViewScreenToolStripMenuItem2})
        Me.ScreenSelectContextMenuStrip.Name = "BackupContextMenuStrip"
        Me.ScreenSelectContextMenuStrip.Size = New System.Drawing.Size(138, 114)
        '
        'OpenToolStripMenuItem3
        '
        Me.OpenToolStripMenuItem3.Name = "OpenToolStripMenuItem3"
        Me.OpenToolStripMenuItem3.Size = New System.Drawing.Size(137, 22)
        Me.OpenToolStripMenuItem3.Text = "Open"
        '
        'AddScreenToolStripMenuItem
        '
        Me.AddScreenToolStripMenuItem.Name = "AddScreenToolStripMenuItem"
        Me.AddScreenToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.AddScreenToolStripMenuItem.Text = "Add Screen"
        '
        'RemoveScreenToolStripMenuItem
        '
        Me.RemoveScreenToolStripMenuItem.Name = "RemoveScreenToolStripMenuItem"
        Me.RemoveScreenToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.RemoveScreenToolStripMenuItem.Text = "Remove"
        '
        'RenameToolStripMenuItem
        '
        Me.RenameToolStripMenuItem.Name = "RenameToolStripMenuItem"
        Me.RenameToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.RenameToolStripMenuItem.Text = "Rename"
        '
        'ViewScreenToolStripMenuItem2
        '
        Me.ViewScreenToolStripMenuItem2.Name = "ViewScreenToolStripMenuItem2"
        Me.ViewScreenToolStripMenuItem2.Size = New System.Drawing.Size(137, 22)
        Me.ViewScreenToolStripMenuItem2.Text = "View Screen"
        '
        'ScreenSelectImageList
        '
        Me.ScreenSelectImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.ScreenSelectImageList.ImageSize = New System.Drawing.Size(48, 48)
        Me.ScreenSelectImageList.TransparentColor = System.Drawing.Color.Transparent
        '
        'ToolTip1
        '
        Me.ToolTip1.AutomaticDelay = 200
        Me.ToolTip1.IsBalloon = True
        Me.ToolTip1.ShowAlways = True
        Me.ToolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.ToolTip1.ToolTipTitle = "Info"
        '
        'Timer1
        '
        Me.Timer1.Interval = 20000
        '
        'NewScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1289, 740)
        Me.Controls.Add(Me.ScreenSelectListView)
        Me.Controls.Add(Me.BackupLoadListView)
        Me.Controls.Add(Me.ListBox1Names)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.ListBox1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "NewScreen"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "New Screen"
        Me.ContextMenuStripListBox.ResumeLayout(False)
        Me.RightClickMenu.ResumeLayout(False)
        Me.BackupContextMenuStrip.ResumeLayout(False)
        Me.ScreenSelectContextMenuStrip.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents RightClickMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenContainingFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsMenuToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveScreenAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStripListBox As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewScreenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ListBox1Names As System.Windows.Forms.ListBox
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenContainingFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackupLoadListView As ListView
    Friend WithEvents NameColumnHeader As ColumnHeader
    Friend WithEvents DateColumnHeader As ColumnHeader
    Friend WithEvents BackupContextMenuStrip As ContextMenuStrip
    Friend WithEvents OpenToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewBackupListToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ScreenSelectListView As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ScreenSelectContextMenuStrip As ContextMenuStrip
    Friend WithEvents OpenToolStripMenuItem3 As ToolStripMenuItem
    Friend WithEvents ViewScreenToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents ViewScreenListToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddScreenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RemoveScreenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RenameToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ScreenSelectImageList As ImageList
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents Timer1 As Timer
End Class
