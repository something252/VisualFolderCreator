Public Class SaveScreen

    Private Sub SaveScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.icon

        SaveScreenDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
    End Sub

    ''' <summary>
    ''' Associate this program with it's custom file extension
    ''' </summary>
    ''' <param name="mainKey">Name used to identify program. Can use spaces I think?</param>
    ''' <param name="ext">Extension including period.</param>
    ''' <param name="iconPath">full Path of the icon to be used for the file extension type. (so install directory icon)</param>
    ''' <param name="fileDescription">appears in right click properties in explorer, so basically program name and maybe save file or whatever appended.</param>
    ''' <param name="appPath">Path of this program.</param>
    Public Sub associateFile(ByVal mainKey As String, ByVal ext As String, ByVal iconPath As String, ByVal fileDescription As String, ByVal appPath As String)
        My.Computer.Registry.ClassesRoot.CreateSubKey(ext).SetValue("", mainKey, Microsoft.Win32.RegistryValueKind.String)
        My.Computer.Registry.ClassesRoot.CreateSubKey(mainKey).SetValue("", fileDescription, Microsoft.Win32.RegistryValueKind.String)
        My.Computer.Registry.ClassesRoot.CreateSubKey(mainKey & "\shell\open\command").SetValue("", appPath & " ""%l"" ", Microsoft.Win32.RegistryValueKind.String)
        If Not (iconPath = "") Then
            My.Computer.Registry.ClassesRoot.CreateSubKey(mainKey & "\DefaultIcon").SetValue("", iconPath, Microsoft.Win32.RegistryValueKind.ExpandString)
        End If
    End Sub

    Private Sub FileAssociationButton_Click(sender As Object, e As EventArgs) Handles FileAssociationButton.Click
        associateFile("Visual Folder Creator", _
                      ".VFCScreen", _
                      "C:\Visual Studio 2013\Projects\Visual Folder Creator\Visual Folder Creator\Resources\icon256.ico", _
                      "Visual Folder Creator Screen File", _
                      Application.ExecutablePath)
    End Sub

    Dim StopProgramEnding As Boolean = False
    Public oldValue As String
    ' tmpSaveLocation is used for when My.Application.CommandLineArgs(0) is empty (newly saved but not yet shutdown screen that tries to save again before shutting down)
    Private Sub SaveScreenButton_Click(sender As Object, e As EventArgs) Handles SaveScreenButton.Click
        oldValue = NewScreen.tmpSaveLocation
        If NewScreen.SaveFileActive = True Then ' already existing save is being used so set to current save file location and name 
            Dim flag1 As Boolean = False, count As Integer = 0, storage1 As Object
            If Not NewScreen.tmpSaveLocation = "" Then
                storage1 = Split(NewScreen.tmpSaveLocation, "\")
            Else
                storage1 = Split(My.Application.CommandLineArgs(0), "\")
            End If
            Dim tmp() As String = storage1

            ' reconstruct directory only path
            Dim newTmp As String = ""
            For i As Integer = 0 To tmp.Count - 2 ' don't add last element (file name and extension)
                newTmp &= tmp(i) & "\"
            Next

            SaveScreenDialog.InitialDirectory = newTmp
            SaveScreenDialog.FileName = tmp(tmp.Count - 1)
            'NewScreen.tmpSaveLocation = SaveScreenDialog.FileName ' very rarely used but needed temp storage, read definition comment
        Else ' new save 
            SaveScreenDialog.FileName = "New Screen" & ".VFCScreen"
        End If
        If SaveScreenDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If NewScreen.SaveFileActive = False OrElse (NewScreen.SaveFileActive = True AndAlso (Not My.Application.CommandLineArgs.Count > 0)) Then
                StopProgramEnding = True ' prevent closing of program when saving options from the "main-exe-non-save-launch" for the first time and also never restarted yet...
                ' so CommandLineArgs is still empty
            Else
                StopProgramEnding = False
            End If
            NewScreen.SaveFileActive = True ' now saving to a file from now on

            'My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
            'If MsgBox("Would you like to keep the same screen title?", MsgBoxStyle.YesNo, "Question") = MsgBoxResult.No Then
            NewScreen.Text = NewScreen.GetFileDisplayString(SaveScreenDialog.FileName)
            'End If

            NewScreen.tmpSaveLocation = SaveScreenDialog.FileName ' very rarely used but needed temp storage, read definition comment
            NewScreen.SaveToSaveFile(SaveScreenDialog.FileName) ' send full path for saving (needs to call directly because no autosave can stop otherwise and more)
            If NewScreen.exitingFirstSave AndAlso Not StopProgramEnding Then
                ' condition when closing this program when it was launched from the main .exe (not a savefile) and just tried closing without saving, 
                ' so now it has been saved now just terminate instead of closing properly here because saving was just performed
                End
            End If
            Me.Close()
        Else ' canceled or aborted etc
            'NewScreen.tmpSaveLocation = oldValue ' so that screen name isn't changed on shutdown
            If Options.saveOnceFlag = True Then
                MsgBox("Autosave not disabled yet, it will take effect after you close this program (where it will then autosave for the last time) or successfully save manually!", MsgBoxStyle.Critical, "Warning")
            End If
        End If
    End Sub

    Private Sub SaveScreen2Button_Click(sender As Object, e As EventArgs) Handles SaveScreen2Button.Click ' save button
        If NewScreen.SaveFileActive Then
            'NewScreen.SaveFile()
            NewScreen.SaveSettings()
        Else
            MsgBox("Save file not set!", MsgBoxStyle.Exclamation, "Error")
            SaveScreenButton_Click(sender, e) ' save as
        End If
    End Sub

    Private Sub QuitButton_Click(sender As Object, e As EventArgs) Handles QuitButton.Click ' exit without saving button
        My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
        If MsgBox("This will exit the entire program right now and not save any changes made." & vbNewLine & "Proceed?", MsgBoxStyle.OkCancel, "Warning") = MsgBoxResult.Ok Then
            End
        End If
    End Sub

End Class