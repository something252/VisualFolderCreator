Imports IconHelper
Imports WinAPI
Imports System.IO

Public Class NewScreen
    Public backgroundPicture As Image
    Public startupSound As String = ""
    Private dragging As Boolean
    Private beginX, beginY As Integer
    Public SaveFileActive As Boolean = False ' flag that determines whether a save file is currently being used and should be autosaved to on shutdown
    Public tmpSaveLocation As String = "" ' used for when My.Application.CommandLineArgs(0) is empty (newly saved but not yet shutdown screen that tries to save again before shutting down)
    Public AutoSaveEnabled As Boolean = True
    Public exitingFirstSave As Boolean = False
    Public saveBackupsDirectory As String = "" ' save backup location
    Public saveBackupsMaximum As Integer = 60 ' maximum allowed save backups in the defined directory
    Private saveBackupsCount As Integer = 0 ' current number for the naming of the next backup save
    Public realDeleteEnabled As Boolean = False ' determines whether items and their real system file/folder are both deleted when an item is deleted on screen
    Public embedIconsEnabled As Boolean = False ' flag for whether icons for each item are embedded in save file or not
    Public ScreenWidthScrollableExpander As Integer = 1, ScreenHeightScrollableExpander As Integer = 1, ScrollableExpander As Boolean = False

    Private Sub NewScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Hide() ' hide screen until end of this loading

        Me.Icon = My.Resources.icon

        If My.Application.CommandLineArgs.Count > 0 Then
            SaveFileActive = True ' save file is being used and should be saved to on shutdown etc. flag
            Dim Arguments = My.Application.CommandLineArgs ' My.Application.CommandlineArgs(0) to retrieve full file path of file opened which opened this program (file type)
            LoadScreenFromFile(My.Application.CommandLineArgs(0)) ' load save file
        End If

        tmpScreenSize = Me.Size
        tmpScreenLocation = Me.Location

        ListView1.Dock = DockStyle.Fill ' fullscreen the desktop screen control

        RefreshBackgroundImage()

        screenWidth = ListView1.ClientSize.Width
        screenHeight = ListView1.ClientSize.Height

        If File.Exists(startupSound) Then
            Dim tmp As New AudioFile(startupSound)
            tmp.Play()
            'My.Computer.Audio.Play(startupSound, AudioPlayMode.Background) ' wave only
        End If
        Me.Show() ' show screen now that loading is done
    End Sub


    Dim screenWidth As Integer, screenHeight As Integer
    Dim resizeInProgress As Boolean = False
    ''' <summary>
    ''' Resize the ListView's background image to the Listview's new size and replace it. (stretch)
    ''' </summary>
    Public Sub ResizeBackgroundImage()
        If (Not resizeInProgress) AndAlso (Not ListView1.ClientSize.Width = 0) AndAlso _
            ((Not screenWidth = ListView1.ClientSize.Width) OrElse (Not screenHeight = ListView1.ClientSize.Height)) AndAlso _
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
                    ListView1.BackgroundImage = bmp1.Clone
                End Using
            End Using
            screenWidth = ListView1.ClientSize.Width
            screenHeight = ListView1.ClientSize.Height
        End If
    End Sub

#Region "MovingBox"
    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        MouseDownIcon(sender, e)
    End Sub

    Private Sub PictureBox1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        MouseMoveIcon(sender, e)
    End Sub

    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        MouseUpIcon()
    End Sub
    ' clicking and holding down of icon has begun
    Private Sub MouseDownIcon(ByRef sender As Object, ByRef e As System.Windows.Forms.MouseEventArgs)
        dragging = True
        beginX = e.X
        beginY = e.Y
    End Sub
    ' moving the icon around the screen
    Private Sub MouseMoveIcon(ByRef sender As Object, ByRef e As System.Windows.Forms.MouseEventArgs)
        If dragging = True Then
            sender.Location = New Point(sender.Location.X + e.X - beginX, sender.Location.Y + e.Y - beginY)
            Me.Refresh()
        End If
    End Sub
    ' ends the icon moving around
    Private Sub MouseUpIcon()
        dragging = False
    End Sub
#End Region
    
    ''' <summary>
    ''' Snaps an item to the grid given its current position in the list view.
    ''' </summary>
    Private Sub SnapItemToGrid(ByRef item As ListViewItem)
        Dim itemSize As Integer = 80 ' set to item size (only helps with determining midpoint of item dragged drop location for user control feel, not determining tile sizes)
        Dim tileSizeX As Integer = 90, tileSizeY As Integer = 92 ' y was 104
        Dim tileSizeXHalf As Integer = tileSizeX / 2, tileSizeYHalf As Integer = tileSizeY / 2

        Dim edgePaddingX As Integer = 16, edgePaddingY As Integer = -4 ' extra room at edges of screen

        Dim tileCount As New Point(tileSizeXHalf + edgePaddingX, tileSizeYHalf + edgePaddingY)

        Dim YaxisAdjustInterval As Integer = 1, YaxisAdjust As Integer = 0 ' items are off by 1 down too much without this after first row on top most, better than using deci
        Dim XaxisAdjustInterval As Integer = 1, XaxisAdjust As Integer = 0 ' items are off by 1 left too much without this after first row on top most, better than using deci

        Dim itemLocation As New Point(item.Position.X + (itemSize / 2), item.Position.Y + (itemSize / 2)) ' set to item position (which is top left) mid point (so add half item size)
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

                InsertItemAtLocation(item, itemLocation, tileCount, _
                                             itemSize, tileSizeXHalf, tileSizeYHalf, edgePaddingX, edgePaddingY, _
                                             YaxisAdjustInterval, YaxisAdjust, _
                                             XaxisAdjustInterval, XaxisAdjust, _
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
    Private Sub InsertItemAtLocation(ByRef item As ListViewItem, ByVal itemLocation As Point, ByVal tileCount As Point, _
                                     ByRef itemSize As Integer, ByRef tileSizeXHalf As Integer, ByRef tileSizeYHalf As Integer, ByRef edgePaddingX As Integer, ByRef edgePaddingY As Integer, _
                                     ByVal YaxisAdjustInterval As Integer, ByVal YaxisAdjust As Integer, _
                                     ByVal XaxisAdjustInterval As Integer, ByVal XaxisAdjust As Integer, _
                                     ByVal tileSizeX As Integer, ByVal tileSizeY As Integer)

        For Each element As ListViewItem In ListView1.Items
            If Not element.Name = item.Name Then ' is not the same as recieved item
                Dim itemMidpoint As New Point(element.Position.X + (itemSize / 2) + XaxisAdjust, element.Position.Y + (itemSize / 2) + YaxisAdjust)

                If (itemMidpoint.X <= tileCount.X + tileSizeXHalf AndAlso itemMidpoint.X >= tileCount.X - tileSizeXHalf) AndAlso
                   (itemMidpoint.Y <= tileCount.Y + tileSizeYHalf AndAlso itemMidpoint.Y >= tileCount.Y - tileSizeYHalf) Then ' move the item in that area currently (via recursion)

                    ' insert at location
                    item.Position = New Point(tileCount.X - (itemSize / 2) + XaxisAdjust, tileCount.Y - (itemSize / 2) - YaxisAdjust) ' snap to the grid location

                    ' increment tile to next one and check if not hit bounds
                    tileCount = New Point(tileCount.X, tileCount.Y + tileSizeY) ' increment y axis downwards one tiles worth
                    YaxisAdjust += YaxisAdjustInterval ' interval
                    If tileCount.Y > ListView1.Size.Height - tileSizeXHalf Then ' hit screen height size limit
                        tileCount.Y = tileSizeYHalf + edgePaddingY ' set back to top most grid location (reset)
                        tileCount.X += tileSizeX ' and increment x over to next snap column
                        YaxisAdjust = 0 ' reset
                        XaxisAdjust += XaxisAdjustInterval ' interval
                    End If

                    InsertItemAtLocation(element, itemLocation, tileCount, _
                                                     itemSize, tileSizeXHalf, tileSizeYHalf, edgePaddingX, edgePaddingY, _
                                                     YaxisAdjustInterval, YaxisAdjust, _
                                                     XaxisAdjustInterval, XaxisAdjust, _
                                                     tileSizeX, tileSizeY)
                    Exit Sub ' break sub
                End If
            End If
        Next
        ' insert at location (nothing blocking placement there found)
        item.Position = New Point(tileCount.X - (itemSize / 2) + XaxisAdjust, tileCount.Y - (itemSize / 2) - YaxisAdjust) ' snap to the grid location
    End Sub


    ''' <summary>
    ''' moving icons around the view from the view (when you release a dragging item in the view)
    ''' </summary>
    Private Sub ListView1_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles ListView1.ItemDrag
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

    ''' <summary>
    ''' moving icons around the view from the view
    ''' </summary>
    Private Sub ListView1_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragOver
        If e.Data.GetDataPresent("System.Windows.Forms.ListViewItem") Then

            Dim si As SCROLLINFO
            si.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(si)
            si.fMask = SIF_POS
            Dim x As Integer = GetScrollInfo(ListView1.Handle, SB_HORZ, si)
            'MsgBox(x)
            si.nPos = si.nMax
            'x = SetScrollInfo(ListView1.Handle, SB_VERT, si, True)

            Dim Offset As Size = Size.Subtract(Cursor.Size, New Size(Cursor.HotSpot.X, Cursor.HotSpot.Y))

            For Each item As ListViewItem In ListView1.SelectedItems
                'Dim Offset2 As Point = Size.Subtract(New Point(item.Position.X, item.Position.Y), Offset)
                'Dim lvi As ListViewItem = CType(e.Data.GetData("System.Windows.Forms.ListViewItem"), ListViewItem)
                'Debugger.Break()
                'If item.Position.X > My Then
                item.Position = Point.Subtract(ListView1.PointToClient(New Point(e.X, e.Y)), Offset)

                ' Debugger.Break()
                'e.Effect = DragDropEffects.Move
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

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' getscrollinfo and setscrollinfo api
    Declare Function GetScrollInfo Lib "user32" Alias "GetScrollInfo" (ByVal hWnd As IntPtr, ByVal n As Integer, ByRef lpScrollInfo As SCROLLINFO) As Integer
    Declare Function SetScrollInfo Lib "user32" Alias "SetScrollInfo" (ByVal hWnd As IntPtr, ByVal n As Integer, ByRef lpcScrollInfo As SCROLLINFO, ByVal bool As Boolean) As Integer
    Structure SCROLLINFO
        Dim cbSize As Integer
        Dim fMask As Integer
        Dim nMin As Integer
        Dim nMax As Integer
        Dim nPage As Integer
        Dim nPos As Integer
        Dim nTrackPos As Integer
    End Structure

    Private Const SB_HORZ = 0
    Private Const SB_VERT = 1

    Private Const SIF_RANGE = &H1
    Private Const SIF_PAGE = &H2
    Private Const SIF_POS = &H4
    Private Const SIF_ALL = (SIF_RANGE Or SIF_PAGE Or SIF_POS)
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    '////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ''' <summary>
    ''' Used with the dragging and dropping of Windows items into the ListView.
    ''' </summary>
    Private Sub ListView1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim MyFiles() As String
            Dim i As Integer

            ' Assign the files to an array.
            MyFiles = e.Data.GetData(DataFormats.FileDrop)
            ' Loop through the array and add the files to the list.
            For i = 0 To MyFiles.Length - 1
                Dim item As ListViewItem = ListView1.FindItemWithText(GetFileDisplayString(MyFiles(i)))
                If (Not ImageList1.Images.ContainsKey(MyFiles(i))) Then
                    Dim newLVI = New ListViewItem
                    newLVI.Text = GetFileDisplayString(MyFiles(i)) ' visible label
                    newLVI.Name = MyFiles(i) ' hidden full path
                    ListView1.Items.Add(newLVI)

                    Dim shfi As Shell32.SHFILEINFO
                    If File.Exists(MyFiles(i)) Then ' file path
                        Using bmp1 = New Bitmap(ImageList1.ImageSize.Width, ImageList1.ImageSize.Height) ' stretch the background image to fit the new size
                            Using g1 = Graphics.FromImage(bmp1)
                                Dim shortcutCheck As Boolean = CheckIfShortcut(MyFiles(i))
                                g1.DrawIcon(IconReader.ExtractIconFromFileEx(MyFiles(i), IconReader.IconSize.ExtraLarge, shortcutCheck, shfi), 0, 0)
                                If shortcutCheck Then ' add shortcut overlay if necessary
                                    g1.DrawImage(My.Resources.ShortcutOverlay, 0, (ImageList1.ImageSize.Height - My.Resources.ShortcutOverlay.Height), _
                                                 My.Resources.ShortcutOverlay.Width, My.Resources.ShortcutOverlay.Height)
                                End If
                                ImageList1.Images.Add(MyFiles(i), bmp1.Clone)
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
    '////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ''' <summary>
    ''' Gets a file display worthy string, without full path or file extension info.
    ''' </summary>
    ''' <param name="str">String to be altered</param>
    Public Function GetFileDisplayString(str As String) As String
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
    ''' Returns a files extension as string.
    ''' </summary>
    ''' <param name="str">File string to be checked</param>
    Public Function GetFileExtensionString(str As String) As String
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
    Public Function GetDirectoryString(str As String) As String
        Dim myStr() As String = Split(str, "\")
        Dim newStr As String = ""
        For i As Integer = 0 To myStr.Count - 2
            newStr &= myStr(i) & "\"
        Next
        Return newStr
    End Function

    ''' <summary>
    ''' Checks if a full path is a shortcut, or basically if it has the .lnk extension.
    ''' </summary>
    ''' <param name="str">Full path string</param>
    Private Function CheckIfShortcut(ByRef str As String) As Boolean ' checks if a arrow should be put on an icon's image (if shortcut)
        Dim myStr() As String = Split(str, "\")
        Dim myStr2() As String = Split(myStr(myStr.Length - 1), ".")

        If myStr2(myStr2.Length - 1) = "lnk" Then
            Return True ' is a shortcut
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Open the containing folder of the selected ListView item(s)
    ''' </summary>
    Private Sub OpenContainingFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenContainingFileToolStripMenuItem.Click
        Dim list1 = ListView1.SelectedItems
        For Each element As ListViewItem In list1
            If File.Exists(element.Name) OrElse Directory.Exists(element.Name) Then
                Call Shell("explorer /select," & element.Name, AppWinStyle.NormalFocus) ' select in containing folder
            Else
                MsgBox("File or folder no longer exists!" & vbNewLine & """" & element.Name & """", MsgBoxStyle.Critical, "Error")
            End If
        Next
    End Sub

    Private Sub NewScreen_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
        'ListView1.BeginUpdate()
        ResizeBackgroundImage() ' resize the background to fill new size
        'ListView1.EndUpdate()
    End Sub

    ''' <summary>
    ''' Execute the file (run) or folder (explorer).
    ''' </summary>
    Private Sub ListView1_ItemActivate(sender As Object, e As EventArgs) Handles ListView1.ItemActivate
        Dim list1 = ListView1.SelectedItems
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
                        My.Computer.FileSystem.DeleteFile(element.Name, FileIO.UIOption.AllDialogs, _
                                                          FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                    ElseIf Directory.Exists(element.Name) Then
                        My.Computer.FileSystem.DeleteDirectory(element.Name, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
                    End If
                Catch ex As Exception
                    MsgBox(ex.Message & vbNewLine & "Error code: 4843", MsgBoxStyle.Critical, "Error")
                End Try
            End If
        Next
    End Sub

    ''' <summary>
    ''' Debug related ToolStripMenuItem.
    ''' </summary>
    Private Sub DebugTestToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DebugTestToolStripMenuItem.Click
        Dim list1 = ListView1.Items
        For Each element In ImageList1.Images
            'MsgBox("Contains image: " & ImageList1.Images.ContainsKey(element.Name))
            'MsgBox("Contains ListViewItem: " & ListView1.Items.ContainsKey(element.Text))

            'MsgBox(element.ToString())
        Next
        For Each element In ListView1.Items
            'MsgBox("Contains image: " & ImageList1.Images.ContainsKey(element.Name))
            'MsgBox("Contains ListViewItem: " & ListView1.Items.ContainsKey(element.Text))

            MsgBox(element.Position.ToString)
        Next
        If ImageList1.Images.Count = 0 Then
            MsgBox("ImageList1.Images is empty.", MsgBoxStyle.Information, "debug message")
        End If
    End Sub

    Private Sub NewScreen_ResizeBegin(sender As Object, e As EventArgs) Handles MyBase.ResizeBegin
        resizeInProgress = True
    End Sub

    Private Sub NewScreen_ResizeEnd(sender As Object, e As EventArgs) Handles MyBase.ResizeEnd
        resizeInProgress = False
        ResizeBackgroundImage()
    End Sub

    Private Sub BREAKToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BREAKToolStripMenuItem.Click
        Debugger.Break()
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

    ''' <summary>
    ''' Saves various settings for this screen.
    ''' </summary>
    Public Function SaveSettings() As Boolean
        'My.Settings.ScreenSize = tmpScreenSize ' uses a temp variable because screen size changes when maximized but we don't want to save that size
        'My.Settings.ScreenLocation = tmpScreenLocation ' uses a temp variable because screen location changes when maximized but we don't want to save that loc

        If CheckIfSaveIsBackup() Then ' check if currently open save file is a backup save
            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
            Dim result1 As MsgBoxResult = MsgBox("The currently open save file was detected as a backup save file!" & vbNewLine & _
                                                 vbNewLine & "Click ""Yes"" to overwrite this save file and remove its backup save file status." & _
                                                 vbNewLine & "Click ""No"" to quit the program without saving." & _
                                                 vbNewLine & "Click ""Cancel"" to cancel.", MsgBoxStyle.YesNoCancel, "Warning")
            ' yes result check is not necessary, by default saving always leaves out backup save file flag strings and setting in save file, only backup saves don't
            If result1 = MsgBoxResult.Cancel Then
                Return True ' abort closing of program
            ElseIf result1 = MsgBoxResult.No Then
                End ' exit program without saving
            End If
        End If

        If Options.saveOnceFlag Then ' save once flag for disabling autosave
            If SaveFileActive Then
                SaveFile()
            Else
                SaveScreen.Show()
                SaveScreen.SaveScreenButton.PerformClick()
            End If
        ElseIf SaveFileActive = False Then
            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
            If MsgBox("Save the screen before leaving?", MsgBoxStyle.YesNo, "Warning") = MsgBoxResult.Yes Then
                exitingFirstSave = True
                SaveScreen.Show()
                SaveScreen.SaveScreenButton.PerformClick()
                Return True ' abort closing of program
            End If
        ElseIf SaveFileActive = True AndAlso AutoSaveEnabled = True Then
            SaveFile()
        End If
        Return False ' close the program
    End Function

    ''' <summary>
    ''' Determines where to create the save file, then calls sub to save to that location.
    ''' </summary>
    Public Sub SaveFile()
        If My.Application.CommandLineArgs.Count > 0 Then
            If Me.tmpSaveLocation = "" Then
                SaveToSaveFile(My.Application.CommandLineArgs(0))
            Else
                SaveToSaveFile(Me.tmpSaveLocation)
            End If
        ElseIf Not Me.tmpSaveLocation = "" Then
            SaveToSaveFile(Me.tmpSaveLocation)
        Else
            MsgBox("Save file location not set.", MsgBoxStyle.Critical, "Error")
        End If
    End Sub

    Dim SaveFileIsABackupSaveFlag As Boolean = False ' flag for whether or not the current save file is a backup flagged one
    ''' <summary>
    ''' Checks if the currently open save file is a backup save.
    ''' </summary>
    Function CheckIfSaveIsBackup() As Boolean
        If My.Application.CommandLineArgs.Count > 0 Then
            If SaveFileIsABackupSaveFlag = True Then
                Return True ' is a backup save file
            End If
        End If
        Return False
    End Function

    Private Sub NewScreen_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        'My.Settings.FirstTimeOpening = False ' no longer the first time openning this screen, so things like screen size will be loaded on startup next time and there-after
        If SaveSettings() = True Then
            e.Cancel = True ' abort closing of the program
        End If
    End Sub

    Dim tmpScreenSize As Point
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

    Private Sub SaveScreenAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveScreenAsToolStripMenuItem.Click
        SaveScreen.Show()
        SaveScreen.TopMost = True ' will make top most no matter what without unsetting
        SaveScreen.TopMost = False ' now basically it was brought to focus now
    End Sub

    Public tempBackgroundLocation As String = "" ' used to store newly changed background info for saving to save file
    ''' <summary>
    ''' Saves the current settings of the program to save file format. (VFCScreen extension which is openable by this program)
    ''' </summary>
    ''' <param name="filePath">File path of the save file to write to.</param>
    Public Sub SaveToSaveFile(filePath As String)
        Try
            Dim ImageKeyPairs As New Dictionary(Of Integer, String) ' (index of imagelist, string key of image)
            Dim count As Integer = 0
            For Each e As String In ImageList1.Images.Keys
                Try
                    ImageKeyPairs.Add(count, e) ' uses count so the "<NoIcon>" doesn't prevent adding of and causes error
                    count += 1
                Catch ex As Exception
                    MsgBox(ex.Message & vbNewLine & "Error code: 23451", MsgBoxStyle.Critical, "Error")
                End Try
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
                Dim filePath3 As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                If Directory.Exists(filePath3) Then
                    filePathTempFinal = filePath3
                Else
                    MsgBox("Could not write to disk, save failed. Error code: 2956239", MsgBoxStyle.Critical, "Critical Error")
                    Exit Sub ' abort saving (should be changed to redo or something)
                End If
            End If

            Dim tmpStr1() As String = Split(filePath, "\")
            Dim fs As System.IO.FileStream = New System.IO.FileStream(filePathTempFinal & "\" & tmpStr1(tmpStr1.Count - 1), System.IO.FileMode.Create)
            If fs.CanWrite Then
                Try
                    WriteToFile("<GeneralSettings>", fs) ' general settings start
                    WriteToFile(vbNewLine, fs) ' readability

                    WriteToFile("<ScreenName>", fs)
                    'If Not tmpSaveLocation = "" Then
                    '     WriteToFile(GetFileDisplayString(tmpSaveLocation), fs)
                    'Else
                    WriteToFile(Me.Text, fs) ' set current form title as screen name
                    'End If
                    WriteToFile("<ScreenName>", fs)

                    WriteToFile("<AutoSaveEnabled>", fs)
                    If AutoSaveEnabled = True Then
                        WriteToFile("True", fs)
                    Else
                        WriteToFile("False", fs)
                    End If
                    WriteToFile("<AutoSaveEnabled>", fs)

                    WriteToFile("<BackgroundImage>", fs)
                    If Not tempBackgroundLocation = "" Then
                        WriteToFile(tempBackgroundLocation, fs)
                    End If
                    WriteToFile("<BackgroundImage>", fs)

                    WriteToFile("<ScreenLocation>", fs)
                    WriteToFile(CStr(tmpScreenLocation.X) & "," & CStr(tmpScreenLocation.Y), fs)
                    WriteToFile("<ScreenLocation>", fs)

                    WriteToFile("<ScreenSize>", fs)
                    WriteToFile(CStr(tmpScreenSize.X) & "," & CStr(tmpScreenSize.Y), fs)
                    WriteToFile("<ScreenSize>", fs)

                    'WriteToFile("<FirstTimeOpening>", fs)
                    'WriteToFile("False", fs)
                    'WriteToFile("<FirstTimeOpening>", fs)

                    WriteToFile("<ScreenMaximized>", fs)
                    If Me.WindowState = FormWindowState.Maximized Then
                        WriteToFile("True", fs) ' maximized state
                    Else
                        WriteToFile("False", fs) ' normal state
                    End If
                    WriteToFile("<ScreenMaximized>", fs)

                    WriteToFile("<ScreenScrollable>", fs)
                    If Me.ListView1.Scrollable = True Then
                        WriteToFile("True", fs)
                    Else
                        WriteToFile("False", fs)
                    End If
                    WriteToFile("<ScreenScrollable>", fs)

                    WriteToFile("<ScreenAutoArrange>", fs)
                    If Me.ListView1.AutoArrange = True Then
                        WriteToFile("True", fs)
                    Else
                        WriteToFile("False", fs)
                    End If
                    WriteToFile("<ScreenAutoArrange>", fs)

                    WriteToFile("<ScreenAlignment>", fs)
                    If Me.ListView1.Alignment = ListViewAlignment.Left Then ' 0 value respresents Left
                        WriteToFile("0", fs)
                    ElseIf Me.ListView1.Alignment = ListViewAlignment.Top Then ' 1 value respresents Top
                        WriteToFile("1", fs)
                    ElseIf Me.ListView1.Alignment = ListViewAlignment.SnapToGrid Then ' 2 value respresents Snap To Grid
                        WriteToFile("2", fs)
                    Else
                        WriteToFile("1", fs) ' default value (Top)
                    End If
                    WriteToFile("<ScreenAlignment>", fs)

                    WriteToFile("<ScreenActivation>", fs)
                    If Me.ListView1.Activation = ItemActivation.TwoClick Then
                        WriteToFile("TwoClick", fs)
                    ElseIf Me.ListView1.Activation = ItemActivation.OneClick Then
                        WriteToFile("OneClick", fs)
                    ElseIf Me.ListView1.Activation = ItemActivation.Standard Then
                        WriteToFile("Standard", fs)
                    End If
                    WriteToFile("<ScreenActivation>", fs)

                    WriteToFile(vbNewLine, fs) ' readability
                    WriteToFile("<StartupSound>", fs)
                    WriteToFile(startupSound, fs)
                    WriteToFile("<StartupSound>", fs)

                    If saveBackupsMaximum > 0 AndAlso Directory.Exists(saveBackupsDirectory) Then ' make a backup save also if enabled
                        'If (My.Application.CommandLineArgs.Count > 0 AndAlso (Not My.Application.CommandLineArgs(0) = Me.tmpSaveLocation)) OrElse (Not SaveScreen.oldValue = "" AndAlso Not Me.tmpSaveLocation = SaveScreen.oldValue) Then
                        'saveBackupsCount = 1 ' set to 1 because save as made a new and different screen save file
                        'Else ' normal count incrementing
                        If saveBackupsCount >= saveBackupsMaximum Then ' update count
                            saveBackupsCount = 1 ' reset to 1 again
                        Else
                            saveBackupsCount += 1
                        End If
                        'End If
                    End If

                    WriteToFile(vbNewLine, fs) ' readability
                    WriteToFile("<BackupDirectory>", fs)
                    WriteToFile(saveBackupsDirectory, fs)
                    WriteToFile("<BackupDirectory>", fs)
                    WriteToFile("<BackupMaximum>", fs)
                    WriteToFile(CStr(saveBackupsMaximum), fs)
                    WriteToFile("<BackupMaximum>", fs)
                    WriteToFile("<BackupCount>", fs)
                    WriteToFile(CStr(saveBackupsCount), fs)
                    WriteToFile("<BackupCount>", fs)

                    WriteToFile(vbNewLine, fs) ' readability
                    WriteToFile("<EmbedIcons>", fs)
                    WriteToFile(CStr(embedIconsEnabled), fs)
                    WriteToFile("<EmbedIcons>", fs)

                    WriteToFile(vbNewLine, fs) ' readability
                    WriteToFile("<RealItemDelete>", fs)
                    WriteToFile(CStr(realDeleteEnabled), fs)
                    WriteToFile("<RealItemDelete>", fs)

                    WriteToFile(vbNewLine, fs) ' readability
                    WriteToFile("<ScrollableExpander>", fs)
                    WriteToFile(CStr(ScrollableExpander), fs)
                    WriteToFile("<ScrollableExpander>", fs)
                    WriteToFile("<ScreenWidthScrollableExpander>", fs)
                    WriteToFile(CStr(ScreenWidthScrollableExpander), fs)
                    WriteToFile("<ScreenWidthScrollableExpander>", fs)
                    WriteToFile("<ScreenHeightScrollableExpander>", fs)
                    WriteToFile(CStr(ScreenHeightScrollableExpander), fs)
                    WriteToFile("<ScreenHeightScrollableExpander>", fs)

                    WriteToFile(vbNewLine, fs) ' readability
                    WriteToFile("<GeneralSettings>", fs) ' general settings end
                    WriteToFile(vbNewLine, fs) ' readability

                    WriteToFile("<LVIGroup>", fs) ' tags start of LVI stuff
                    WriteToFile(vbNewLine, fs) ' readability
                    Dim tempDecrement As Integer = 0 ' keeping cosmetic readability of save file for just the "<ListViewIndex>" counts
                    For i As Integer = 0 To ImageKeyPairs.Count - 1
                        Dim tmpFileDisplayString As String = GetFileDisplayString(ImageKeyPairs.Item(i))
                        'If ListView1.Items.ContainsKey(tmpFileDisplayString) Then

                        WriteToFile("<ListViewIndex>", fs) ' ImageKeyPairs(INDEX, ...) 
                        WriteToFile(CStr(i - tempDecrement), fs) ' index number of dictionary
                        WriteToFile("<ListViewIndex>", fs) ' ImageKeyPairs(INDEX, ...) 

                        WriteToFile("<Position>", fs) ' item position 
                        WriteToFile(CStr(ListView1.Items(i).Position.X) & "," & CStr(ListView1.Items(i).Position.Y), fs)
                        'WriteToFile(CStr(ListView1.Items(ListView1.Items.IndexOfKey(tmpFileDisplayString)).Position.X) & "," & _
                        '            CStr(ListView1.Items(ListView1.Items.IndexOfKey(tmpFileDisplayString)).Position.Y), fs)
                        WriteToFile("<Position>", fs) ' item position 

                        WriteToFile("<ItemDisplayName>", fs)
                        WriteToFile(ListView1.Items(i).Text, fs) ' display name of item
                        WriteToFile("<ItemDisplayName>", fs)

                        WriteToFile("<ItemFullPath>", fs) ' ImageKeyPairs.Item(i)
                        WriteToFile(ImageKeyPairs.Item(i), fs) ' full path of item
                        WriteToFile("<ItemFullPath>", fs) ' ImageKeyPairs.Item(i)

                        If embedIconsEnabled Then
                            WriteToFile("<IconImage>", fs)

                            'WriteToFile(ImagesStorage.Item(i), fs) ' write 
                            'Dim ic As New ImageConverter
                            'Dim imgInFile As Image = ic.ConvertTo(ImageList1.Images(i), Imaging.ImageFormat.Png)
                            If (ImageList1.Images(i).RawFormat.Equals(Imaging.ImageFormat.MemoryBmp)) Then
                                'Dim bytes() As Byte = Convert.FromBase64String(ImageToBase64(ImageList1.Images(i), Imaging.ImageFormat.Bmp, i))

                                If False Then
                                    Using memStream As New MemoryStream
                                        ImageList1.Images(i).Save(memStream, Imaging.ImageFormat.Png)

                                        Dim str As String = Convert.ToBase64String(memStream.ToArray)
                                        Dim encoding As New System.Text.UTF8Encoding
                                        fs.Write(encoding.GetBytes(str), 0, encoding.GetByteCount(str))
                                        fs.Flush() ' write stuff up to this point to the file
                                        'ImageToBase64(ImageList1.Images(i), Imaging.ImageFormat.Png, i)
                                    End Using
                                End If

                                WriteToFile(ImageToBase64(ImageList1.Images(i), Imaging.ImageFormat.Bmp, i), fs) ' write image
                            Else
                                MsgBox("DANGER ZONE!!!!", MsgBoxStyle.Critical, "Error")
                            End If

                            WriteToFile("<IconImage>", fs)
                        End If

                        WriteToFile(vbNewLine, fs) ' readability
                        'Else
                        '     tempDecrement += 1 ' cosmetic count is minus one for display in save file only
                        'End If
                    Next

                    For Each item As ListViewItem In ListView1.Items
                        If item.Text = "*[Screen Expander]*" AndAlso item.Name = "" Then
                            WriteToFile("<ListViewIndex>", fs) ' ImageKeyPairs(INDEX, ...) 
                            WriteToFile("9999999999999", fs) ' index number of dictionary
                            WriteToFile("<ListViewIndex>", fs) ' ImageKeyPairs(INDEX, ...) 

                            WriteToFile("<Position>", fs) ' item position 
                            WriteToFile(CStr(item.Position.X) & "," & CStr(item.Position.Y), fs)
                            WriteToFile("<Position>", fs) ' item position 

                            WriteToFile("<ItemDisplayName>", fs)
                            WriteToFile(item.Text, fs) ' display name of item
                            WriteToFile("<ItemDisplayName>", fs)

                            WriteToFile("<ItemFullPath>", fs)
                            WriteToFile("", fs)
                            WriteToFile("<ItemFullPath>", fs)

                            WriteToFile(vbNewLine, fs) ' readability
                        End If
                    Next

                    WriteToFile("<LVIGroup>", fs) ' tags end of LVI stuff

                    fs.Dispose() ' close file stream so it can be moved etc.
                    If fs IsNot Nothing Then
                        fs.Close()
                        fs = Nothing
                    End If

                    If Not filePath = "" Then
                        Dim tmpStr = Split(filePath, "\")

                        My.Computer.FileSystem.CopyFile(filePathTempFinal & "\" & tmpStr(tmpStr.Count - 1), filePath, True) ' save file before save backup appending

                        If saveBackupsMaximum > 0 AndAlso Directory.Exists(saveBackupsDirectory) Then ' make a backup save also if enabled
                            Using fsB As System.IO.FileStream = New System.IO.FileStream(filePathTempFinal & "\" & tmpStr1(tmpStr1.Count - 1), System.IO.FileMode.Append)
                                If fsB.CanWrite Then
                                    ' append a backup file flag, so this save is identified as a backup file in the future
                                    WriteToFile(vbNewLine, fsB) ' readability
                                    WriteToFile("<BackupFileFlag>", fsB)
                                    WriteToFile("True", fsB)
                                    WriteToFile("<BackupFileFlag>", fsB)
                                End If
                            End Using
                            My.Computer.FileSystem.CopyFile(filePathTempFinal & "\" & tmpStr(tmpStr.Count - 1), _
                                                            saveBackupsDirectory & "\" & GetFileDisplayString(tmpStr(tmpStr.Count - 1)) & _
                                                            " [Backup " & saveBackupsCount & "]" & "." & GetFileExtensionString(tmpStr(tmpStr.Count - 1)), True)
                            ' count is updated before when saving to file
                        End If

                        My.Computer.FileSystem.DeleteFile(filePathTempFinal & "\" & tmpStr(tmpStr.Count - 1)) ' delete the temp file
                    Else ' default is desktop
                        MsgBox("Save file location was not accessible, attempting to save to desktop as 'Default' named save instead.", MsgBoxStyle.Critical, "Error")
                        Dim tmpStr = Split(filePath, "\")
                        My.Computer.FileSystem.MoveFile(filePathTempFinal & "\" & tmpStr(tmpStr.Count - 1), _
                                                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\" & "Default.VFCScreen", True)
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
                    MsgBox("Could not finish writing to disk, save failed. Error code: 2956", MsgBoxStyle.Critical, "Critical Error")
                    Exit Sub ' abort saving (should be changed to redo or something)
                End Try
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
        End Try
    End Sub

    ''' <summary>
    ''' Writes a string to a given FileStream.
    ''' </summary>
    ''' <param name="str">String to be written using the given FileStream.</param>
    ''' <param name="fs">FileStream used for writing to file.</param>
    Private Overloads Sub WriteToFile(ByRef str As String, ByRef fs As System.IO.FileStream)
        'Dim charArray() As Char = str.ToCharArray
        'For Each c As Char In charArray
        '   fs.WriteByte(CByte(AscW(c)))
        'Next
        Dim encoding As New System.Text.UTF8Encoding
        fs.Write(encoding.GetBytes(str), 0, encoding.GetByteCount(str))

        fs.Flush() ' write stuff up to this point to the file
    End Sub

    Private Function ImageToBase64(ByRef image As System.Drawing.Image, ByRef format As System.Drawing.Imaging.ImageFormat, index As Integer) As String
        Using ms As New MemoryStream()
            ' Convert Image to byte[]
            image.Save(ms, format)
            Dim imageBytes() As Byte = ms.ToArray()

            ' Convert byte[] to Base64 String
            Dim base64String As String = Convert.ToBase64String(imageBytes)

            Return base64String
        End Using
    End Function

    Private Function Base64ToImage(ByRef base64String As String) As Image
        ' Convert Base64 String to byte[]
        Dim imageBytes() As Byte = Convert.FromBase64String(base64String)
        Using ms As New MemoryStream(imageBytes, 0, imageBytes.Length)
            ' Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length)
            Dim image As Image = image.FromStream(ms, False)

            Return image
        End Using
    End Function

    ''' <summary>
    ''' Loads a screen from the given save file path.
    ''' </summary>
    ''' <param name="filePath">File path of a screen save file.</param>
    Private Sub LoadScreenFromFile(filePath As String)
        'MsgBox("go into editor and attach process now")
        Dim encoding As New System.Text.UTF8Encoding
        Dim SaveData As String = My.Computer.FileSystem.ReadAllText(filePath, encoding)
        Dim SaveBackupFlag() As String = Split(SaveData, "<BackupFileFlag>")
        Dim LVIData() As String = Split(SaveData, "<LVIGroup>") ' stores only the LVI group text here for use with more splits
        Dim LVIDisplayNames() As String = Split(LVIData(1), "<ItemDisplayName>")

        Dim LVIFullPath() As String = Split(LVIData(1), "<ItemFullPath>")
        'Dim LVI_Image() As String = Split(LVIData(1), "<IconImage>")
        Dim LVIPosition() As String = Split(LVIData(1), "<Position>")

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim GeneralSettings() As String = Split(LVIData(0), "<GeneralSettings>")
        Dim ScreenName() As String = Split(GeneralSettings(1), "<ScreenName>")
        Dim AutoSaveEnabledTmp() As String = Split(GeneralSettings(1), "<AutoSaveEnabled>")
        Dim BackgroundImage() As String = Split(GeneralSettings(1), "<BackgroundImage>")
        Dim StartupSoundTemp() As String = Split(GeneralSettings(1), "<StartupSound>")
        Dim ScreenLocation() As String = Split(GeneralSettings(1), "<ScreenLocation>")
        Dim ScreenSize() As String = Split(GeneralSettings(1), "<ScreenSize>")
        'Dim FirstTimeOpening() As String = Split(GeneralSettings(1), "<FirstTimeOpening>")
        Dim ScreenMaximized() As String = Split(GeneralSettings(1), "<ScreenMaximized>")
        Dim ScreenScrollable() As String = Split(GeneralSettings(1), "<ScreenScrollable>")
        Dim ScreenAutoArrange() As String = Split(GeneralSettings(1), "<ScreenAutoArrange>")
        Dim ScreenAlignment() As String = Split(GeneralSettings(1), "<ScreenAlignment>")
        Dim ScreenActivation() As String = Split(GeneralSettings(1), "<ScreenActivation>")

        Dim BackupDirectory() As String = Split(GeneralSettings(1), "<BackupDirectory>")
        Dim BackupMaximum() As String = Split(GeneralSettings(1), "<BackupMaximum>")
        Dim BackupCount() As String = Split(GeneralSettings(1), "<BackupCount>")

        Dim EmbedIconFlag() As String = Split(GeneralSettings(1), "<EmbedIcons>")

        Dim RealDelete() As String = Split(GeneralSettings(1), "<RealItemDelete>")

        Dim ScrollableExpanderTmp() As String = Split(GeneralSettings(1), "<ScrollableExpander>")
        Dim ScreenWidthScrollableExpanderTmp() As String = Split(GeneralSettings(1), "<ScreenWidthScrollableExpander>")
        Dim ScreenHeightScrollableExpanderTmp() As String = Split(GeneralSettings(1), "<ScreenHeightScrollableExpander>")

        Try
            If HasIndexOne(ScreenName) Then
                Me.Text = ScreenName(1) ' set title of window
            End If

            If HasIndexOne(AutoSaveEnabledTmp) Then
                AutoSaveEnabled = CBool(AutoSaveEnabledTmp(1))
            End If

            If HasIndexOne(BackgroundImage) Then
                If File.Exists(BackgroundImage(1)) Then
                    backgroundPicture = Image.FromFile(BackgroundImage(1))
                    tempBackgroundLocation = BackgroundImage(1) ' so that it keeps re-saving the location to save file on close
                End If
            End If

            If HasIndexOne(StartupSoundTemp) Then
                If File.Exists(StartupSoundTemp(1)) Then
                    startupSound = StartupSoundTemp(1)
                End If
            End If

            If HasIndexOne(ScreenSize) Then
                Dim strTemp() As String = Split(ScreenSize(1), ",")
                Me.Size = New Point(CInt(strTemp(0)), CInt(strTemp(1)))
            End If

            If HasIndexOne(ScreenLocation) Then
                Dim strTemp2() = Split(ScreenLocation(1), ",")
                Me.Location = New Point(CInt(strTemp2(0)), CInt(strTemp2(1)))
                Dim allScreens() As Windows.Forms.Screen = Screen.AllScreens
                Dim totalX As Integer = 0, currentY As Integer = 0, countTmp As Integer = 0
                For Each screen1 In allScreens ' test if screen location is out of bounds (usually only possible if was saved on a now not active monitor or save edited badly)

                    totalX += screen1.Bounds.Width
                    currentY = screen1.Bounds.Height

                    If countTmp = 0 Then ' check on first screen only
                        If Me.Location.X < 0 AndAlso (Me.Location.X + Me.Size.Width <= 0) Then ' check for negative X
                            GoTo resetScreen
                        ElseIf Me.Location.Y < 0 Then ' check for negative Y
                            GoTo resetScreen
                        End If
                    End If

                    If Not Me.Location.X > totalX AndAlso Me.Location.Y < currentY - 30 Then ' taskbar is not accounted for very well so program could be hidden by it if save is edited
                        Exit For ' doesn't need location resetting
                    End If

                    countTmp += 1
                    If allScreens.Count = countTmp Then ' if last screen tested
resetScreen:
                        Me.Location = New Point(0, 0) ' reset the location to upper left most position
                        Exit For
                    End If
                Next
            End If

            If HasIndexOne(ScreenMaximized) Then
                If CBool(ScreenMaximized(1)) = True Then
                    Me.WindowState = FormWindowState.Maximized
                Else
                    Me.WindowState = FormWindowState.Normal
                End If
            End If

            If HasIndexOne(ScreenScrollable) Then
                If CBool(ScreenScrollable(1)) = True Then
                    Me.ListView1.Scrollable = True
                Else
                    Me.ListView1.Scrollable = False
                End If
            End If

            If HasIndexOne(ScreenAutoArrange) Then
                If CBool(ScreenAutoArrange(1)) = True Then
                    Me.ListView1.AutoArrange = True
                Else
                    Me.ListView1.AutoArrange = False
                End If
            End If

            If HasIndexOne(ScreenAlignment) Then
                If CInt(ScreenAlignment(1)) = 0 Then ' 0 value respresents Left
                    Me.ListView1.Alignment = ListViewAlignment.Left
                ElseIf CInt(ScreenAlignment(1)) = 1 Then ' 1 value respresents Top
                    Me.ListView1.Alignment = ListViewAlignment.Top
                ElseIf CInt(ScreenAlignment(1)) = 2 Then ' 2 value respresents Snap To Grid
                    Me.ListView1.Alignment = ListViewAlignment.SnapToGrid
                End If
            End If

            If HasIndexOne(ScreenActivation) Then
                If ScreenActivation(1) = "TwoClick" Then
                    Me.ListView1.Activation = ItemActivation.TwoClick
                ElseIf ScreenActivation(1) = "OneClick" Then
                    Me.ListView1.Activation = ItemActivation.OneClick
                ElseIf ScreenActivation(1) = "Standard" Then
                    Me.ListView1.Activation = ItemActivation.Standard
                End If
            End If

            ' save auto backup information
            If HasIndexOne(BackupDirectory) Then
                saveBackupsDirectory = BackupDirectory(1)
            End If
            If HasIndexOne(BackupMaximum) Then
                saveBackupsMaximum = CInt(BackupMaximum(1))
            End If
            If HasIndexOne(BackupCount) Then
                saveBackupsCount = CInt(BackupCount(1))
            End If

            If HasIndexOne(RealDelete) Then
                realDeleteEnabled = CBool(RealDelete(1))
            End If

            If HasIndexOne(ScrollableExpanderTmp) Then ' scrollable expander item is currently in the view flag
                ScrollableExpander = CBool(ScrollableExpanderTmp(1))
            End If
            If HasIndexOne(ScreenWidthScrollableExpanderTmp) Then ' scrollable expander width (options NumericUpDown value)
                ScreenWidthScrollableExpander = CInt(ScreenWidthScrollableExpanderTmp(1))
            End If
            If HasIndexOne(ScreenHeightScrollableExpanderTmp) Then ' scrollable expander height (options NumericUpDown value)
                ScreenHeightScrollableExpander = CInt(ScreenHeightScrollableExpanderTmp(1))
            End If

            If HasIndexOne(SaveBackupFlag) Then ' determine whether save is a backup save file flagged one
                If CBool(SaveBackupFlag(1)) = True Then
                    SaveFileIsABackupSaveFlag = True
                Else
                    SaveFileIsABackupSaveFlag = False
                End If
            Else
                SaveFileIsABackupSaveFlag = False
            End If

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Dim embeddedIconsFlag As Boolean
            If HasIndexOne(EmbedIconFlag) AndAlso CBool(EmbedIconFlag(1)) = True Then
                embeddedIconsFlag = True
            Else
                embeddedIconsFlag = False
            End If

            Dim count1 As Integer = 0
            For i As Integer = 1 To LVIDisplayNames.Count - 2 Step 2
                Dim newItem As New ListViewItem
                Dim ic As New ImageConverter

                'Dim image As Image = Base64ToImage(LVI_Image(i))
                'ImageList1.Images.Add(LVIFullPath(i), image)
                newItem.Name = LVIFullPath(i) ' hidden full path
                newItem.Text = LVIDisplayNames(i) ' visible label
                'newItem.ImageKey = LVIFullPath(i)
                ListView1.Items.Add(newItem)

                Dim pos() As String = Split(LVIPosition(i), ",")
                ListView1.Items(count1).Position = New Point(pos(0), pos(1))

                ' get icon from file and not saved one
                Dim shfi As Shell32.SHFILEINFO
                If File.Exists(LVIFullPath(i)) Then ' file path
                    If embeddedIconsFlag Then
                        ' do load from embedded icon here
                    Else
                        Using bmp1 = New Bitmap(ImageList1.ImageSize.Width, ImageList1.ImageSize.Height) ' stretch the background image to fit the new size
                            Using g1 = Graphics.FromImage(bmp1)
                                Dim shortcutCheck As Boolean = CheckIfShortcut(LVIFullPath(i))
                                g1.DrawIcon(IconReader.ExtractIconFromFileEx(LVIFullPath(i), IconReader.IconSize.ExtraLarge, shortcutCheck, shfi), 0, 0)
                                If shortcutCheck Then ' add shortcut overlay if necessary
                                    g1.DrawImage(My.Resources.ShortcutOverlay, 0, (ImageList1.ImageSize.Height - My.Resources.ShortcutOverlay.Height), _
                                                 My.Resources.ShortcutOverlay.Width, My.Resources.ShortcutOverlay.Height)
                                End If
                                ImageList1.Images.Add(LVIFullPath(i), bmp1.Clone)
                                ListView1.Items(ListView1.Items.Count - 1).ImageKey = LVIFullPath(i)
                            End Using
                        End Using
                    End If
                ElseIf Directory.Exists(LVIFullPath(i)) Then ' folder path
                    If embeddedIconsFlag Then
                        ' do load from embedded icon here
                    Else
                        ImageList1.Images.Add(LVIFullPath(i), IconReader.GetFolderIcon(IconReader.IconSize.ExtraLarge, IconReader.FolderType.Open, shfi))
                        ListView1.Items(ListView1.Items.Count - 1).ImageKey = LVIFullPath(i)
                    End If
                Else ' unknown path given
                    If Not (LVIDisplayNames(i) = "*[Screen Expander]*" AndAlso LVIFullPath(i) = "") Then ' used for scrollable screen expanding
                        MsgBox("File or Folder is missing. It has been marked with an X icon.", MsgBoxStyle.Critical, "Warning!")
                        ImageList1.Images.Add(LVIFullPath(i), My.Resources.NoIcon) ' "<NoIcon>"
                        ListView1.Items(ListView1.Items.Count - 1).ImageKey = LVIFullPath(i)
                    End If
                End If

                count1 += 1
            Next
        Catch ex As Exception
            MsgBox("Critical error loading save file" & vbNewLine & ex.Message & vbNewLine & ex.Source, MsgBoxStyle.Critical, "Error")
            End
        End Try
    End Sub

    ''' <summary>
    ''' Used to test if the stringarray(1) index is not out of bounds and ready for use
    ''' </summary>
    ''' <param name="test">string array to be tested</param>
    Private Function HasIndexOne(ByRef test() As String) As Boolean
        If (test.Count - 1) >= 1 Then
            Return True ' has an index value of 1 available for use at least ex: blahblah(1) is not out of bounds
        Else
            Return False ' has no content
        End If
    End Function

    ''' <summary>
    ''' Show the ListBox view of the Listview.
    ''' </summary>
    Private Sub ShowListBox() Handles ViewListToolStripMenuItem.Click
        ListView1.Visible = False

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

            Dim Name As String = ListBox1Names.Items.Item(selectedIndex)

            ' delete item in listview
            Dim item As ListViewItem = ListView1.Items.Item(ListView1.Items.IndexOfKey(Name))
            ImageList1.Images.RemoveByKey(item.ImageKey)
            ListView1.Items.Item(item.Index).Remove()

            If confirmed Then ' flag for deleting the real item on the OS also or not
                Try
                    If File.Exists(ListBox1Names.Items.Item(selectedIndex)) Then
                        My.Computer.FileSystem.DeleteFile(ListBox1Names.Items.Item(selectedIndex), FileIO.UIOption.AllDialogs, _
                                                            FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                    ElseIf Directory.Exists(ListBox1Names.Items.Item(selectedIndex)) Then
                        My.Computer.FileSystem.DeleteDirectory(ListBox1Names.Items.Item(selectedIndex), FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
                    End If
                Catch ex As Exception
                    MsgBox(ex.Message & vbNewLine & "Error code: 4843", MsgBoxStyle.Critical, "Error")
                End Try
            End If

            ' delete item in listboxes
            ListBox1.Items.RemoveAt(selectedIndex)
            ListBox1Names.Items.RemoveAt(selectedIndex)
        End If
    End Sub

    Private Sub ViewScreenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewScreenToolStripMenuItem.Click
        ListBox1.Visible = False
        ListView1.Visible = True
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
            If File.Exists(ListBox1Names.Items.Item(selectedIndex)) OrElse Directory.Exists(ListBox1Names.Items.Item(selectedIndex)) Then
                Try
                    Process.Start(ListBox1Names.Items.Item(selectedIndex))
                Catch ex As System.ComponentModel.Win32Exception
                    If CheckIfShortcut(ListBox1Names.Items.Item(selectedIndex)) AndAlso ex.Message = "The operation was canceled by the user" Then
                        MsgBox("The shortcut " & "'" & GetFileDisplayString(ListBox1Names.Items.Item(selectedIndex)) & ".lnk'" & " targets a no longer present file or folder.", MsgBoxStyle.Critical, "Error")
                    End If
                End Try
            Else
                MsgBox("File or folder no longer exists!" & vbNewLine & """" & ListBox1Names.Items.Item(selectedIndex) & """", MsgBoxStyle.Critical, "Error")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Open containing folder for ListBox view.
    ''' </summary>
    Private Sub OpenContainingFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenContainingFolderToolStripMenuItem.Click
        Dim selectedIndex As Integer = ListBox1.SelectedIndex
        If Not IsNothing(selectedIndex) AndAlso selectedIndex >= 0 Then
            If File.Exists(ListBox1Names.Items.Item(selectedIndex)) OrElse Directory.Exists(ListBox1Names.Items.Item(selectedIndex)) Then
                Call Shell("explorer /select," & ListBox1Names.Items.Item(selectedIndex), AppWinStyle.NormalFocus) ' select in containing folder
            Else
                MsgBox("File or folder no longer exists!" & vbNewLine & """" & ListBox1Names.Items.Item(selectedIndex) & """", MsgBoxStyle.Critical, "Error")
            End If
        End If
    End Sub

    Dim itemPositions As Object
    ''' <summary>
    ''' Store ListView item positions for later restoring
    ''' </summary>
    Public Sub StoreItemPositions()
        Dim list As New ArrayList
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

End Class