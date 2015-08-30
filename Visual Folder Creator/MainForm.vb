Public Class MainForm

    Private Sub VisualFolderCreator_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.icon
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub CreateScreenButton_Click(sender As Object, e As EventArgs) Handles CreateScreenButton.Click
        NewScreen.Show() ' open new screen creation window
        Me.Close()
    End Sub
End Class
