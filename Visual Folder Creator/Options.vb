Public Class Options
    Dim loading As Boolean = True ' flag for whether loading this form still or not

    Private Sub Options_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.icon

        ScreenNameTextBox.Text = NewScreen.Text

        AutoSaveCheckBox.Checked = NewScreen.AutoSaveEnabled

        If Not NewScreen.tempBackgroundLocation = "" Then
            TextBox1.Text = NewScreen.tempBackgroundLocation
        Else
            TextBox1.Text = ""
        End If

        If IO.File.Exists(NewScreen.startupSound) Then
            TextBox2.Text = NewScreen.startupSound
        Else
            TextBox2.Text = ""
        End If

        If NewScreen.realDeleteEnabled = True Then
            RealDeleteCheckBox.Checked = True
        Else
            RealDeleteCheckBox.Checked = False
        End If

        If NewScreen.embedIconsEnabled = True Then
            EmbedIconsCheckBox.Checked = True
        Else
            EmbedIconsCheckBox.Checked = False
        End If

        AutoArrangeCheckBox.Checked = NewScreen.ListView1.AutoArrange
        If NewScreen.ListView1.Alignment = ListViewAlignment.Top Then
            AlignmentComboBox.SelectedIndex = 0
        ElseIf NewScreen.ListView1.Alignment = ListViewAlignment.Left Then
            AlignmentComboBox.SelectedIndex = 1
        ElseIf NewScreen.ListView1.Alignment = ListViewAlignment.SnapToGrid Then
            AlignmentComboBox.SelectedIndex = 2
        End If
        If NewScreen.ListView1.Activation = ItemActivation.OneClick Then
            ClickActivationComboBox.SelectedIndex = 0
        ElseIf NewScreen.ListView1.Activation = ItemActivation.TwoClick Then
            ClickActivationComboBox.SelectedIndex = 1
        ElseIf NewScreen.ListView1.Activation = ItemActivation.Standard Then
            ClickActivationComboBox.SelectedIndex = 2
        End If

        ScrollableCheckBox.Checked = NewScreen.ListView1.Scrollable
        ScreenWidthNumericUpDown.Value = CDec(NewScreen.ScreenWidthScrollableExpander)
        ScreenHeightNumericUpDown.Value = CDec(NewScreen.ScreenHeightScrollableExpander)
        If ScrollableCheckBox.Checked = True Then
            ScreenWidthNumericUpDown.Enabled = True
            ScreenHeightNumericUpDown.Enabled = True
        Else
            ScreenWidthNumericUpDown.Enabled = False
            ScreenHeightNumericUpDown.Enabled = False
        End If

        If IO.Directory.Exists(NewScreen.saveBackupsDirectory) Then
            TextBox3.Text = NewScreen.saveBackupsDirectory
        End If
        If NewScreen.saveBackupsMaximum > 0 Then
            NumericUpDown1.Value = NewScreen.saveBackupsMaximum
        Else
            NumericUpDown1.Value = 0
        End If

        loading = False ' no longer loading up
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
        NewScreen.Text = ScreenNameTextBox.Text

        NewScreen.AutoSaveEnabled = AutoSaveCheckBox.Checked

        If backgroundChanged Then
            If IO.File.Exists(TextBox1.Text) Then
                NewScreen.backgroundPicture = Image.FromFile(TextBox1.Text)
            Else
                NewScreen.backgroundPicture = My.Resources.BlackPixel ' also occurs when background is cleared on options screen
            End If
            NewScreen.tempBackgroundLocation = TextBox1.Text ' set string for save file saving
            NewScreen.RefreshBackgroundImage()
        End If

        If startupSoundChanged Then
            If IO.File.Exists(TextBox2.Text) Then
                NewScreen.startupSound = TextBox2.Text
            Else
                NewScreen.startupSound = "" ' set to blank so nothing plays
            End If
        End If

        If RealDeleteCheckBox.Checked = True Then
            NewScreen.realDeleteEnabled = True
        Else
            NewScreen.realDeleteEnabled = False
        End If

        If EmbedIconsCheckBox.Checked = True Then
            NewScreen.embedIconsEnabled = True
        Else
            NewScreen.embedIconsEnabled = False
        End If

        If (NewScreen.ListView1.Scrollable = Not ScrollableCheckBox.Checked) OrElse _
            (ScrollableExpanderNumericsChanged = True OrElse NewScreen.ScrollableExpander = False) Then ' if scrollable is changed to not same as listview scrollable setting

            RemoveExpanderItem() ' remove expander listview item if it exists

            If ScrollableCheckBox.Checked = True Then
                NewScreen.ScreenWidthScrollableExpander = CInt(ScreenWidthNumericUpDown.Value)
                NewScreen.ScreenHeightScrollableExpander = CInt(ScreenHeightNumericUpDown.Value)
                If ScreenWidthNumericUpDown.Value > 1 OrElse ScreenHeightNumericUpDown.Value > 1 Then
                    Dim tmpWidth As Integer = CInt(ScreenWidthNumericUpDown.Value) * NewScreen.ListView1.Width, _
                        tmpHeight As Integer = CInt(ScreenHeightNumericUpDown.Value) * NewScreen.ListView1.Height
                    NewScreen.ScrollableExpander = True
                    Dim newItem As New ListViewItem
                    newItem.Name = "" ' hidden full path
                    newItem.Text = "*[Screen Expander]*" ' visible label
                    NewScreen.ListView1.Items.Add(newItem)
                    NewScreen.ListView1.Items(NewScreen.ListView1.Items.Count - 1).Position = New Point(tmpWidth - 102, tmpHeight - 102)
                End If
            End If
            NewScreen.saveAndRestoreItemPositions = True ' items are restored in closed event for this form
            NewScreen.StoreItemPositions()
        End If
        NewScreen.ListView1.Scrollable = ScrollableCheckBox.Checked

        NewScreen.ListView1.AutoArrange = AutoArrangeCheckBox.Checked

        If AlignmentComboBox.SelectedIndex = 0 AndAlso (Not NewScreen.ListView1.Alignment = ListViewAlignment.Top) Then ' 0 value respresents Left
            NewScreen.ListView1.Alignment = ListViewAlignment.Top ' setting to Top is actually what Left is at the start of program
        ElseIf AlignmentComboBox.SelectedIndex = 1 AndAlso (Not NewScreen.ListView1.Alignment = ListViewAlignment.Left) Then ' 1 value respresents Top
            NewScreen.ListView1.Alignment = ListViewAlignment.Left ' setting to Left is actually what Top is at the start of program
        ElseIf AlignmentComboBox.SelectedIndex = 2 AndAlso (Not NewScreen.ListView1.Alignment = ListViewAlignment.SnapToGrid) Then ' 2 value respresents Snap To Grid
            NewScreen.ListView1.Alignment = ListViewAlignment.SnapToGrid
        End If

        If ClickActivationComboBox.SelectedIndex = 0 Then
            NewScreen.ListView1.Activation = ItemActivation.OneClick
        ElseIf ClickActivationComboBox.SelectedIndex = 1 Then
            NewScreen.ListView1.Activation = ItemActivation.TwoClick
        ElseIf ClickActivationComboBox.SelectedIndex = 2 Then
            NewScreen.ListView1.Activation = ItemActivation.Standard
        End If
        ' save backups
        With NewScreen
            .saveBackupsDirectory = TextBox3.Text
            .saveBackupsMaximum = NumericUpDown1.Value
        End With

        'below must be the last most executed stuff for the apply button
        If NewScreen.SaveFileActive = False Then
            MsgBox("You have currently not saved this screen to a file yet." & vbNewLine & "For changes to be saved you must save the screen first using the ""Save As"" menu button located in the right click menu.", MsgBoxStyle.Exclamation, "Warning")
            NewScreen.SaveSettings()
        ElseIf saveOnceFlag Then ' save file is being used and one save must be made (autosave disabling)
            NewScreen.SaveSettings()
        End If
        Me.Close()
    End Sub

    ''' <summary>
    ''' Removes the screen expander item used solely for expanding the screen view when scrollable setting is enabled.
    ''' </summary>
    Private Sub RemoveExpanderItem()
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
            MsgBox("Activating this option will move current icons around from their current state." & _
                   vbNewLine & "Do not enable this option if you do not want to mess up your current layout!", MsgBoxStyle.Exclamation, "Warning")
        End If
    End Sub

    Public saveOnceFlag As Boolean = False ' flag for saving when apply button is pressed (at the end of its statements)
    Private Sub AutoSaveCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles AutoSaveCheckBox.CheckedChanged
        If AutoSaveCheckBox.Checked = False AndAlso Not loading Then
            MsgBox("If you don't want your changes to be lost on exit you will have to manually use the ""Save As"" menu to save from now on if you disable autosaving." & _
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
            TextBox3.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub RealDeleteCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles RealDeleteCheckBox.CheckedChanged
        If RealDeleteCheckBox.Checked = True AndAlso Not loading Then
            MsgBox("Activating this option means that when you remove an item on your screen it will also delete the real thing on your operating system." & _
                   vbNewLine & "Do not enable this option if you do not want to have files or folder deleted from your system!", MsgBoxStyle.Exclamation, "Warning")
        End If
    End Sub

    Private Sub ClearBackgroundImageButton_Click(sender As Object, e As EventArgs) Handles ClearBackgroundImageButton.Click
        TextBox1.Text = ""
        backgroundChanged = True
    End Sub

    Private Sub ClearBackgroundImageButton2_Click(sender As Object, e As EventArgs) Handles ClearBackgroundImageButton2.Click
        TextBox2.Text = ""
        startupSoundChanged = True
    End Sub

    Private Sub Options_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        If NewScreen.saveAndRestoreItemPositions = True Then ' restore items (things like changing scrollable setting invoke this)
            NewScreen.RestoreItemPositions() ' restore item positions since they needed to be saved when changing a setting for listview
        End If
        NewScreen.ResizeBackgroundImage() ' refresh background (needed just because of scrollable changing after reloading save with saved scroll (tested with height))
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
        If Not IsNothing(TextBox3.Text) AndAlso System.IO.Directory.Exists(TextBox3.Text) Then
            Try
                Process.Start(TextBox3.Text)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
            End Try
        End If
    End Sub

    Private Sub SaveBackupClearButton_Click(sender As Object, e As EventArgs) Handles SaveBackupClearButton.Click
        TextBox3.Text = ""
    End Sub
End Class