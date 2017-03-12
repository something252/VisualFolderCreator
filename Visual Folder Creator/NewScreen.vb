Imports IconHelper
Imports WinAPI
Imports System.IO
Imports Newtonsoft.Json

Public Class NewScreen
    Public backgroundPicture As Image
    Public startupSound As String = ""
    Public SaveFileActive As Boolean = False ' flag that determines whether a save file is currently being used and should be autosaved to on shutdown
    Public tmpSaveLocation As String = "" ' used for when My.Application.CommandLineArgs(0) is empty
    Public AutoSaveEnabled As Boolean = True
    Public exitingFirstSave As Boolean = False
    Public ThisScreenFile As ScreenFile
    Public saveBackupsPath As String = "" ' save backup file location
    Public saveFileNewName As String = ""
    Public saveBackupsMaximum As Integer = 60 ' maximum allowed save backups in the defined file
    Public saveBackupsIndex As Integer = -1 ' newest backup index
    Public realDeleteEnabled As Boolean = False ' determines whether items and their real system file/folder are both deleted when an item is deleted on screen
    Public embedIconsEnabled As Boolean = False ' flag for whether icons for each item are embedded in save file or not
    Public ScreenWidthScrollableExpander As Integer = 1, ScreenHeightScrollableExpander As Integer = 1, ScrollableExpander As Boolean = False
    Public QuitWithoutSaving As Boolean = False
    Public Shared SavingFlag As Boolean = False
    Private Shared MainForm As NewScreen

    Public Wrapper As New Simple3Des("hZOT\vSa&nFwjJCFz.jzL2TALX$qpU")
    Public PWrapper As Simple3Des = Nothing
    Public NewPWrapper As Simple3Des = Nothing
    Public PasswordEnabled As Boolean = False

    Private Sub NewScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Hide() ' hide screen until end of this loading
        Me.Icon = My.Resources.icon
        MainForm = Me

        If My.Application.CommandLineArgs.Count > 0 Then
            If LoadScreenFromFile(My.Application.CommandLineArgs(0)) Then ' load save file Then
                If ThisScreenFile.BackupFile = False Then
                    LoadSettings()
                End If
            Else
                MsgBox("File not recognized:" & vbNewLine & """" & My.Application.CommandLineArgs(0) & """", MsgBoxStyle.Critical)
                End ' abort execution
            End If
        Else
            ThisScreenFile = New ScreenFile
            ThisScreenFile.AddScreen(New ScreenInfo)
            ThisScreenFile.CurrentScreen.ItemLists.Add("New Screen", New List(Of ScreenInfo.ItemSettings))
            ThisScreenFile.CurrentScreen.CurrentItemListName = "New Screen"
            ViewBackupListToolStripMenuItem.Visible = False
            FirstTimeToolTip = True
        End If

        tmpScreenSize = Me.Size
        tmpScreenLocation = Me.Location

        ListView1.Dock = DockStyle.Fill ' fullscreen the desktop screen control

        RefreshBackgroundImage()

        screenWidth = ListView1.ClientSize.Width
        screenHeight = ListView1.ClientSize.Height

        If File.Exists(startupSound) Then
            Try
                Dim fileType As String = LCase(GetFileExtensionString(startupSound))
                If fileType = "mp3" OrElse fileType = "mid" OrElse fileType = "idi" Then
                    Dim tmp As New AudioFile(startupSound)
                    tmp.Play()
                ElseIf fileType = "wav" Then
                    My.Computer.Audio.Play(startupSound, AudioPlayMode.Background)
                End If
            Catch
            End Try
        End If

        Me.Show() ' show screen now that loading is done
    End Sub

    Dim FirstTimeToolTip As Boolean = False
    Private Sub NewScreen_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        If FirstTimeToolTip Then
            ToolTip1.Show("Drag and drop files/folders onto the screen to add and move around." & vbNewLine & vbNewLine _
                          & "Right click on screen for additional options.", ListView1, 20000)
        End If
    End Sub

    Private Sub NewScreen_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not FirstTimeSaveAsSuccess AndAlso Not QuitWithoutSaving Then

            If SaveSettings() = True Then
                e.Cancel = True ' abort closing of the program
            End If

        End If
    End Sub

    Private Sub ToolTip1_Popup(sender As Object, e As PopupEventArgs) Handles ToolTip1.Popup
        If FirstTimeToolTip Then
            FirstTimeToolTip = False
            Timer1.Start()
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ToolTip1.Active = False
        Timer1.Enabled = False
    End Sub

    Dim screenWidth As Integer, screenHeight As Integer
    Dim resizeInProgress As Boolean = False
    ''' <summary>
    ''' Resize the ListView's background image to the Listview's new size and replace it. (stretch)
    ''' </summary>
    Public Sub ResizeBackgroundImage()
        If (Not resizeInProgress) AndAlso (Not ListView1.ClientSize.Width = 0) AndAlso
            ((Not screenWidth = ListView1.ClientSize.Width) OrElse (Not screenHeight = ListView1.ClientSize.Height)) AndAlso
            (Not ListView1.ClientSize.Height = 0) Then
            RefreshBackgroundImage()
        End If
    End Sub

    ''' <summary>
    ''' refresh the background image by replacing it at the control's current dimensions
    ''' </summary>
    Public Sub RefreshBackgroundImage()
        If Not IsNothing(backgroundPicture) Then ' if background image is something
            Using bmp1 = New Bitmap(ListView1.ClientSize.Width, ListView1.ClientSize.Height) ' stretch the background image to fit the new size
                Using g1 = Graphics.FromImage(bmp1)
                    g1.DrawImage(backgroundPicture, 0, 0, bmp1.Width, bmp1.Height)
                    ListView1.BackgroundImage = CType(bmp1.Clone, Image)
                End Using
            End Using
            screenWidth = ListView1.ClientSize.Width
            screenHeight = ListView1.ClientSize.Height
        Else
            ListView1.BackgroundImage = Nothing
        End If
    End Sub

#Region "ListView Item Manipulation"
    ''' <summary>
    ''' Snaps an item to the grid given its current position in the list view.
    ''' </summary>
    Private Sub SnapItemToGrid(ByRef item As ListViewItem)
        Dim itemSize As Integer = 80 ' set to item size (only helps with determining midpoint of item dragged drop location for user control feel, not determining tile sizes)
        Dim tileSizeX As Integer = 90, tileSizeY As Integer = 92 ' y was 104
        Dim tileSizeXHalf As Integer = CInt(tileSizeX / 2), tileSizeYHalf As Integer = CInt(tileSizeY / 2)

        Dim edgePaddingX As Integer = 16, edgePaddingY As Integer = -4 ' extra room at edges of screen

        Dim tileCount As New Point(tileSizeXHalf + edgePaddingX, tileSizeYHalf + edgePaddingY)

        Dim YaxisAdjustInterval As Integer = 1, YaxisAdjust As Integer = 0 ' items are off by 1 down too much without this after first row on top most, better than using deci
        Dim XaxisAdjustInterval As Integer = 1, XaxisAdjust As Integer = 0 ' items are off by 1 left too much without this after first row on top most, better than using deci

        Dim itemLocation As New Point(CInt(item.Position.X + (itemSize / 2)), CInt(item.Position.Y + (itemSize / 2))) ' set to item position (which is top left) mid point (so add half item size)
        If itemLocation.X < 0 + edgePaddingX Then ' beyond left limit of listview
            itemLocation.X = 0 + edgePaddingX
        ElseIf itemLocation.X > ListView1.Width - tileSizeX Then ' beyond limit of listview width (furthest right)
            itemLocation.X = ListView1.Width - tileSizeXHalf
        End If
        If itemLocation.Y < 0 Then ' beyond upper limit of listview
            itemLocation.Y = 0
        ElseIf itemLocation.Y > ListView1.Height - tileSizeY Then ' beyond limit of listview Height (furthest down)
            itemLocation.Y = ListView1.Height - tileSizeYHalf
        End If

        While (True)
            If (itemLocation.X <= tileCount.X + tileSizeXHalf AndAlso itemLocation.X >= tileCount.X - tileSizeXHalf) AndAlso
               (itemLocation.Y <= tileCount.Y + tileSizeYHalf AndAlso itemLocation.Y >= tileCount.Y - tileSizeYHalf) Then

                InsertItemAtLocation(item, itemLocation, tileCount,
                                             itemSize, tileSizeXHalf, tileSizeYHalf, edgePaddingX, edgePaddingY,
                                             YaxisAdjustInterval, YaxisAdjust,
                                             XaxisAdjustInterval, XaxisAdjust,
                                             tileSizeX, tileSizeY)
                Exit While ' break

            Else ' increment and loop again
                tileCount = New Point(tileCount.X, tileCount.Y + tileSizeY) ' increment y axis downwards one tiles worth
                YaxisAdjust += YaxisAdjustInterval ' interval
                If tileCount.Y > ListView1.Size.Height - tileSizeXHalf Then ' hit screen height size limit
                    tileCount.Y = tileSizeYHalf + edgePaddingY ' set back to top most grid location (reset)
                    tileCount.X += tileSizeX ' and increment x over to next snap column
                    YaxisAdjust = 0 ' reset
                    XaxisAdjust += XaxisAdjustInterval ' interval
                End If
            End If
        End While
    End Sub

    ''' <summary>
    ''' Inserts given item at location and move all items in the way according to top alignment.
    ''' </summary>
    Private Sub InsertItemAtLocation(ByRef item As ListViewItem, ByVal itemLocation As Point, ByVal tileCount As Point,
                                     ByRef itemSize As Integer, ByRef tileSizeXHalf As Integer, ByRef tileSizeYHalf As Integer, ByRef edgePaddingX As Integer, ByRef edgePaddingY As Integer,
                                     ByVal YaxisAdjustInterval As Integer, ByVal YaxisAdjust As Integer,
                                     ByVal XaxisAdjustInterval As Integer, ByVal XaxisAdjust As Integer,
                                     ByVal tileSizeX As Integer, ByVal tileSizeY As Integer)

        For Each element As ListViewItem In ListView1.Items
            If Not element.Name = item.Name Then ' is not the same as recieved item
                Dim itemMidpoint As New Point(CInt(element.Position.X + (itemSize / 2) + XaxisAdjust), CInt(element.Position.Y + (itemSize / 2) + YaxisAdjust))

                If (itemMidpoint.X <= tileCount.X + tileSizeXHalf AndAlso itemMidpoint.X >= tileCount.X - tileSizeXHalf) AndAlso
                   (itemMidpoint.Y <= tileCount.Y + tileSizeYHalf AndAlso itemMidpoint.Y >= tileCount.Y - tileSizeYHalf) Then ' move the item in that area currently (via recursion)

                    ' insert at location
                    item.Position = New Point(CInt(tileCount.X - (itemSize / 2) + XaxisAdjust), CInt(tileCount.Y - (itemSize / 2) - YaxisAdjust)) ' snap to the grid location

                    ' increment tile to next one and check if not hit bounds
                    tileCount = New Point(tileCount.X, tileCount.Y + tileSizeY) ' increment y axis downwards one tiles worth
                    YaxisAdjust += YaxisAdjustInterval ' interval
                    If tileCount.Y > ListView1.Size.Height - tileSizeXHalf Then ' hit screen height size limit
                        tileCount.Y = tileSizeYHalf + edgePaddingY ' set back to top most grid location (reset)
                        tileCount.X += tileSizeX ' and increment x over to next snap column
                        YaxisAdjust = 0 ' reset
                        XaxisAdjust += XaxisAdjustInterval ' interval
                    End If

                    InsertItemAtLocation(element, itemLocation, tileCount,
                                                     itemSize, tileSizeXHalf, tileSizeYHalf, edgePaddingX, edgePaddingY,
                                                     YaxisAdjustInterval, YaxisAdjust,
                                                     XaxisAdjustInterval, XaxisAdjust,
                                                     tileSizeX, tileSizeY)
                    Exit Sub ' break sub
                End If
            End If
        Next
        ' insert at location (nothing blocking placement there found)
        item.Position = New Point(CInt(tileCount.X - (itemSize / 2) + XaxisAdjust), CInt(tileCount.Y - (itemSize / 2) - YaxisAdjust)) ' snap to the grid location
    End Sub

    ''' <summary>
    ''' moving icons around the view from the view (when you release a dragging item in the view)
    ''' </summary>
    Private Sub ListView1_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles ListView1.ItemDrag
        doOnceDragOver = True
        For Each item As ListViewItem In ListView1.SelectedItems
            'Dim lvi As ListViewItem = CType(e.Item, ListViewItem)
            ListView1.DoDragDrop(New DataObject("System.Windows.Forms.ListViewItem", item), DragDropEffects.Move)
            If item.Text = "*[Screen Expander]*" AndAlso item.Name = "" Then ' used for scrollable screen expanding
                ScrollableExpander = False
                item.Remove()
            Else
                SnapItemToGrid(item)
            End If
        Next
    End Sub

    Dim Pos As Point
    Dim difference As Point
    Dim doOnceDragOver As Boolean = True
    Dim CursorOffset As Size
    ''' <summary>
    ''' moving icons around the view from the view
    ''' </summary>
    Private Sub ListView1_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragOver
        If e.Data.GetDataPresent("System.Windows.Forms.ListViewItem") Then

            'Dim si As SCROLLINFO
            'si.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(si)
            'si.fMask = SIF_POS
            'Dim x As Integer = GetScrollInfo(ListView1.Handle, SB_HORZ, si)
            'si.nPos = si.nMax
            'x = SetScrollInfo(ListView1.Handle, SB_VERT, si, True)

            If doOnceDragOver Then
                doOnceDragOver = False
                CursorOffset = Size.Subtract(Cursor.Size, New Size(Cursor.HotSpot.X, Cursor.HotSpot.Y))
                Pos = ListView1.PointToClient(New Point(Cursor.Position.X - CursorOffset.Width, Cursor.Position.Y - CursorOffset.Height))
                difference = New Point(0, 0)
            Else
                Dim tmp As Point = ListView1.PointToClient(New Point(Cursor.Position.X - CursorOffset.Width, Cursor.Position.Y - CursorOffset.Height))
                difference = New Point(tmp.X - Pos.X, tmp.Y - Pos.Y)
                Pos = tmp
            End If

            For Each item As ListViewItem In ListView1.SelectedItems
                item.Position = New Point(item.Position.X + difference.X, item.Position.Y + difference.Y)

                e.Effect = DragDropEffects.Move
            Next
        End If
    End Sub

    ''' <summary>
    ''' when an outside file or folder or an icon from within the control dragged into/on the control
    ''' </summary>
    Private Sub ListView1_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All ' "specifying All means that dropping is enabled for any FileDrop"
        ElseIf e.Data.GetDataPresent("System.Windows.Forms.ListViewItem") Then
            e.Effect = DragDropEffects.Move
        End If
    End Sub


    ' getscrollinfo and setscrollinfo api
    'Declare Function GetScrollInfo Lib "user32" Alias "GetScrollInfo" (ByVal hWnd As IntPtr, ByVal n As Integer, ByRef lpScrollInfo As SCROLLINFO) As Integer
    'Declare Function SetScrollInfo Lib "user32" Alias "SetScrollInfo" (ByVal hWnd As IntPtr, ByVal n As Integer, ByRef lpcScrollInfo As SCROLLINFO, ByVal bool As Boolean) As Integer
    'Structure SCROLLINFO
    '    Dim cbSize As Integer
    '    Dim fMask As Integer
    '    Dim nMin As Integer
    '    Dim nMax As Integer
    '    Dim nPage As Integer
    '    Dim nPos As Integer
    '    Dim nTrackPos As Integer
    'End Structure
    '
    'Private Const SB_HORZ = 0
    'Private Const SB_VERT = 1
    '
    'Private Const SIF_RANGE = &H1
    'Private Const SIF_PAGE = &H2
    'Private Const SIF_POS = &H4
    'Private Const SIF_ALL = (SIF_RANGE Or SIF_PAGE Or SIF_POS)


    ''' <summary>
    ''' Used with the dragging and dropping of Windows items into the ListView.
    ''' </summary>
    Private Sub ListView1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim MyFiles() As String
            Dim i As Integer

            ' Assign the files to an array.
            MyFiles = CType(e.Data.GetData(DataFormats.FileDrop), String())
            ' Loop through the array and add the files to the list.
            For i = 0 To MyFiles.Length - 1
                'Dim item As ListViewItem = ListView1.FindItemWithText(GetFileDisplayString(MyFiles(i)))
                If (Not ImageList1.Images.ContainsKey(MyFiles(i))) Then
                    Dim newLVI = New ListViewItem
                    newLVI.Text = GetFileDisplayString(MyFiles(i)) ' visible label
                    newLVI.Name = MyFiles(i) ' hidden full path
                    ListView1.Items.Add(newLVI)

                    Dim shfi As New Shell32.SHFILEINFO
                    If File.Exists(MyFiles(i)) Then ' file path
                        Using bmp1 = New Bitmap(ImageList1.ImageSize.Width, ImageList1.ImageSize.Height) ' stretch the background image to fit the new size
                            Using g1 = Graphics.FromImage(bmp1)
                                Dim shortcutCheck As Boolean = CheckIfShortcut(MyFiles(i))
                                g1.DrawIcon(IconReader.ExtractIconFromFileEx(MyFiles(i), IconReader.IconSize.ExtraLarge, shortcutCheck, shfi), 0, 0)
                                If shortcutCheck Then ' add shortcut overlay if necessary
                                    g1.DrawImage(My.Resources.ShortcutOverlay, 0, (ImageList1.ImageSize.Height - My.Resources.ShortcutOverlay.Height),
                                                 My.Resources.ShortcutOverlay.Width, My.Resources.ShortcutOverlay.Height)
                                End If
                                ImageList1.Images.Add(MyFiles(i), CType(bmp1.Clone, Image))
                            End Using
                        End Using
                    ElseIf Directory.Exists(MyFiles(i)) Then ' folder path
                        ImageList1.Images.Add(MyFiles(i), IconReader.GetFolderIcon(IconReader.IconSize.ExtraLarge, IconReader.FolderType.Open, shfi))
                    Else ' unknown path given
                        MsgBox("Error Code: 156123", MsgBoxStyle.Critical, "Warning!")
                    End If

                    ListView1.Items(ListView1.Items.Count - 1).ImageKey = MyFiles(i)

                Else
                    If Directory.Exists(MyFiles(i)) Then
                        MsgBox("Folder already exists!" & vbNewLine & """" & MyFiles(i) & """", MsgBoxStyle.Exclamation, "Warning")
                    ElseIf File.Exists(MyFiles(i)) Then
                        MsgBox("File already exists!" & vbNewLine & """" & MyFiles(i) & """", MsgBoxStyle.Exclamation, "Warning")
                    Else
                        MsgBox("Unknown error!" & vbNewLine & """" & MyFiles(i) & """", MsgBoxStyle.Critical, "Warning")
                    End If
                End If
            Next
        End If
    End Sub
#End Region

    ''' <summary>
    ''' Open the containing folder of the selected ListView item(s)
    ''' </summary>
    Private Sub OpenContainingFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenContainingFileToolStripMenuItem.Click
        Dim list1 = ListView1.SelectedItems
        If ListView1.SelectedItems.Count > 0 Then
            For Each element As ListViewItem In list1
                If File.Exists(element.Name) OrElse Directory.Exists(element.Name) Then
                    Call Shell("explorer /select," & element.Name, AppWinStyle.NormalFocus) ' select in containing folder
                Else
                    MsgBox("File or folder no longer exists!" & vbNewLine & """" & element.Name & """", MsgBoxStyle.Critical, "Error")
                End If
            Next
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub NewScreen_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
        'ListView1.BeginUpdate()
        ResizeBackgroundImage() ' resize the background to fill new size
        'ListView1.EndUpdate()
    End Sub

    ''' <summary>
    ''' Execute the file (run) or folder (explorer)
    ''' </summary>
    Private Sub ListView1_ItemActivate(sender As Object, e As EventArgs) Handles ListView1.ItemActivate
        Dim list1 = ListView1.SelectedItems
        If ListView1.SelectedItems.Count > 0 Then
            For Each element As ListViewItem In list1
                If File.Exists(element.Name) OrElse Directory.Exists(element.Name) Then
                    Try
                        Process.Start(element.Name)
                    Catch ex As System.ComponentModel.Win32Exception
                        If CheckIfShortcut(element.Name) AndAlso ex.Message = "The operation was canceled by the user" Then
                            MsgBox("The shortcut " & "'" & GetFileDisplayString(element.Name) & ".lnk'" & " targets a no longer present file or folder.", MsgBoxStyle.Critical, "Error")
                        End If
                    End Try
                Else
                    MsgBox("File or folder no longer exists!" & vbNewLine & """" & element.Name & """", MsgBoxStyle.Critical, "Error")
                End If
            Next
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        DeleteSelectedItems()
    End Sub

    Private Sub ListView1_KeyDown(sender As Object, e As KeyEventArgs) Handles ListView1.KeyDown
        If e.KeyCode = Keys.Delete Then
            DeleteSelectedItems()
        End If
    End Sub

    ''' <summary>
    ''' Deletes the currently selected items in the screen.
    ''' </summary>
    Private Sub DeleteSelectedItems()
        Dim list1 = ListView1.SelectedItems
        If list1.Count > 0 Then
            Dim confirmed As Boolean = False
            For Each element As ListViewItem In list1
                If element.Text = "*[Screen Expander]*" AndAlso element.Name = "" Then ' used for scrollable screen expanding
                    ScrollableExpander = False
                    element.Remove()
                    Continue For
                End If

                If realDeleteEnabled AndAlso Not confirmed Then
                    If Not MsgBox("Are you sure you wish to delete the file(s)/folder(s) on your system as well?", MsgBoxStyle.YesNo, "Warning") = MsgBoxResult.Yes Then
                        Exit For
                    Else
                        confirmed = True
                    End If
                End If
                ImageList1.Images.RemoveByKey(element.Name)
                element.Remove()
                If realDeleteEnabled Then ' flag for deleting the real item on the OS also or not
                    Try
                        If File.Exists(element.Name) Then
                            My.Computer.FileSystem.DeleteFile(element.Name, FileIO.UIOption.AllDialogs,
                                                              FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                        ElseIf Directory.Exists(element.Name) Then
                            My.Computer.FileSystem.DeleteDirectory(element.Name, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
                        End If
                    Catch ex As Exception
                        MsgBox(ex.Message & vbNewLine & "Error code: 4843", MsgBoxStyle.Critical, "Error")
                    End Try
                End If
            Next
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub NewScreen_ResizeBegin(sender As Object, e As EventArgs) Handles MyBase.ResizeBegin
        resizeInProgress = True
    End Sub

    Private Sub NewScreen_ResizeEnd(sender As Object, e As EventArgs) Handles MyBase.ResizeEnd
        resizeInProgress = False
        ResizeBackgroundImage()
    End Sub

    Private Sub OptionsMenuToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OptionsMenuToolStripMenuItem.Click
        Options.Show()
        Options.TopMost = True ' will make top most no matter what without unsetting
        Options.TopMost = False ' now basically it was brought to focus now
    End Sub

    ''' <summary>
    ''' Open file(s).
    ''' </summary>
    Private Sub OpenToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem1.Click
        ' activate selected items (multiple open possible)
        ListView1_ItemActivate(sender, e)
    End Sub

    Dim tmpScreenSize As Size
    Private Sub NewScreen_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Normal Then
            tmpScreenSize = Me.Size ' save temp size for later settings saving
        End If
    End Sub

    Dim tmpScreenLocation As Point
    Private Sub NewScreen_Move(sender As Object, e As EventArgs) Handles MyBase.Move
        If Me.WindowState = FormWindowState.Normal Then
            tmpScreenLocation = Me.Location ' save temp location for later settings saving
        End If
    End Sub

    Dim itemPositions As List(Of Point)
    ''' <summary>
    ''' Store ListView item positions for later restoring
    ''' </summary>
    Public Sub StoreItemPositions()
        Dim list As New List(Of Point)
        For Each item As ListViewItem In ListView1.Items
            list.Add(item.Position)
        Next
        itemPositions = list
    End Sub


    Public saveAndRestoreItemPositions As Boolean = False
    ''' <summary>
    ''' Restore ListView item positions
    ''' </summary>
    Public Sub RestoreItemPositions()
        If saveAndRestoreItemPositions = True Then
            saveAndRestoreItemPositions = False
            For i As Integer = 0 To ListView1.Items.Count - 1
                ListView1.Items.Item(i).Position.ToString() ' this is needed to add a pause... otherwise items won't move to their newly set positions
                ListView1.Items.Item(i).Position = itemPositions.Item(i)
            Next
            itemPositions = Nothing
        End If
        ' when set to scrollable the item position will be off by some pixels but may return to normal again when you set back to non-scrollable
    End Sub

#Region "Saving and Loading"
    Public FirstTimeSaveAsSuccess As Boolean = False
    ''' <summary>
    ''' Saves various settings for this screen.
    ''' </summary>
    Public Function SaveSettings() As Boolean

        If Options.saveOnceFlag Then ' save once flag for disabling autosave
            If SaveFileActive Then
                CreateSaveFile()
            Else
                SaveScreen.Show()
                SaveScreen.SaveScreenButton.PerformClick()
            End If
        ElseIf SaveFileActive = False Then
            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
            Dim Response As MsgBoxResult = MsgBox("Save the screen before leaving?", MsgBoxStyle.YesNoCancel, "Warning")
            If Response = MsgBoxResult.Yes Then
                exitingFirstSave = True
                SaveScreen.Show()
                SaveScreen.SaveScreenButton.PerformClick()
                Return True ' abort closing of program
            ElseIf Response = MsgBoxResult.Cancel Then
                Return True
            End If
        ElseIf SaveFileActive = True AndAlso AutoSaveEnabled = True Then
            CreateSaveFile()
        End If
        Return False ' close the program
    End Function

    ''' <summary>
    ''' Determines where to create the save file, then calls sub to save to that location.
    ''' </summary>
    Public Sub CreateSaveFile()
        If My.Application.CommandLineArgs.Count > 0 Then
            If tmpSaveLocation = "" Then
                SaveToFile(My.Application.CommandLineArgs(0))
            Else
                SaveToFile(tmpSaveLocation)
            End If
        ElseIf Not tmpSaveLocation = "" Then
            SaveToFile(tmpSaveLocation)
        Else
            MsgBox("Save file location not set.", MsgBoxStyle.Critical, "Error")
        End If
    End Sub

    Private Sub SaveScreenAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveScreenAsToolStripMenuItem.Click
        SaveScreen.Show()
        SaveScreen.TopMost = True
    End Sub

#Region "ScreenFile Class"
    Public Class ScreenFile
        Public Screens As New List(Of ScreenInfo)
        Public Function CurrentScreen() As ScreenInfo
            If Screens.Count > 0 AndAlso Screens.Count >= CurrentIndex + 1 Then
                Return Screens.Item(CurrentIndex)
            Else
                Return Nothing
            End If
        End Function
        Public Property CurrentSubScreen() As List(Of ScreenInfo.ItemSettings)
            Get
                If Not SavingFlag AndAlso Screens.Count > 0 AndAlso Screens.Count >= CurrentIndex + 1 Then
                    Return (Screens.Item(CurrentIndex)).ItemLists((Screens.Item(CurrentIndex)).CurrentItemListName)
                Else
                    Return Nothing
                End If
            End Get
            Set(value As List(Of ScreenInfo.ItemSettings))
                If Not SavingFlag AndAlso Screens.Count > 0 AndAlso Screens.Count >= CurrentIndex + 1 Then
                    Screens.Item(CurrentIndex).ItemLists(Screens.Item(CurrentIndex).CurrentItemListName) = value
                End If
            End Set
        End Property
        Public Function AddSubScreen(subName As String) As Boolean
            If Not IsNothing(CurrentScreen()) Then
                If CurrentScreen.ItemLists.Keys.Contains(subName) Then
                    MsgBox("Name already exists.", MsgBoxStyle.Critical)
                    Return False
                Else
                    CurrentScreen.ItemLists.Add(subName, New List(Of ScreenInfo.ItemSettings))
                    Return True
                End If
            End If
            Return False
        End Function
        Public Function RemoveSubScreen(subName As String) As Boolean
            If Not IsNothing(CurrentScreen()) AndAlso Not IsNothing(CurrentScreen.ItemLists) Then
                If CurrentScreen.ItemLists.Keys.Contains(subName) Then
                    If CurrentScreen.ItemLists.Keys.Count <> 1 Then
                        If CurrentScreen.CurrentItemListName = subName Then
                            CurrentScreen.CurrentItemListName = ""
                        End If
                        CurrentScreen.ItemLists.Remove(subName)
                        Return True
                    Else
                        MsgBox("Cannot delete last screen.", MsgBoxStyle.Critical)
                        Return False
                    End If
                End If
            End If
            Return False
        End Function
        Public Sub LoadSubScreen(subName As String)
            If Not IsNothing(CurrentScreen()) AndAlso Not IsNothing(CurrentScreen.ItemLists) Then
                If CurrentScreen.ItemLists.Keys.Contains(subName) Then
                    Dim NewList As New List(Of ScreenInfo.ItemSettings)
                    NewList = MainForm.SaveCurrentListViewItems()
                    CurrentScreen.ItemLists(CurrentScreen.CurrentItemListName) = NewList
                    CurrentScreen.CurrentItemListName = subName
                    MainForm.Text = CurrentScreen.CurrentItemListName
                    MainForm.LoadListView()
                End If
            End If
        End Sub
        Public Function RenameSubScreen(fromSubName As String, toSubName As String) As Boolean
            If Not IsNothing(CurrentScreen()) AndAlso Not IsNothing(CurrentScreen.ItemLists) Then
                If CurrentScreen.ItemLists.Keys.Contains(fromSubName) AndAlso Not IsNothing(CurrentScreen.ItemLists(fromSubName)) Then
                    If Not CurrentScreen.ItemLists.Keys.Contains(toSubName) Then

                        AddSubScreen(toSubName)
                        CurrentScreen.ItemLists(toSubName) = CurrentScreen.ItemLists(fromSubName)
                        If fromSubName = CurrentScreen.CurrentItemListName Then
                            CurrentScreen.CurrentItemListName = toSubName
                        End If
                        CurrentScreen.ItemLists.Remove(fromSubName)
                        Return True

                    Else
                        MsgBox("Name already exists.", MsgBoxStyle.Critical)
                        Return False
                    End If
                End If
            End If
            Return False
        End Function

        Public Timestamps As New List(Of Date)

        Public BackupFile As Boolean = False
        Public Passworded As Boolean = False
        Public Decrypted As Boolean = False ' set to true when saved to file, used to verify proper decryption
        Public ScreensMaximum As Integer = 1
        Public CurrentIndex As Integer = -1 ' current element of backup list

        Public Sub AddScreen(save As ScreenInfo)
            If Screens.Count <= ScreensMaximum Then
                If ScreensMaximum <> 0 Then
                    If ScreensMaximum > Screens.Count AndAlso Screens.Count - 1 >= CurrentIndex AndAlso Screens.Count <> 0 Then
                        SortListAscending(Screens.Count)
                        CurrentIndex += 1
                    Else
                        IncrementBackupIndex()
                    End If
                    If Screens.Count >= CurrentIndex + 1 Then
                        Screens.Item(CurrentIndex) = save
                        Timestamps.Item(CurrentIndex) = Date.Now
                    Else
                        Screens.Add(save)
                        Timestamps.Add(Date.Now)
                    End If
                End If
            Else ' purge oldest backups (maximum backups lowered)
                SortListAscending(ScreensMaximum)
            End If
        End Sub

        Private Sub IncrementBackupIndex()
            If CurrentIndex >= ScreensMaximum - 1 Then
                CurrentIndex = 0 ' reset to 0 again
            Else
                CurrentIndex += 1
            End If
        End Sub

        Private Sub SortListAscending(LimitCount As Integer)
            Dim NewScreenList As New List(Of ScreenInfo)
            Dim NewTimeList As New List(Of Date)
            Dim c As Integer = LimitCount
            For i = CurrentIndex To 0 Step -1
                If c <> 0 Then
                    NewScreenList.Insert(0, Screens.Item(i))
                    NewTimeList.Insert(0, Timestamps.Item(i))
                Else
                    Exit For
                End If
                c -= 1
            Next
            If c <> 0 Then
                For i = Screens.Count - 1 To 0 Step -1
                    If c <> 0 Then
                        NewScreenList.Insert(0, Screens.Item(i))
                        NewTimeList.Insert(0, Timestamps.Item(i))
                    Else
                        Exit For
                    End If
                    c -= 1
                Next
            End If
            Screens = NewScreenList
            Timestamps = NewTimeList
            CurrentIndex = LimitCount - 1
        End Sub
    End Class

    Public Class ScreenInfo
        Public Settings As New GeneralSettings
        Public ItemLists As New Dictionary(Of String, List(Of ItemSettings))
        Public CurrentItemListName As String ' current or recent ItemList key

        Public Class GeneralSettings
            'Public ScreenName As String
            Public AutoSaveEnabled As Boolean
            Public BackgroundImage As String
            Public ScreenLocation As New Point
            Public ScreenSize As New Size
            'Public FirstTimeOpening As Boolean = False
            Public ScreenMaximized As Boolean
            Public ScreenScrollable As Boolean
            Public ScreenAutoArrange As Boolean
            Public ScreenAlignment As ListViewAlignment = ListViewAlignment.Top
            Public ScreenActivation As ItemActivation
            Public StartupSound As String
            'Public EmbedIcons As Boolean
            Public RealItemDelete As Boolean = False
            Public ScrollableExpander As Boolean
            Public ScreenWidthScrollableExpander As Integer
            Public ScreenHeightScrollableExpander As Integer
            Public BackupFileName As String
        End Class

        Public Class ItemSettings
            Public ListViewIndex As Integer
            Public Position As New Point
            Public ItemDisplayName As String
            Public ItemFullPath As String
            'Public IconImage As String
        End Class
    End Class
#End Region

    Public tempBackgroundLocation As String = "" ' used to store newly changed background info for saving to save file
    ''' <summary>
    ''' Saves the current settings of the program to save file format. (VFCScreen extension which is openable by this program)
    ''' </summary>
    ''' <param name="filePath">File path of the save file to write to.</param>
    Public Sub SaveToFile(filePath As String)
        Try
            Dim NewScreen As New ScreenInfo

            If AutoSaveEnabled = True Then
                NewScreen.Settings.AutoSaveEnabled = True
            Else
                NewScreen.Settings.AutoSaveEnabled = False
            End If
            NewScreen.Settings.BackgroundImage = tempBackgroundLocation
            NewScreen.Settings.ScreenLocation = tmpScreenLocation
            NewScreen.Settings.ScreenSize = tmpScreenSize
            If Me.WindowState = FormWindowState.Maximized Then
                NewScreen.Settings.ScreenMaximized = True ' maximized state
            Else
                NewScreen.Settings.ScreenMaximized = False ' normal state
            End If
            If ListView1.Scrollable = True Then
                NewScreen.Settings.ScreenScrollable = True
            Else
                NewScreen.Settings.ScreenScrollable = False
            End If
            If ListView1.AutoArrange = True Then
                NewScreen.Settings.ScreenAutoArrange = True
            Else
                NewScreen.Settings.ScreenAutoArrange = False
            End If
            NewScreen.Settings.ScreenAlignment = ListView1.Alignment
            NewScreen.Settings.ScreenActivation = ListView1.Activation
            NewScreen.Settings.StartupSound = startupSound

            NewScreen.Settings.BackupFileName = saveBackupsPath

            'NewSave.Settings.EmbedIcons = embedIconsEnabled
            NewScreen.Settings.RealItemDelete = realDeleteEnabled
            NewScreen.Settings.ScrollableExpander = ScrollableExpander
            NewScreen.Settings.ScreenWidthScrollableExpander = ScreenWidthScrollableExpander
            NewScreen.Settings.ScreenHeightScrollableExpander = ScreenHeightScrollableExpander

            NewScreen.CurrentItemListName = ThisScreenFile.CurrentScreen.CurrentItemListName
            ThisScreenFile.CurrentSubScreen = SaveCurrentListViewItems()
            For Each key As String In ThisScreenFile.CurrentScreen.ItemLists.Keys
                NewScreen.ItemLists.Add(key, ThisScreenFile.CurrentScreen.ItemLists(key))
            Next

            ' make in temp folder until done writing then move to real location
            Dim filePathTemp1 As String = Environment.GetEnvironmentVariable("TEMP")
            Dim filePathTemp2 As String = Environment.GetEnvironmentVariable("TMP")
            Dim filePathTempFinal As String
            If Directory.Exists(filePathTemp1) Then ' "C:\Temp" for example
                filePathTempFinal = filePathTemp1
            ElseIf Directory.Exists(filePathTemp2) Then
                filePathTempFinal = filePathTemp2
            Else
                Dim filePath3 As String = My.Computer.FileSystem.Drives.Item(0).Name
                filePath3 = filePath3.TrimEnd("\"c)
                If Directory.Exists(filePath3) Then
                    filePathTempFinal = filePath3
                Else
                    MsgBox("Could not write to disk, save failed. Error code: 2956239", MsgBoxStyle.Critical, "Critical Error")
                    SavingFlag = False
                    Exit Sub ' abort saving (should be changed to redo or something)
                End If
            End If

            Try
                Dim tmpStr() As String = Split(filePath, "\")

                ' add a backup if enabled
                If saveBackupsMaximum >= 0 AndAlso Directory.Exists(GetDirectoryString(saveBackupsPath)) Then
                    Try
                        Dim NewBackup As New ScreenFile
                        NewScreen.Settings.BackupFileName = saveBackupsPath

                        If File.Exists(saveBackupsPath) Then
                            Dim RawText As String = My.Computer.FileSystem.ReadAllText(saveBackupsPath)
                            If RawText <> "" Then
                                SavingFlag = True
                                Try
                                    If Not IsNothing(PWrapper) Then
                                        Dim PText As String = Wrapper.DecryptData(PWrapper.DecryptData(RawText))
                                        NewBackup = JsonConvert.DeserializeObject(Of ScreenFile)(PText)
                                        If Not NewBackup.Passworded Then
                                            NewBackup = JsonConvert.DeserializeObject(Of ScreenFile)(Wrapper.DecryptData(RawText))
                                        End If
                                    Else
                                        NewBackup = JsonConvert.DeserializeObject(Of ScreenFile)(Wrapper.DecryptData(RawText))
                                    End If
                                Catch ex As Exception
                                    MsgBox("Backup file not recognized." & vbNewLine & ex.Message)
                                End Try
                                SavingFlag = False
                            End If
                        End If

                        NewBackup.BackupFile = True
                        NewBackup.ScreensMaximum = saveBackupsMaximum
                        NewBackup.CurrentIndex = saveBackupsIndex
                        NewBackup.Decrypted = True
                        NewBackup.Passworded = PasswordEnabled
                        NewBackup.AddScreen(NewScreen)

                        Using fsB As System.IO.FileStream = New System.IO.FileStream(saveBackupsPath, System.IO.FileMode.Create)
                            If fsB.CanWrite Then
                                SavingFlag = True
                                If PasswordEnabled Then
                                    If Not IsNothing(NewPWrapper) Then
                                        WriteToFile(NewPWrapper.EncryptData(Wrapper.EncryptData(JsonConvert.SerializeObject(NewBackup))), fsB)
                                    ElseIf Not IsNothing(PWrapper) Then
                                        WriteToFile(PWrapper.EncryptData(Wrapper.EncryptData(JsonConvert.SerializeObject(NewBackup))), fsB)
                                    End If
                                Else
                                    WriteToFile(Wrapper.EncryptData(JsonConvert.SerializeObject(NewBackup)), fsB)
                                End If
                                SavingFlag = False
                            Else
                                MsgBox("Could not write save file.", MsgBoxStyle.Critical, "Error")
                            End If
                        End Using
                    Catch ex As Exception
                    End Try
                End If

                Dim NewScreenFile As New ScreenFile
                NewScreenFile.BackupFile = False
                NewScreenFile.Decrypted = True
                NewScreenFile.Passworded = PasswordEnabled
                'NewScreenFile.ScreensMaximum = 1
                'NewScreenFile.CurrentIndex = -1
                NewScreenFile.AddScreen(NewScreen)

                Using fs As System.IO.FileStream = New System.IO.FileStream(filePathTempFinal & "\" & tmpStr(tmpStr.Count - 1), System.IO.FileMode.Create)
                    If fs.CanWrite Then
                        SavingFlag = True
                        If PasswordEnabled Then
                            If Not IsNothing(NewPWrapper) Then
                                WriteToFile(NewPWrapper.EncryptData(Wrapper.EncryptData(JsonConvert.SerializeObject(NewScreenFile))), fs)
                            ElseIf Not IsNothing(PWrapper) Then
                                WriteToFile(PWrapper.EncryptData(Wrapper.EncryptData(JsonConvert.SerializeObject(NewScreenFile))), fs)
                            End If
                        Else
                            WriteToFile(Wrapper.EncryptData(JsonConvert.SerializeObject(NewScreenFile)), fs)
                        End If
                        SavingFlag = False
                    Else
                        MsgBox("Could not write save file.", MsgBoxStyle.Critical, "Error")
                    End If
                End Using
                My.Computer.FileSystem.CopyFile(filePathTempFinal & "\" & tmpStr(tmpStr.Count - 1), filePath, True) ' save file
                My.Computer.FileSystem.DeleteFile(filePathTempFinal & "\" & tmpStr(tmpStr.Count - 1)) ' delete the temp file

            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
                MsgBox("Could not finish writing to disk, save failed. Error code: 2956", MsgBoxStyle.Critical, "Critical Error")
                Exit Sub ' abort saving
            Finally
                SavingFlag = False
            End Try
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
        Finally
            SavingFlag = False
        End Try
    End Sub

    ''' <summary>
    ''' Writes a string to a given FileStream.
    ''' </summary>
    ''' <param name="str">String to be written using the given FileStream.</param>
    ''' <param name="fs">FileStream used for writing to file.</param>
    Private Shared Sub WriteToFile(ByRef str As String, ByRef fs As System.IO.FileStream)
        Dim encoding As New System.Text.UTF8Encoding
        fs.Write(encoding.GetBytes(str), 0, encoding.GetByteCount(str))
        fs.Flush() ' write stuff up to this point to the file
    End Sub

    ''' <summary>
    ''' Save current ListView items to given settings before closing.
    ''' </summary>
    Private Function SaveCurrentListViewItems() As List(Of ScreenInfo.ItemSettings)
        Dim newList As New List(Of ScreenInfo.ItemSettings)
        For i As Integer = 0 To ListView1.Items.Count - 1
            If ListView1.Items(i).Text = "*[Screen Expander]*" AndAlso ListView1.Items(i).Name = "" Then
                Dim newItem As New ScreenInfo.ItemSettings
                newItem.ListViewIndex = -99
                newItem.Position = ListView1.Items(i).Position
                newItem.ItemDisplayName = ListView1.Items(i).Text ' display name of item
                newItem.ItemFullPath = "" ' full path of item
                newList.Add(newItem)
            Else
                Dim newItem As New ScreenInfo.ItemSettings
                newItem.ListViewIndex = i
                newItem.Position = ListView1.Items(i).Position
                newItem.ItemDisplayName = ListView1.Items(i).Text ' display name of item
                newItem.ItemFullPath = ListView1.Items(i).Name  ' full path of item
                newList.Add(newItem)
            End If
        Next
        Return newList
    End Function

    Public TempText As String = ""
    Public Decrypted As Boolean = False
    Public FailedLoadDeserialize As Boolean = False
    ''' <summary>
    ''' Loads a screen from the given save file path.
    ''' </summary>
    ''' <param name="filePath">File path of a screen save file.</param>
    Private Function LoadScreenFromFile(filePath As String) As Boolean
        If File.Exists(filePath) Then
            TempText = My.Computer.FileSystem.ReadAllText(filePath, New System.Text.UTF8Encoding)
            If TempText <> "" Then

                Dim Data As String = Wrapper.DecryptData(TempText)
                SavingFlag = True
                Try
                    ThisScreenFile = JsonConvert.DeserializeObject(Of ScreenFile)(Data)
                Catch ex As Exception
                    FailedLoadDeserialize = True
                End Try
                SavingFlag = False
                If FailedLoadDeserialize OrElse Not ThisScreenFile.Decrypted Then ' passworded
                    PasswordEnabled = True
                    While (Not Decrypted)
                        LogOn.ShowDialog() ' repeat until correct password entered
                    End While
                End If

                If ThisScreenFile.BackupFile = False Then ' false by default, not a backup file if false
                    SaveFileActive = True ' save file is being used and should be saved to on shutdown etc. flag
                    ViewBackupListToolStripMenuItem.Visible = False
                    saveBackupsPath = ThisScreenFile.CurrentScreen.Settings.BackupFileName
                    If File.Exists(saveBackupsPath) Then
                        TempText = My.Computer.FileSystem.ReadAllText(saveBackupsPath, New System.Text.UTF8Encoding)
                        SavingFlag = True
                        Dim BackupTest2 As ScreenFile = Nothing
                        SavingFlag = True
                        Try
                            If PasswordEnabled Then
                                BackupTest2 = JsonConvert.DeserializeObject(Of ScreenFile)(Wrapper.DecryptData(PWrapper.DecryptData(TempText)))
                            Else
                                BackupTest2 = JsonConvert.DeserializeObject(Of ScreenFile)(Wrapper.DecryptData(TempText))
                            End If
                        Catch ex As Exception
                            BackupTest2 = Nothing
                            MsgBox("Backup file not recognized." & vbNewLine & ex.Message)
                        End Try
                        SavingFlag = False
                        If Not IsNothing(BackupTest2) Then
                            saveBackupsMaximum = BackupTest2.ScreensMaximum
                            saveBackupsIndex = BackupTest2.CurrentIndex
                        End If
                    End If
                Else ' backup file
                    ViewBackupListToolStripMenuItem.Visible = True
                    saveBackupsMaximum = ThisScreenFile.ScreensMaximum
                    saveBackupsIndex = ThisScreenFile.CurrentIndex
                    If ThisScreenFile.Screens.Count - 1 >= ThisScreenFile.CurrentIndex Then
                        saveBackupsPath = ThisScreenFile.CurrentScreen.Settings.BackupFileName
                        ShowBackupList()
                    Else
                        TempText = Nothing
                        SavingFlag = False
                        Return False
                    End If
                End If

                TempText = Nothing
                Return True
            Else
                MsgBox("File is blank, cannot load anything.", MsgBoxStyle.Critical)
                End
            End If
        End If
        Return False
    End Function

    ''' <summary>
    ''' Load Settings from currently active ScreenFile.
    ''' </summary>
    Private Sub LoadSettings()
        Try
            Me.Text = ThisScreenFile.CurrentScreen.CurrentItemListName
            AutoSaveEnabled = ThisScreenFile.CurrentScreen.Settings.AutoSaveEnabled

            If File.Exists(ThisScreenFile.CurrentScreen.Settings.BackgroundImage) Then
                backgroundPicture = Image.FromFile(ThisScreenFile.CurrentScreen.Settings.BackgroundImage)
            Else
                backgroundPicture = Nothing
            End If
            tempBackgroundLocation = ThisScreenFile.CurrentScreen.Settings.BackgroundImage

            startupSound = ThisScreenFile.CurrentScreen.Settings.StartupSound

            Me.Size = ThisScreenFile.CurrentScreen.Settings.ScreenSize
            Me.Location = ThisScreenFile.CurrentScreen.Settings.ScreenLocation

            Dim allScreens() As Windows.Forms.Screen = Screen.AllScreens
            Dim totalX As Integer = 0, currentY As Integer = 0, countTmp As Integer = 0
            For Each screen1 In allScreens ' test if screen location is out of bounds (usually only possible if saved on a now inactive monitor)

                totalX += screen1.Bounds.Width
                currentY = screen1.Bounds.Height

                ' check for negative X or Y on first screen
                If countTmp = 0 AndAlso (Me.Location.X < 0 AndAlso (Me.Location.X + Me.Size.Width <= 0) OrElse Me.Location.Y < 0) Then
                    Me.Location = New Point(0, 0) ' reset the location to upper left most position
                    Exit For
                End If

                If Not Me.Location.X > totalX AndAlso Me.Location.Y < currentY - 30 Then
                    Exit For ' doesn't need location resetting
                End If

                countTmp += 1
                If allScreens.Count = countTmp Then ' if last screen tested
                    Me.Location = New Point(0, 0) ' reset the location to upper left most position
                    Exit For
                End If
            Next

            If ThisScreenFile.CurrentScreen.Settings.ScreenMaximized Then
                Me.WindowState = FormWindowState.Maximized
            Else
                Me.WindowState = FormWindowState.Normal
            End If
            Me.ListView1.Scrollable = ThisScreenFile.CurrentScreen.Settings.ScreenScrollable
            Me.ListView1.AutoArrange = ThisScreenFile.CurrentScreen.Settings.ScreenAutoArrange
            Me.ListView1.Alignment = ThisScreenFile.CurrentScreen.Settings.ScreenAlignment
            Me.ListView1.Activation = ThisScreenFile.CurrentScreen.Settings.ScreenActivation

            realDeleteEnabled = ThisScreenFile.CurrentScreen.Settings.RealItemDelete

            ScrollableExpander = ThisScreenFile.CurrentScreen.Settings.ScrollableExpander
            ScreenWidthScrollableExpander = ThisScreenFile.CurrentScreen.Settings.ScreenWidthScrollableExpander
            ScreenHeightScrollableExpander = ThisScreenFile.CurrentScreen.Settings.ScreenHeightScrollableExpander

            LoadListView()

        Catch ex As Exception
            MsgBox("Critical error loading save file" & vbNewLine & ex.Message & vbNewLine & ex.Source, MsgBoxStyle.Critical, "Error")
            End
        End Try
    End Sub

    ''' <summary>
    ''' Load ListView Items from currently active List.
    ''' </summary>
    Public Sub LoadListView()
        Dim embeddedIconsFlag As Boolean = False
        ListView1.Items.Clear()
        ImageList1.Images.Clear()
        If Not IsNothing(ThisScreenFile.CurrentSubScreen) Then
            For i As Integer = 0 To ThisScreenFile.CurrentSubScreen.Count - 1
                Dim newItem As New ListViewItem

                newItem.Name = ThisScreenFile.CurrentSubScreen(i).ItemFullPath ' hidden full path
                newItem.Text = ThisScreenFile.CurrentSubScreen(i).ItemDisplayName ' visible label
                ListView1.Items.Add(newItem)

                ListView1.Items(i).Position = ThisScreenFile.CurrentSubScreen(i).Position

                ' get icon from file and not saved one
                Dim shfi As New Shell32.SHFILEINFO
                If File.Exists(newItem.Name) Then ' file path
                    If embeddedIconsFlag Then
                        ' do load from embedded icon here
                    Else
                        Using bmp1 = New Bitmap(ImageList1.ImageSize.Width, ImageList1.ImageSize.Height) ' stretch the background image to fit the new size
                            Using g1 = Graphics.FromImage(bmp1)
                                Dim shortcutCheck As Boolean = CheckIfShortcut(ThisScreenFile.CurrentSubScreen(i).ItemFullPath)
                                g1.DrawIcon(IconReader.ExtractIconFromFileEx(newItem.Name, IconReader.IconSize.ExtraLarge, shortcutCheck, shfi), 0, 0)
                                If shortcutCheck Then ' add shortcut overlay if necessary
                                    g1.DrawImage(My.Resources.ShortcutOverlay, 0, (ImageList1.ImageSize.Height - My.Resources.ShortcutOverlay.Height),
                                                 My.Resources.ShortcutOverlay.Width, My.Resources.ShortcutOverlay.Height)
                                End If
                                ImageList1.Images.Add(newItem.Name, CType(bmp1.Clone, Image))
                                ListView1.Items(ListView1.Items.Count - 1).ImageKey = newItem.Name
                            End Using
                        End Using
                    End If
                ElseIf Directory.Exists(newItem.Name) Then ' folder path
                    If embeddedIconsFlag Then
                        ' do load from embedded icon here
                    Else
                        ImageList1.Images.Add(newItem.Name, IconReader.GetFolderIcon(IconReader.IconSize.ExtraLarge, IconReader.FolderType.Open, shfi))
                        ListView1.Items(ListView1.Items.Count - 1).ImageKey = newItem.Name
                    End If
                Else ' unknown path given
                    If Not (newItem.Text = "*[Screen Expander]*" AndAlso newItem.Name = "") Then ' used for scrollable screen expanding
                        MsgBox("File or Folder is missing. It has been marked with an X icon.", MsgBoxStyle.Critical, "Warning!")
                        ImageList1.Images.Add(newItem.Name, My.Resources.NoIcon) ' "<NoIcon>"
                        ListView1.Items(ListView1.Items.Count - 1).ImageKey = newItem.Name
                    End If
                End If
            Next
        End If
    End Sub
#End Region

#Region "Helpful Functions"
    ''' <summary>
    ''' Gets a file display worthy string, without full path or file extension info.
    ''' </summary>
    ''' <param name="str">String to be altered</param>
    Public Shared Function GetFileDisplayString(str As String) As String
        Dim myStr() As String = Split(str, "\")
        Dim newStr As String = myStr(myStr.Length - 1)
        Dim myStr2() As String = Split(newStr, ".")
        Dim constructName As String = myStr2(0)
        For i As Integer = 1 To myStr2.Length - 2 ' don't add extension
            constructName = constructName & "." & myStr2(i)
        Next
        Return constructName
    End Function

    ''' <summary>
    ''' Returns file extension.
    ''' </summary>
    ''' <param name="str">File string to be checked</param>
    Public Shared Function GetFileExtensionString(str As String) As String
        Dim myStr() As String = Split(str, ".")
        If myStr.Count > 0 Then
            Return myStr(myStr.Count - 1)
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' Returns the containing directory of a given string. (Split on "\")
    ''' </summary>
    ''' <param name="str">File string to be checked</param>
    Public Shared Function GetDirectoryString(str As String) As String
        Dim myStr() As String = Split(str, "\")
        Dim newStr As String = ""
        For i As Integer = 0 To myStr.Count - 2
            newStr &= myStr(i) & "\"
        Next
        Return newStr
    End Function

    ''' <summary>
    ''' Get file/folder name
    ''' </summary>
    Public Shared Function GetFileName(str As String, Optional showExtension As Boolean = False) As String
        If Not str Is Nothing AndAlso str.Length > 1 Then
            Dim count As Integer = 0
            For i As Integer = str.Length - 1 To 0 Step -1
                If str(i) = "\"c AndAlso Not (str.Length - 1) = i Then
                    If showExtension Then
                        Return str.Substring(i + 1, count)
                    Else
                        Return RemoveExtension(str.Substring(i + 1, count))
                    End If
                ElseIf i = 0 Then
                    Return str
                End If
                count += 1
            Next
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Removes the extension from a string. (That was constructed by split on backslash)
    ''' </summary>
    Public Shared Function RemoveExtension(str As String) As String
        If Not str Is Nothing AndAlso str.Length > 0 Then
            Dim index As Integer = -999 ' store index of last period
            For i As Integer = (str.Length - 1) To 0 Step -1
                If str(i) = "." Then
                    index = i
                    Exit For ' extension found
                ElseIf str(i) = "\" Then
                    Exit For ' failed to find extension
                End If
            Next
            If Not index = -999 Then
                Dim newStr As String = ""
                For i As Integer = 0 To index - 1
                    newStr &= str(i)
                Next
                Return newStr
            End If
        End If
        Return str
    End Function

    ''' <summary>
    ''' Checks if a full path is a shortcut, or basically if it has the .lnk extension.
    ''' </summary>
    ''' <param name="str">Full path string</param>
    Private Shared Function CheckIfShortcut(ByRef str As String) As Boolean ' checks if a arrow should be put on an icon's image (if shortcut)
        Dim myStr() As String = Split(str, "\")
        Dim myStr2() As String = Split(myStr(myStr.Length - 1), ".")

        If myStr2(myStr2.Length - 1) = "lnk" Then
            Return True ' is a shortcut
        Else
            Return False
        End If
    End Function

    Private Shared Function StringToBase64(ByRef Str As String) As String
        If Not SavingFlag Then
            If Not IsNothing(Str) Then
                Dim b As Byte() = System.Text.Encoding.UTF8.GetBytes(Str)
                Return System.Convert.ToBase64String(b)
            Else
                Return ""
            End If
        Else
            Return Str
        End If
    End Function

    Private Shared Function Base64ToString(ByRef Str As String) As String
        If Not SavingFlag Then
            If Not IsNothing(Str) Then
                Dim b As Byte() = System.Convert.FromBase64String(Str)
                Dim str1 As String = System.Text.Encoding.UTF8.GetString(b)
                Return str1
            Else
                Return ""
            End If
        Else
            Return Str
        End If
    End Function

    Private Shared Function ImageToBase64(ByRef image As System.Drawing.Image, ByRef format As System.Drawing.Imaging.ImageFormat) As String
        Using ms As New MemoryStream()
            ' Convert Image to byte[]
            image.Save(ms, format)
            Dim imageBytes() As Byte = ms.ToArray()

            ' Convert byte[] to Base64 String
            Dim base64String As String = Convert.ToBase64String(imageBytes)

            Return base64String
        End Using
    End Function

    Private Shared Function Base64ToImage(ByRef base64String As String) As Image
        ' Convert Base64 String to byte[]
        Dim imageBytes() As Byte = Convert.FromBase64String(base64String)
        Using ms As New MemoryStream(imageBytes, 0, imageBytes.Length)
            ' Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length)
            Dim image As Image = Image.FromStream(ms, False)

            Return image
        End Using
    End Function
#End Region

#Region "Backup Load ListView"
    Dim backupSortColumn As Integer = -1
    Dim backupViewShown As Boolean = False
    Private BackupColumnSortInfo As ColumnSortInfo
    Private Sub BackupLoadListView_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles BackupLoadListView.ColumnClick
        If IsNothing(BackupColumnSortInfo) Then
            BackupColumnSortInfo = New ColumnSortInfo
        End If

        BackupColumnSortInfo.CurrentColumn = BackupLoadListView.Columns.Item(e.Column) ' set new column
        BackupColumnSortInfo.CurrentColumn.Text = BackupLoadListView.Columns.Item(e.Column).Text

        If Not backupViewShown Then
            ' Determine whether the column is the same as the last column clicked.
            If e.Column <> backupSortColumn Then
                ' Set the sort column to the new column.
                backupSortColumn = e.Column
                ' Set the sort order to ascending by default.
                BackupColumnSortInfo.CurrentSortOrder = SortOrder.Descending
            Else
                ' Determine what the last sort order was and change it.
                If BackupColumnSortInfo.CurrentSortOrder = SortOrder.Descending Then
                    BackupColumnSortInfo.CurrentSortOrder = SortOrder.Ascending
                Else
                    BackupColumnSortInfo.CurrentSortOrder = SortOrder.Descending
                End If
            End If
        End If
        backupViewShown = False

        BackupLoadListView.ListViewItemSorter = New ListViewComparer(BackupColumnSortInfo.CurrentColumn.Index, BackupColumnSortInfo.CurrentSortOrder)
        BackupLoadListView.Sort()
    End Sub

    ''' <summary>
    ''' Show list of backups available to load.
    ''' </summary>
    Private Sub ShowBackupList()
        ListView1.Visible = False
        BackupLoadListView.Enabled = True
        If Not BackupListReopened Then
            QuitWithoutSaving = True
        End If

        If BackupLoadListView.Items.Count > 0 Then
            BackupLoadListView.Items.Clear()
        End If

        Dim c As Integer = ThisScreenFile.Screens.Count
        For i As Integer = ThisScreenFile.CurrentIndex To 0 Step -1
            If c <> 0 Then
                Dim newItem As ListViewItem = BackupLoadListView.Items.Add(" [Backup File " & i + 1 & "]")
                newItem.Name = CStr(i)
                BackupLoadListView.Items.Item(newItem.Index).SubItems.Add(FormatDateTime(ThisScreenFile.Timestamps.Item(i)))
            Else
                Exit For
            End If
            c -= 1
        Next
        If c <> 0 Then
            For i = ThisScreenFile.Screens.Count - 1 To 0 Step -1
                If c <> 0 Then
                    Dim newItem As ListViewItem = BackupLoadListView.Items.Add(" [Backup File " & i + 1 & "]")
                    newItem.Name = CStr(i)
                    BackupLoadListView.Items.Item(newItem.Index).SubItems.Add(FormatDateTime(ThisScreenFile.Timestamps.Item(i)))
                Else
                    Exit For
                End If
                c -= 1
            Next
        End If

        Dim colIndex As Integer = 1
        If Not IsNothing(BackupColumnSortInfo) Then
            backupViewShown = True
        End If
        BackupLoadListView_ColumnClick(Nothing, New ColumnClickEventArgs(colIndex))

        BackupLoadListView.Dock = DockStyle.Fill
        BackupLoadListView.Visible = True
    End Sub

    Dim BackupListReopened As Boolean = False
    Private Sub OpenBackupList()
        ListView1.Visible = False
        MainForm.Text = "Backup List"
        BackupLoadListView.Visible = True
        BackupLoadListView.Enabled = True
        QuitWithoutSaving = False
        BackupListReopened = True
        ShowBackupList()
    End Sub

    Private Sub CloseBackupList()
        ListView1.Visible = True
        MainForm.Text = ThisScreenFile.CurrentScreen.CurrentItemListName
        BackupLoadListView.Items.Clear()
        BackupLoadListView.Visible = False
        BackupLoadListView.Enabled = False
        QuitWithoutSaving = False
        BackupListReopened = False
        RefreshBackgroundImage()
    End Sub

    Dim LoadedBackupOnce As Boolean = False ' prevent closing backup list until a backup is chosen
    Private Sub BackupListBackupLoad() Handles BackupLoadListView.DoubleClick
        If BackupLoadListView.SelectedItems.Count > 0 Then
            Dim index As Integer = CInt(BackupLoadListView.SelectedItems.Item(0).Name)
            ThisScreenFile.CurrentIndex = index
            LoadSettings()
            LoadedBackupOnce = True
            CloseBackupList()
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk)
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub OpenToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem2.Click
        BackupListBackupLoad()
    End Sub

    Private Sub ViewBackupListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewBackupListToolStripMenuItem.Click
        OpenBackupList()
    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        If ThisScreenFile.BackupFile AndAlso LoadedBackupOnce Then
            CloseBackupList()
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub
#End Region

#Region "ListBox Items View"
    ''' <summary>
    ''' Show the ListBox view of the ListView.
    ''' </summary>
    Private Sub ShowListBox() Handles ViewListToolStripMenuItem.Click
        MainForm.Text = "Item List"
        ListView1.Visible = False
        ListBox1Names.Enabled = True
        ListBox1.Enabled = True

        If ListBox1.Items.Count > 0 Then
            ListBox1.Items.Clear()
        End If
        If ListBox1Names.Items.Count > 0 Then
            ListBox1Names.Items.Clear()
        End If

        For Each item As ListViewItem In ListView1.Items
            ListBox1.Items.Add(item.Text)
            ListBox1Names.Items.Add(item.Name)
        Next

        ListBox1.Dock = DockStyle.Fill
        ListBox1.Visible = True
    End Sub

    Private Sub CloseListBox() Handles ViewScreenToolStripMenuItem.Click
        MainForm.Text = ThisScreenFile.CurrentScreen.CurrentItemListName
        ListBox1.Visible = False
        ListBox1Names.Enabled = False
        ListBox1.Enabled = False
        ListView1.Visible = True
    End Sub

    Private Sub ListBox1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Right Then
            ListBox1.SelectedIndex = ListBox1.IndexFromPoint(e.X, e.Y)
            ContextMenuStripListBox.Show(MousePosition)
        End If
    End Sub

    Private Sub ListBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ListBox1.KeyDown
        If e.KeyCode = Keys.Delete Then
            DeleteToolStripMenuItem1_Click()
        End If
    End Sub

    ''' <summary>
    ''' Delete the selected item in both the ListBox and ListView, uses another ListBox containing each item's full path to delete. (Single item delete in ListBox only)
    ''' </summary>
    Private Sub DeleteToolStripMenuItem1_Click() Handles DeleteToolStripMenuItem1.Click
        Dim selectedIndex As Integer = ListBox1.SelectedIndex
        If Not IsNothing(selectedIndex) AndAlso selectedIndex >= 0 Then

            Dim confirmed As Boolean = False
            If realDeleteEnabled Then
                Dim result As MsgBoxResult = MsgBox("Are you sure you wish to delete the file(s)/folder(s) on your system as well?", MsgBoxStyle.YesNoCancel, "Warning")
                If result = MsgBoxResult.Yes Then
                    confirmed = True
                ElseIf result = MsgBoxResult.No Then
                    confirmed = False
                Else ' cancel or X button clicked (abort)
                    Exit Sub
                End If
            End If

            Dim Name As String = ListBox1Names.Items.Item(selectedIndex).ToString

            ' delete item in listview
            Dim item As ListViewItem = ListView1.Items.Item(ListView1.Items.IndexOfKey(Name))
            ImageList1.Images.RemoveByKey(item.ImageKey)
            ListView1.Items.Item(item.Index).Remove()

            If confirmed Then ' flag for deleting the real item on the OS also or not
                Try
                    If File.Exists(CType(ListBox1Names.Items.Item(selectedIndex), String)) Then
                        My.Computer.FileSystem.DeleteFile(ListBox1Names.Items.Item(selectedIndex).ToString, FileIO.UIOption.AllDialogs,
                                                            FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                    ElseIf Directory.Exists(ListBox1Names.Items.Item(selectedIndex).ToString) Then
                        My.Computer.FileSystem.DeleteDirectory(ListBox1Names.Items.Item(selectedIndex).ToString, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
                    End If
                Catch ex As Exception
                    MsgBox(ex.Message & vbNewLine & "Error code: 4843", MsgBoxStyle.Critical, "Error")
                End Try
            End If

            ' delete item in listboxes
            ListBox1.Items.RemoveAt(selectedIndex)
            ListBox1Names.Items.RemoveAt(selectedIndex)
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        OpenListBoxItem()
    End Sub

    Private Sub OpenToolStripMenuItem_Click() Handles OpenToolStripMenuItem.Click
        OpenListBoxItem()
    End Sub

    Private Sub OpenListBoxItem()
        Dim selectedIndex As Integer = ListBox1.SelectedIndex
        If Not IsNothing(selectedIndex) AndAlso selectedIndex >= 0 Then
            If File.Exists(ListBox1Names.Items.Item(selectedIndex).ToString) OrElse Directory.Exists(ListBox1Names.Items.Item(selectedIndex).ToString) Then
                Try
                    Process.Start(ListBox1Names.Items.Item(selectedIndex).ToString)
                Catch ex As System.ComponentModel.Win32Exception
                    If CheckIfShortcut(ListBox1Names.Items.Item(selectedIndex).ToString) AndAlso ex.Message = "The operation was canceled by the user" Then
                        MsgBox("The shortcut " & "'" & GetFileDisplayString(ListBox1Names.Items.Item(selectedIndex).ToString) & ".lnk'" & " targets a no longer present file or folder.", MsgBoxStyle.Critical, "Error")
                    End If
                End Try
            Else
                MsgBox("File or folder no longer exists!" & vbNewLine & """" & ListBox1Names.Items.Item(selectedIndex).ToString & """", MsgBoxStyle.Critical, "Error")
            End If
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    ''' <summary>
    ''' Open containing folder for ListBox view.
    ''' </summary>
    Private Sub OpenContainingFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenContainingFolderToolStripMenuItem.Click
        Dim selectedIndex As Integer = ListBox1.SelectedIndex
        If Not IsNothing(selectedIndex) AndAlso selectedIndex >= 0 Then
            If File.Exists(ListBox1Names.Items.Item(selectedIndex).ToString) OrElse Directory.Exists(ListBox1Names.Items.Item(selectedIndex).ToString) Then
                Call Shell("explorer /select," & ListBox1Names.Items.Item(selectedIndex).ToString, AppWinStyle.NormalFocus) ' select in containing folder
            Else
                MsgBox("File or folder no longer exists!" & vbNewLine & """" & ListBox1Names.Items.Item(selectedIndex).ToString & """", MsgBoxStyle.Critical, "Error")
            End If
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub
#End Region

#Region "Screen List View"
    Private Sub ScreenSelectListView_DoubleClick(sender As Object, e As EventArgs) Handles ScreenSelectListView.DoubleClick
        OpenToolStripMenuItem3_Click()
    End Sub

    Private Sub OpenToolStripMenuItem3_Click() Handles OpenToolStripMenuItem3.Click
        If ScreenSelectListView.SelectedItems.Count > 0 Then
            ThisScreenFile.LoadSubScreen(ScreenSelectListView.SelectedItems(0).Text)
            CloseScreenList()
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk)
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub ScreenSelectListView_KeyDown(sender As Object, e As KeyEventArgs) Handles ScreenSelectListView.KeyDown
        If e.KeyCode = Keys.Delete Then
            RemoveScreenToolStripMenuItem_Click()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub RemoveScreenToolStripMenuItem_Click() Handles RemoveScreenToolStripMenuItem.Click
        If ScreenSelectListView.SelectedItems.Count > 0 Then
            ThisScreenFile.RemoveSubScreen(ScreenSelectListView.SelectedItems(0).Text)
            ScreenSelectListView.SelectedItems(0).Remove()
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub AddScreenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddScreenToolStripMenuItem.Click
        Dim Response As String = InputBox("Specify name")
        If Response.Length > 0 Then
            For i As Integer = 0 To ScreenSelectListView.Items.Count - 1
                If ScreenSelectListView.Items(i).Text = Response Then
                    MsgBox("Name already exists!", MsgBoxStyle.Critical)
                    Exit Sub
                End If
            Next
            ScreenSelectListView.Items.Add(Response)
            ScreenSelectListView.Items(ScreenSelectListView.Items.Count - 1).ImageKey = "Folder"
            ThisScreenFile.AddSubScreen(Response)
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub RenameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RenameToolStripMenuItem.Click
        If ScreenSelectListView.SelectedItems.Count > 0 Then
            Dim NewName As String = InputBox("Rename", , ScreenSelectListView.SelectedItems(0).Text)
            If NewName.Length > 0 Then
                If ThisScreenFile.RenameSubScreen(ScreenSelectListView.SelectedItems(0).Text, NewName) Then
                    ScreenSelectListView.SelectedItems(0).Text = NewName
                End If
            Else
                My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
            End If
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub ViewScreenListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewScreenListToolStripMenuItem.Click
        OpenScreenList()
    End Sub

    Private Sub ViewScreenToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ViewScreenToolStripMenuItem2.Click
        CloseScreenList()
    End Sub

    Private Sub OpenScreenList()
        ListView1.Visible = False
        Dim shfi As New Shell32.SHFILEINFO
        ScreenSelectImageList.Images.Add("Folder", IconReader.GetFolderIcon(IconReader.IconSize.ExtraLarge, IconReader.FolderType.Open, shfi))
        MainForm.Text = "Screen List"
        ScreenSelectListView.Dock = DockStyle.Fill
        ScreenSelectListView.Visible = True
        ScreenSelectListView.Enabled = True
        ShowScreenList()
    End Sub

    Private Sub CloseScreenList()
        If Not ThisScreenFile.CurrentScreen.CurrentItemListName = "" Then
            ListView1.Visible = True
            MainForm.Text = ThisScreenFile.CurrentScreen.CurrentItemListName
            ScreenSelectListView.Items.Clear()
            ScreenSelectImageList.Images.Clear()
            ScreenSelectListView.Visible = False
            ScreenSelectListView.Enabled = False
        Else
            MsgBox("Open a screen first", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub ShowScreenList()
        If Not IsNothing(ThisScreenFile.CurrentScreen) Then
            For i As Integer = 0 To ThisScreenFile.CurrentScreen.ItemLists.Count - 1
                ScreenSelectListView.Items.Add(ThisScreenFile.CurrentScreen.ItemLists.ElementAt(i).Key)
                ScreenSelectListView.Items(ScreenSelectListView.Items.Count - 1).ImageKey = "Folder"
            Next
        End If
    End Sub
#End Region

#Region "ListView Column Sorter"
    ''' <summary>
    ''' Contains the current way items are sorted and the column being sorted
    ''' </summary>
    Class ColumnSortInfo
        Public CurrentColumn As ColumnHeader ' contains the column
        Public CurrentSortOrder As SortOrder = SortOrder.None ' contains the current sort
    End Class

    ''' <summary>
    ''' Implements a comparer for ListView columns.
    ''' </summary>
    Private Class ListViewComparer
        Implements IComparer

        Private columnNumber As Integer
        Private CurrentSortOrder As SortOrder

        Public Sub New(ByVal columnNumberTmp As Integer, ByVal CurrSortOrderTmp As SortOrder)
            columnNumber = columnNumberTmp
            CurrentSortOrder = CurrSortOrderTmp
        End Sub

        ''' <summary>
        ''' Compare the items in the appropriate column.
        ''' </summary>
        Public Function Compare(ByVal obj1 As Object, ByVal obj2 As Object) As Integer Implements System.Collections.IComparer.Compare
            Dim Item1 As ListViewItem = DirectCast(obj1, ListViewItem)
            Dim Item2 As ListViewItem = DirectCast(obj2, ListViewItem)

            ' get the sub-item values
            Dim X As String
            If Item1.SubItems.Count <= columnNumber Then
                X = ""
            Else
                X = Item1.SubItems(columnNumber).Text
            End If

            Dim Y As String
            If Item2.SubItems.Count <= columnNumber Then
                Y = ""
            Else
                Y = Item2.SubItems(columnNumber).Text
            End If

            ' compare them
            If CurrentSortOrder = SortOrder.Ascending Then
                If IsNumeric(X) AndAlso IsNumeric(Y) Then
                    Return Val(X).CompareTo(Val(Y))
                ElseIf IsDate(X) AndAlso IsDate(Y) Then
                    Return DateTime.Parse(X).CompareTo(DateTime.Parse(Y))
                Else
                    Return String.Compare(X, Y)
                End If
            Else
                If IsNumeric(X) AndAlso IsNumeric(Y) Then
                    Return Val(Y).CompareTo(Val(X))
                ElseIf IsDate(X) AndAlso IsDate(Y) Then
                    Return DateTime.Parse(Y).CompareTo(DateTime.Parse(X))
                Else
                    Return String.Compare(Y, X)
                End If
            End If
        End Function
    End Class
#End Region
End Class