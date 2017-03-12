Imports Newtonsoft.Json

Public Class LogOn
    Private LoginSuccess As Boolean = False

    Private Sub ConfirmButton_Click(sender As Object, e As EventArgs) Handles ConfirmButton.Click
        If PasswordTextBox.Text.Length > 0 Then
            NewScreen.PWrapper = New Simple3Des(PasswordTextBox.Text)
            If TestWrapper() Then
                NewScreen.Decrypted = True
                LoginSuccess = True
                Me.Close()
            Else
                PasswordTextBox.Text = ""
                PasswordTextBox.Focus()
                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Asterisk)
            End If
        Else
            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
        End If
    End Sub

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.icon
    End Sub

    Private Sub Login_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not LoginSuccess Then
            End
        End If
    End Sub

    Private Sub Login_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        PasswordTextBox.Focus()
    End Sub

    Private Sub Login_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown, PasswordTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            ConfirmButton.PerformClick()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Public Shared Function TestWrapper() As Boolean
        If Not IsNothing(NewScreen.PWrapper) Then
            Dim Data As String = NewScreen.Wrapper.DecryptData(NewScreen.PWrapper.DecryptData(NewScreen.TempText))
            Try
                NewScreen.SavingFlag = True
                NewScreen.ThisScreenFile = JsonConvert.DeserializeObject(Of NewScreen.ScreenFile)(Data)
            Catch ex As Exception
                Return False
            Finally
                NewScreen.SavingFlag = False
            End Try
            If Not IsNothing(NewScreen.ThisScreenFile) AndAlso NewScreen.ThisScreenFile.Decrypted Then
                NewScreen.FailedLoadDeserialize = False
                Return True
            End If
        End If
        Return False
    End Function

End Class