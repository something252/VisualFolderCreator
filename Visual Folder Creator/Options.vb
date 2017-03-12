Imports System.IO

Public Class Options
    Dim loading As Boolean = True ' flag for whether loading this form still or not
    Dim PasswordEnabled As Boolean
    Dim NewPWrapper As Simple3Des

    Private Sub Options_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With NewScreen
            Me.Icon = My.Resources.icon

            AutoSaveCheckBox.Checked = .AutoSaveEnabled

            If Not .tempBackgroundLocation = "" Then
                TextBox1.Text = .tempBackgroundLocation
            Else
                TextBox1.Text = ""
            End If

            If IO.File.Exists(.startupSound) Then
                TextBox2.Text = .startupSound
            Else
                TextBox2.Text = ""
            End If

            If .realDeleteEnabled = True Then
                RealDeleteCheckBox.Checked = True
            Else
                RealDeleteCheckBox.Checked = False
            End If

            If .embedIconsEnabled = True Then
                EmbedIconsCheckBox.Checked = True
            Else
                EmbedIconsCheckBox.Checked = False
            End If

            AutoArrangeCheckBox.Checked = .ListView1.AutoArrange
            If .ListView1.Alignment = ListViewAlignment.Top Then
                AlignmentComboBox.SelectedIndex = 0
            ElseIf .ListView1.Alignment = ListViewAlignment.Left Then
                AlignmentComboBox.SelectedIndex = 1
            ElseIf .ListView1.Alignment = ListViewAlignment.SnapToGrid Then
                AlignmentComboBox.SelectedIndex = 2
            End If
            If .ListView1.Activation = ItemActivation.OneClick Then
                ClickActivationComboBox.SelectedIndex = 0
            ElseIf .ListView1.Activation = ItemActivation.TwoClick Then
                ClickActivationComboBox.SelectedIndex = 1
            ElseIf .ListView1.Activation = ItemActivation.Standard Then
                ClickActivationComboBox.SelectedIndex = 2
            End If

            ScrollableCheckBox.Checked = .ListView1.Scrollable
            ScreenWidthNumericUpDown.Value = CDec(.ScreenWidthScrollableExpander)
            ScreenHeightNumericUpDown.Value = CDec(.ScreenHeightScrollableExpander)
            If ScrollableCheckBox.Checked = True Then
                ScreenWidthNumericUpDown.Enabled = True
                ScreenHeightNumericUpDown.Enabled = True
            Else
                ScreenWidthNumericUpDown.Enabled = False
                ScreenHeightNumericUpDown.Enabled = False
            End If

            Dim saveBackupDir As String = GetContainingFolder(.saveBackupsPath)
            If Directory.Exists(saveBackupDir) Then
                If File.Exists(.saveBackupsPath) Then
                    ScreenNameTextBox.Text = NewScreen.GetFileName(.saveBackupsPath)
                ElseIf Not .saveFileNewName = "" Then
                    ScreenNameTextBox.Text = .saveFileNewName
                End If
                TextBox3.Text = saveBackupDir.TrimEnd("\"c) & "\"
            Else
                If Not .saveFileNewName = "" Then
                    ScreenNameTextBox.Text = .saveFileNewName
                Else
                    ScreenNameTextBox.Text = .ThisScreenFile.CurrentScreen.CurrentItemListName & " [Backup File]"
                End If
            End If

            If .saveBackupsMaximum > 0 Then
                NumericUpDown1.Value = .saveBackupsMaximum
            Else
                NumericUpDown1.Value = 0
            End If

            If .PasswordEnabled Then
                PasswordCheckBox.Checked = True
                PasswordTextBox.Visible = False
                PasswordButton.Visible = False
            End If
            PasswordEnabled = .PasswordEnabled
            NewPWrapper = .NewPWrapper

            loading = False ' no longer loading up
        End With
    End Sub

    Dim backgroundChanged As Boolean = False
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim path As String = NewScreen.GetDirectoryString(TextBox1.Text)
        If IO.Directory.Exists(path) Then
            OpenImageFileDialog.InitialDirectory = path ' set initial path to currently used one where a file was selected in the past
            If IO.File.Exists(TextBox1.Text) Then
                OpenImageFileDialog.FileName = NewScreen.GetFileDisplayString(TextBox1.Text)
            Else
                OpenImageFileDialog.FileName = ""
            End If
        End If

        If OpenImageFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox1.Text = OpenImageFileDialog.FileName
            backgroundChanged = True
        End If
    End Sub

    Dim startupSoundChanged As Boolean = False
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim path As String = NewScreen.GetDirectoryString(TextBox2.Text)
        If IO.Directory.Exists(path) Then
            OpenAudioFileDialog.InitialDirectory = path ' set initial path to currently used one where a file was selected in the past
            If IO.File.Exists(TextBox2.Text) Then
                OpenAudioFileDialog.FileName = NewScreen.GetFileDisplayString(TextBox2.Text)
            Else
                OpenAudioFileDialog.FileName = ""
            End If
        End If

        If OpenAudioFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox2.Text = OpenAudioFileDialog.FileName
            startupSoundChanged = True
        End If
    End Sub

    Private Sub ApplyButton_Click(sender As Object, e As EventArgs) Handles ApplyButton.Click
        With NewScreen
            If PasswordTextBox.Text.Length > 0 OrElse PasswordButton.Text = "Reconfirm" OrElse (PasswordCheckBox.Checked AndAlso PasswordTextBox.Visible) Then
                My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
                If MsgBox("Password not confirmed yet, are you sure you want to leave?", MsgBoxStyle.YesNo) <> MsgBoxResult.Yes Then
                    Exit Sub ' cancel apply/leave
                End If
            End If

            .AutoSaveEnabled = AutoSaveCheckBox.Checked

            If backgroundChanged Then
                If IO.File.Exists(TextBox1.Text) Then
                    .backgroundPicture = Image.FromFile(TextBox1.Text)
                Else
                    .backgroundPicture = My.Resources.BlackPixel ' also occurs when background is cleared on options screen
                End If
                .tempBackgroundLocation = TextBox1.Text ' set string for save file saving
                .RefreshBackgroundImage()
            End If

            If startupSoundChanged Then
                If IO.File.Exists(TextBox2.Text) Then
                    .startupSound = TextBox2.Text
                Else
                    .startupSound = "" ' set to blank so nothing plays
                End If
            End If

            If RealDeleteCheckBox.Checked = True Then
                .realDeleteEnabled = True
            Else
                .realDeleteEnabled = False
            End If

            If EmbedIconsCheckBox.Checked = True Then
                .embedIconsEnabled = True
            Else
                .embedIconsEnabled = False
            End If

            If (.ListView1.Scrollable = Not ScrollableCheckBox.Checked) OrElse
            (ScrollableExpanderNumericsChanged = True OrElse .ScrollableExpander = False) Then ' if scrollable is changed to not same as listview scrollable setting

                RemoveExpanderItem() ' remove expander listview item if it exists

                If ScrollableCheckBox.Checked = True Then
                    .ScreenWidthScrollableExpander = CInt(ScreenWidthNumericUpDown.Value)
                    .ScreenHeightScrollableExpander = CInt(ScreenHeightNumericUpDown.Value)
                    If ScreenWidthNumericUpDown.Value > 1 OrElse ScreenHeightNumericUpDown.Value > 1 Then
                        Dim tmpWidth As Integer = CInt(ScreenWidthNumericUpDown.Value) * .ListView1.Width,
                        tmpHeight As Integer = CInt(ScreenHeightNumericUpDown.Value) * .ListView1.Height
                        .ScrollableExpander = True
                        Dim newItem As New ListViewItem
                        newItem.Name = "" ' hidden full path
                        newItem.Text = "*[Screen Expander]*" ' visible label
                        .ListView1.Items.Add(newItem)
                        .ListView1.Items(.ListView1.Items.Count - 1).Position = New Point(tmpWidth - 102, tmpHeight - 102)
                    End If
                End If
                .saveAndRestoreItemPositions = True ' items are restored in closed event for this form
                .StoreItemPositions()
            End If

            .ListView1.Scrollable = ScrollableCheckBox.Checked
            .ListView1.AutoArrange = AutoArrangeCheckBox.Checked

            If AlignmentComboBox.SelectedIndex = 0 AndAlso (Not .ListView1.Alignment = ListViewAlignment.Top) Then ' 0 value respresents Left
                .ListView1.Alignment = ListViewAlignment.Top ' setting to Top is actually what Left is at the start of program
            ElseIf AlignmentComboBox.SelectedIndex = 1 AndAlso (Not .ListView1.Alignment = ListViewAlignment.Left) Then ' 1 value respresents Top
                .ListView1.Alignment = ListViewAlignment.Left ' setting to Left is actually what Top is at the start of program
            ElseIf AlignmentComboBox.SelectedIndex = 2 AndAlso (Not .ListView1.Alignment = ListViewAlignment.SnapToGrid) Then ' 2 value respresents Snap To Grid
                .ListView1.Alignment = ListViewAlignment.SnapToGrid
            End If

            If ClickActivationComboBox.SelectedIndex = 0 Then
                .ListView1.Activation = ItemActivation.OneClick
            ElseIf ClickActivationComboBox.SelectedIndex = 1 Then
                .ListView1.Activation = ItemActivation.TwoClick
            ElseIf ClickActivationComboBox.SelectedIndex = 2 Then
                .ListView1.Activation = ItemActivation.Standard
            End If

            .saveBackupsPath = TextBox3.Text & ScreenNameTextBox.Text & ".VFCScreen"
            .saveFileNewName = ScreenNameTextBox.Text
            .saveBackupsMaximum = CInt(NumericUpDown1.Value)

            .PasswordEnabled = PasswordEnabled
            .NewPWrapper = NewPWrapper


            If saveOnceFlag Then ' save file is being used and one save must be made (autosave disabling)
                .SaveSettings()
            End If
        End With
        Me.Close()
    End Sub

    Private Sub Options_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        If NewScreen.saveAndRestoreItemPositions = True Then ' restore items (things like changing scrollable setting invoke this)
            NewScreen.RestoreItemPositions() ' restore item positions since they needed to be saved when changing a setting for listview
        End If
        NewScreen.ResizeBackgroundImage() ' refresh background (needed just because of scrollable changing after reloading save with saved scroll (tested with height))
    End Sub

    ''' <summary>
    ''' Removes the screen expander item used solely for expanding the screen view when scrollable setting is enabled.
    ''' </summary>
    Private Shared Sub RemoveExpanderItem()
        For Each item As ListViewItem In NewScreen.ListView1.Items
            If item.Text = "*[Screen Expander]*" AndAlso item.Name = "" Then ' used for scrollable screen expanding
                NewScreen.ScrollableExpander = False
                item.Remove()
                Exit For
            End If
        Next
    End Sub

    Private Sub CancelChangesButton_Click(sender As Object, e As EventArgs) Handles CancelChangesButton.Click
        Me.Close()
    End Sub

    Private Sub AutoArrangeCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles AutoArrangeCheckBox.CheckedChanged
        If AutoArrangeCheckBox.Checked = True AndAlso Not loading Then
            MsgBox("Activating this option will move current icons around from their current state." &
                   vbNewLine & "Do not enable this option if you do not want to mess up your current layout!", MsgBoxStyle.Exclamation, "Warning")
        End If
    End Sub

    Public saveOnceFlag As Boolean = False ' flag for saving when apply button is pressed (at the end of its statements)
    Private Sub AutoSaveCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles AutoSaveCheckBox.CheckedChanged
        If AutoSaveCheckBox.Checked = False AndAlso Not loading Then
            MsgBox("If you don't want your changes to be lost on exit you will have to manually use the ""Save As"" menu to save from now on if you disable autosaving." &
               vbNewLine & "Remember you can always use the ""Exit Wihout Saving"" button in the ""Save As"" menu to exit without saving if you want to leave autosave on, but still undo changes made since last save.", MsgBoxStyle.Information, "Warning")
            MsgBox("The program will also have to save all current changes when you hit the apply button however in order to disable autosaving from now on.", MsgBoxStyle.Exclamation, "Warning")
            saveOnceFlag = True
        Else
            saveOnceFlag = False
        End If
    End Sub

    Private Sub ScrollableCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles ScrollableCheckBox.CheckedChanged
        If ScrollableCheckBox.Checked = True AndAlso Not loading Then
            'MsgBox("Activating this option will move current icons around from their current state." & _
            '       vbNewLine & "Do not enable this option if you do not want to mess up your current layout!", MsgBoxStyle.Exclamation, "Warning")
            ScreenWidthNumericUpDown.Enabled = True
            ScreenHeightNumericUpDown.Enabled = True
        ElseIf ScrollableCheckBox.Checked = False AndAlso Not loading Then
            ScreenWidthNumericUpDown.Enabled = False
            ScreenHeightNumericUpDown.Enabled = False
        End If
    End Sub

    Private Sub BrowseBackupFolderButton_Click(sender As Object, e As EventArgs) Handles BrowseBackupFolderButton.Click
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox3.Text = FolderBrowserDialog1.SelectedPath.TrimEnd("\"c) & "\"
        End If
    End Sub

    Private Sub RealDeleteCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles RealDeleteCheckBox.CheckedChanged
        If RealDeleteCheckBox.Checked = True AndAlso Not loading Then
            MsgBox("Activating this option means that when you remove an item on your screen it will also delete the real thing on your operating system." &
                   vbNewLine & "Do not enable this option if you do not want to have files or folder deleted from your system!", MsgBoxStyle.Exclamation, "Warning")
        End If
    End Sub

    Private Sub ClearBackgroundImageButton_Click(sender As Object, e As EventArgs) Handles ClearBackgroundImageButton.Click
        If TextBox1.Text <> "" Then
            TextBox1.Text = ""
            backgroundChanged = True
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub ClearBackgroundImageButton2_Click(sender As Object, e As EventArgs) Handles ClearBackgroundImageButton2.Click
        If TextBox2.Text <> "" Then
            TextBox2.Text = ""
            startupSoundChanged = True
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Dim ScrollableExpanderNumericsChanged As Boolean = False
    Private Sub ScreenWidthNumericUpDown_ValueChanged(sender As Object, e As EventArgs) Handles ScreenWidthNumericUpDown.ValueChanged
        ScrollableExpanderNumericsChanged = True
    End Sub

    Private Sub ScreenHeightNumericUpDown_ValueChanged(sender As Object, e As EventArgs) Handles ScreenHeightNumericUpDown.ValueChanged
        ScrollableExpanderNumericsChanged = True
    End Sub

    Private Sub AboutButton_Click(sender As Object, e As EventArgs) Handles AboutButton.Click
        About.Show()
    End Sub

    Private Sub SaveBackupOpenButton_Click(sender As Object, e As EventArgs) Handles SaveBackupOpenButton.Click
        If Directory.Exists(TextBox3.Text) Then
            Try
                Process.Start(TextBox3.Text)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
            End Try
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub SaveBackupClearButton_Click(sender As Object, e As EventArgs) Handles SaveBackupClearButton.Click
        If TextBox3.Text <> "" Then
            TextBox3.Text = ""
        Else
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Shared Function GetContainingFolder(Path As String) As String
        If Not IsNothing(Path) Then
            Dim Folders() As String = Split(Path, "\")
            If Folders.Length > 0 Then
                Dim Result As String = Folders(0)
                For i As Integer = 1 To Folders.Length - 2 Step 1
                    Result &= "\" & Folders(i)
                Next
                Return Result
            Else
                Return ""
            End If
        Else
            Return ""
        End If
    End Function

    Dim charactersDisallowed As String = "\/:*?""<>|"
    Private Sub ScreenNameTextBox_TextChanged(sender As Object, e As EventArgs) Handles ScreenNameTextBox.TextChanged
        PreventDisallowedCharacters(CType(sender, TextBox), charactersDisallowed)
    End Sub

    Dim charactersDisallowed2 As String = "/*?""<>|"
    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        PreventDisallowedCharacters(CType(sender, TextBox), charactersDisallowed2)
    End Sub

    Private Shared Sub PreventDisallowedCharacters(TxtBox As TextBox, ByRef DisallowedChars As String)
        Dim NewText As String = TxtBox.Text
        Dim Letter As String
        Dim SelectionIndex As Integer = TxtBox.SelectionStart
        Dim Change As Integer

        For i As Integer = 0 To TxtBox.Text.Length - 1
            Letter = TxtBox.Text.Substring(i, 1)
            If DisallowedChars.Contains(Letter) Then
                NewText = NewText.Replace(Letter, "")
                Change += 1
                My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk)
            End If
        Next
        TxtBox.Text = NewText
        TxtBox.Select(SelectionIndex - Change, 0)
    End Sub

    Private Sub PasswordCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles PasswordCheckBox.CheckedChanged
        If PasswordCheckBox.Checked = True Then
            PasswordButton.Visible = True
            PasswordTextBox.Visible = True
            PasswordTextBox.Focus()
        Else
            PasswordEnabled = False
            PasswordButton.Visible = False
            PasswordTextBox.Visible = False
            PasswordTextBox.Text = ""
            PasswordButton.Text = "Confirm"
        End If
    End Sub

    Dim PasswordTest As String
    Private Sub PasswordButton_Click(sender As Object, e As EventArgs) Handles PasswordButton.Click
        If PasswordButton.Text = "Confirm" Then
            If PasswordTextBox.Text <> "" Then
                Dim wrapper As New Simple3Des(PasswordTextBox.Text)
                PasswordTest = wrapper.EncryptData("TestString")
                NewPWrapper = Nothing
                PasswordTextBox.Text = ""
                PasswordTextBox.Focus()
                PasswordButton.Text = "Reconfirm"
            Else
                My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
            End If
        ElseIf PasswordButton.Text = "Reconfirm" Then
            If PasswordTextBox.Text <> "" Then
                Dim wrapper As New Simple3Des(PasswordTextBox.Text)
                If wrapper.DecryptData(PasswordTest) = "TestString" Then
                    NewPWrapper = wrapper
                    PasswordEnabled = True
                    PasswordButton.Visible = False
                    PasswordTextBox.Visible = False
                    MsgBox("Password set.", MsgBoxStyle.Information)
                Else
                    PasswordEnabled = False
                    MsgBox("Passwords do not match.", MsgBoxStyle.Exclamation)
                End If
                PasswordTextBox.Text = ""
                PasswordButton.Text = "Confirm"
            Else
                My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Exclamation)
            End If
        End If
    End Sub

    Private Sub PasswordTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles PasswordTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            PasswordButton.PerformClick()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub
End Class